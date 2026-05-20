using MDD4All.AssemblyLoading.Contracts;
using System.Reflection;

namespace MDD4All.DME.DataAccess.Assemblies
{
    public class AssemblyPovider : IAssemblyProvider
    {
        public Assembly GetAssemblyByPath(string path)
        {
            DataModelLoadContext dataModelLoadContext = new DataModelLoadContext(path);

            return dataModelLoadContext.LoadFromAssemblyPath(path);
        }
    }
}
