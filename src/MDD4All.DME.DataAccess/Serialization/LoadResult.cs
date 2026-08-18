namespace MDD4All.DME.DataAccess.Serialization
{
    // What came of an attempt to turn file content back into an object. Says what happened, not
    // what to tell the user - phrasing that is the job of whoever is closer to the screen.
    public enum LoadResult
    {
        Loaded,
        FileNotReadable,
        NotReadableAsJson,
        DoesNotMatchType,
        DeserializationFailed,
        NoObject
    }
}
