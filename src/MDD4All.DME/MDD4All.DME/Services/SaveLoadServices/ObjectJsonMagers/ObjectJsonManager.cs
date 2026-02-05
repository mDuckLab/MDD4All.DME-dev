using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MDD4All.DME.Services
{
    public class ObjectJsonManager : ObservableObject
    {
        public ObjectJsonManager()
        {
            this.SerializerSettings = new JsonSerializerSettings
            {
                // Includes the full C# type name in the JSON (as $type). 
                // This is vital for deserializing inherited classes correctly.
                TypeNameHandling = TypeNameHandling.All,
                // Forces the reader to look for metadata (like $type or $id) at the beginning.
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                // Ensures that the same object isn't saved twice; instead, it uses references ($id/$ref).
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                // Prevents the serializer from crashing if objects point to each other in a circle.
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                // Explicitly writes 'null' into the JSON file instead of skipping the property.
                NullValueHandling = NullValueHandling.Include,
                // Ensures a "fresh start" by replacing existing collections and objects instead of 
                // appending new data to them. This prevents data pollution and duplicate entries.
                // Example: If a list currently has 3 items and you load a file containing 2 items, 
                // 'Replace' ensures the list has exactly 2 items. Without this, the list would 
                // incorrectly grow to 5 items due to default 'Append' behavior.
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                // Allows the use of private or internal constructors when creating objects from JSON.
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                // Uses a simplified assembly name in the $type metadata for better compatibility.
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                // Formats the resulting JSON string with indentation and line breaks for human readability.
                Formatting = Formatting.Indented,
                // Adds a custom converter to handle Dictionary structures correctly during conversion.
                // This converter handles the transformation of IDictionary objects.
                // It solves the problem that standard JSON only allows strings as keys, 
                // whereas C# dictionaries can use complex objects as keys.
                Converters = new List<JsonConverter> { new DictionaryJsonConverter() }
            };
        }

        private object? _activeObject;
        public object? ActiveObject
        {
            get
            {
                return _activeObject;
            }
            set
            {
                if (_activeObject != value)
                {
                    _activeObject = value;
                    OnPropertyChanged(nameof(ActiveObject));
                }
            }
        }

        private Type? _selectedType;
        public Type? SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged(nameof(SelectedType));
                }
            }
        }

        private bool _isNamespaceFilterActive = true;
        public bool IsNamespaceFilterActive
        {
            get
            {
                return _isNamespaceFilterActive;
            }
            set
            {
                if (_isNamespaceFilterActive != value)
                {
                    _isNamespaceFilterActive = value;
                    OnPropertyChanged(nameof(IsNamespaceFilterActive));
                    // We also notify that the list of types might have changed
                    OnPropertyChanged(nameof(GetAvailableDataModels));
                }
            }
        }

        public JsonSerializerSettings SerializerSettings { get; private set; }

        public string ActiveObjectJsonString
        {
            get
            {
                string result = string.Empty;

                if (ActiveObject != null && SelectedType != null)
                {
                    result = JsonConvert.SerializeObject(ActiveObject, this.SerializerSettings);
                }

                return result;
            }
        }

        public void CreateNewInstance()
        {
            if (SelectedType != null)
            {
                // Assigning to the property triggers OnPropertyChanged
                this.ActiveObject = Activator.CreateInstance(SelectedType);
            }
        }


        public Type? FindTypeByFullName(string typeFullName)
        {
            // Example Input: "MDD4All.DME.DataModels.PersonsExamples.Person, MDD4All.DME"
            Type? result = null;

            if (!string.IsNullOrEmpty(typeFullName))
            {
                string cleanTypeName = typeFullName.Split(',')[0].Trim();
                // Result: "MDD4All.DME.DataModels.PersonsExamples.Person"
                // The assembly info ", MDD4All.DME" is removed to match the C# FullName.

                // Search for the Person type in your available models
                result = GetAvailableDataModels().FirstOrDefault(type => type.FullName == cleanTypeName);
            }

            return result;
        }

        public List<Type> GetAvailableDataModels()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> filteredTypes = new List<Type>();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && type.IsPublic && !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null)
                {
                    if (IsNamespaceFilterActive)
                    {
                        if (type.Namespace != null && type.Namespace.Contains("DataModels"))
                        {
                            filteredTypes.Add(type);
                        }
                    }
                    else
                    {
                        if (type.Namespace != null && !type.Namespace.StartsWith("Microsoft") && !type.Namespace.StartsWith("System"))
                        {
                            filteredTypes.Add(type);
                        }
                    }
                }
            }
            return filteredTypes;
        }

        public object? LoadFromContent(string jsonContent)
        {
            object? result = null;

            if (!string.IsNullOrEmpty(jsonContent))
            {
                try
                {
                    // Parse the raw JSON string into a JObject to inspect its properties manually
                    Newtonsoft.Json.Linq.JObject rawJson = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);

                    // Search for the "$type" property which holds the class metadata
                    Newtonsoft.Json.Linq.JToken? typeToken = rawJson["$type"];

                    if (typeToken != null)
                    {
                        // Convert the type string into a real C# System.Type
                        Type? type = FindTypeByFullName(typeToken.ToString());

                        if (type != null)
                        {
                            // Update the manager's selected type to match the imported data
                            this.SelectedType = type;

                            // Deserialize the JSON into the specific C# object instance
                            result = JsonConvert.DeserializeObject(jsonContent, type, this.SerializerSettings);

                            // Assign the result to ActiveObject to trigger a UI and tree refresh
                            this.ActiveObject = result;
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex.Message);
#endif
                }
            }
            return result;
        }

        public (string FileName, string Base64Data)? GetExportPackage()
        {
            (string FileName, string Base64Data)? result = null;

            if (!string.IsNullOrEmpty(ActiveObjectJsonString) && SelectedType != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(ActiveObjectJsonString);
                string base64 = Convert.ToBase64String(bytes);
                string name = SelectedType.Name + ".json";

                result = (name, base64);
            }
            return result;
        }

    }
}