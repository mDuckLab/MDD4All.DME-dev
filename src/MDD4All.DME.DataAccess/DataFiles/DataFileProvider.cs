using MDD4All.DME.DataAccess.Serialization;

namespace MDD4All.DME.DataAccess.DataFiles
{
    // A data file on one side, an object on the other. Everything in between - reading the file,
    // deciding which format it is in, handing it to the serializer - happens here.
    //
    // What is deliberately not here: which file, whether the user should be asked first, and what
    // to say when it does not work. Those are decisions, and decisions belong to the view model.
    public class DataFileProvider
    {
        private readonly DataSerializer _dataSerializer;

        public DataFileProvider(DataSerializer dataSerializer)
        {
            _dataSerializer = dataSerializer;
        }

        // Nothing is handed back unless the result is Loaded, so a caller cannot accidentally
        // take over a half-built object.
        public LoadResult Read(string filePath, Type rootType, bool verifyRootType, out object? loadedObject)
        {
            LoadResult result;

            loadedObject = null;

            string content = "";
            bool contentRead = false;

            try
            {
                content = File.ReadAllText(filePath);
                contentRead = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            if (!contentRead)
            {
                result = LoadResult.FileNotReadable;
            }
            else
            {
                result = _dataSerializer.LoadFromJson(content, rootType, verifyRootType, out loadedObject);
            }

            return result;
        }

        public void Write(string filePath, object rootObject, bool includeTypeInformation,
                          bool writeComplexDictionaryKeys)
        {
            string content = _dataSerializer.ToJson(rootObject, includeTypeInformation,
                                                    writeComplexDictionaryKeys);

            File.WriteAllText(filePath, content);
        }

        // Which data model a file belongs to, read from the file itself without building anything.
        // Returns null for a file written with the type information setting off.
        public string? ReadTypeName(string filePath)
        {
            string? result = null;

            try
            {
                result = DataSerializer.ReadTypeNameFromJson(File.ReadAllText(filePath));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return result;
        }
    }
}
