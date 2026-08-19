using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace MDD4All.DME.DataAccess.Serialization
{
    // Turns objects into text and back. Keeps nothing of what passes through: every method is
    // handed what it needs and gives the result back, so asking it something never changes what
    // is currently open. The object itself lives in DataManagerObjectViewModel.
    public class DataSerializer
    {
        public object? CreateInstance(Type rootType)
        {
            object? result = Activator.CreateInstance(rootType);

            return result;
        }

        // The settings, built fresh per call so one call cannot reconfigure the next.
        private JsonSerializerSettings CommonSettings()
        {
            JsonSerializerSettings result = new JsonSerializerSettings
            {
                // Wherever the declared type does not say enough, the real one is written into the
                // file as an extra property named $type:
                //
                //     "Pet": { "$type": "PetShop.Dog, PetShop", "Name": "Rex" }
                //
                // A property declared as Animal holding a Dog would otherwise come back as an
                // Animal, so a data model built on inheritance loses everything below the base
                // class. "Auto" means this is written only where it is needed, not everywhere.
                //
                // The root object is the exception and needs help - see ToJson.
                TypeNameHandling = TypeNameHandling.Auto,

                // A property cleared to null has to appear in the file as null. Otherwise reading
                // leaves whatever the constructor put there and the cleared value reappears.
                NullValueHandling = NullValueHandling.Include,

                // Collections are replaced, not appended to. A list the constructor filled with
                // three items would otherwise hold five after loading a file that has two.
                ObjectCreationHandling = ObjectCreationHandling.Replace,

                // Data files are read and diffed by hand, so they are written across several lines
                // rather than as one.
                Formatting = Formatting.Indented
            };

            return result;
        }

        // Converters are Json.NET's list of exceptions: walking the graph it asks each of them
        // CanConvert(type) for every value it meets, and the first one to say yes writes that
        // value in its place. The settings above still apply to everything around it.
        //
        // The one registered here exists because JSON allows only strings as property names, so a
        // dictionary keyed by an object has nowhere to put its keys.
        private JsonSerializerSettings SettingsWithConverter(bool writeComplexDictionaryKeys)
        {
            JsonSerializerSettings result = CommonSettings();

            result.Converters = new List<JsonConverter>
            {
                new DictionaryJsonConverter(writeComplexDictionaryKeys)
            };

            return result;
        }

        // includeTypeInformation writes the type into the file, so a file can name its own model.
        //
        // writeComplexDictionaryKeys off writes a dictionary keyed by an object as null instead of
        // the Key/Value form only this application understands. Costs the content, keeps the file
        // readable elsewhere.
        public string ToJson(object rootObject, bool includeTypeInformation, bool writeComplexDictionaryKeys)
        {
            string result = string.Empty;

            JsonSerializerSettings settings = SettingsWithConverter(writeComplexDictionaryKeys);

            if (includeTypeInformation)
            {
                // The exception mentioned at TypeNameHandling above. Inside the graph every value
                // has a declared type to be measured against, but the root has none - Json.NET
                // knows what it was handed and sees no reason to record it. Declaring the root as
                // "object" creates that gap on purpose, and Auto fills it in.
                result = JsonConvert.SerializeObject(rootObject, typeof(object), settings);
            }
            else
            {
                result = JsonConvert.SerializeObject(rootObject, settings);
            }

            return result;
        }


        // Turns JSON text into an object, or reports why it could not.
        // Takes the text, never a file path.
        //
        // verifyRootType means the type is only a guess and has to be held against the file.
        // It is false when the file named its own type - then there is nothing to verify.
        public LoadResult LoadFromJson(string json, Type targetType, bool verifyRootType, out object? loadedObject)
        {
            // Three checks, ordered by how much each one can explain:
            //
            //   syntax        - this is not JSON at all
            //   names         - this is JSON, but not written from this type
            //   deserializing - something went wrong
            //
            // Whichever can give the better answer runs first. The starting value is a failure,
            // so success has to be reached rather than assumed, and nothing is handed back until
            // all three have passed.
            LoadResult result = LoadResult.DeserializationFailed;

            loadedObject = null;

            Newtonsoft.Json.Linq.JToken? rawJson = null;

            // Syntax. The parsed result is also what the name comparison below reads from.
            try
            {
                rawJson = Newtonsoft.Json.Linq.JToken.Parse(json);
            }
            catch (Exception exception)
            {
                result = LoadResult.NotReadableAsJson;
                Console.WriteLine(exception);
            }

            if (rawJson != null)
            {
                bool namesMatch = true;

                // Names. Skipped for a stated type, and for a root that is an array - an array
                // carries no names to compare, which happens when the data model root is a list.
                Newtonsoft.Json.Linq.JObject? rootObject = rawJson as Newtonsoft.Json.Linq.JObject;

                if (verifyRootType && rootObject != null)
                {
                    // The target type is either the type read from the file's own $type or the
                    // model the user currently has selected. Here it supplies the names that are
                    // allowed to appear.
                    List<string> knownNames = new List<string>();

                    foreach (PropertyInfo property in targetType.GetProperties())
                    {
                        knownNames.Add(property.Name);
                    }

                    // The other side: the names the file actually carries. $type and $id are
                    // written by Json.NET itself and belong to no class, so they stay out.
                    List<string> fileNames = new List<string>();

                    foreach (Newtonsoft.Json.Linq.JProperty jsonProperty in rootObject.Properties())
                    {
                        if (!jsonProperty.Name.StartsWith("$"))
                        {
                            fileNames.Add(jsonProperty.Name);
                        }
                    }

                    // Compared in this direction on purpose. A name the type does not know means
                    // the file belongs to something else. A name missing from the file does not,
                    // which is what lets a file written before a property was added still load.
                    foreach (string fileName in fileNames)
                    {
                        if (!knownNames.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                        {
                            namesMatch = false;
                            break;
                        }
                    }
                }

                if (!namesMatch)
                {
                    result = LoadResult.DoesNotMatchType;
                }
                else
                {
                    // Deserializing. The text goes in rather than the parsed result above, because
                    // Json.NET has to see the raw "$type" entries to resolve them itself.
                    try
                    {
                        object? deserializedJson = JsonConvert.DeserializeObject(json, targetType,
                                                                                 SettingsWithConverter(true));

                        // A file containing just "null" parses fine and deserializes to nothing.
                        if (deserializedJson == null)
                        {
                            result = LoadResult.NoObject;
                        }
                        else
                        {
                            loadedObject = deserializedJson;
                            result = LoadResult.Loaded;
                        }
                    }
                    catch (Exception exception)
                    {
                        // result is still DeserializationFailed, so there is nothing to set here.
                        Console.WriteLine(exception);
                    }
                }
            }

            return result;
        }


        // Reads only the $type metadata Json.NET wrote into the file, without deserializing the rest.
        // The result is assembly-qualified ("Namespace.Type, AssemblyName") - callers that need the
        // plain type name have to strip the assembly part themselves.
        public static string? ReadTypeNameFromJson(string jsonContent)
        {
            string? result = null;

            try
            {
                Newtonsoft.Json.Linq.JObject rawJson = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
                Newtonsoft.Json.Linq.JToken? typeToken = rawJson["$type"];

                if (typeToken != null)
                {
                    result = typeToken.ToString();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return result;
        }

    }
}
