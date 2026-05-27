using MDD4All.AssemblyLoading.Contracts;
using System.Reflection;

namespace MDD4All.DME.DataAccess.Assemblies
{
    public class AssemblyPovider : IAssemblyProvider
    {
        public string? ProxiesDllPath { get; set; }

        public Assembly GetAssemblyByPath(string path)
        {
            DataModelLoadContext dataModelLoadContext = new DataModelLoadContext(path, ProxiesDllPath);

            return dataModelLoadContext.LoadFromAssemblyPath(path);
        }
    }
}
