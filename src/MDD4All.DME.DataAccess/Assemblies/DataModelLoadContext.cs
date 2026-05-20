using MDD4All.AssemblyLoading.Contracts;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace MDD4All.DME.DataAccess.Assemblies
{
    public class DataModelLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public DataModelLoadContext(string dllPath)
        {
            FileInfo dllFileInfo = new FileInfo(dllPath);
            
            string pluginPath = "";

            if (dllFileInfo != null && dllFileInfo.Directory != null)
            {
                pluginPath = dllFileInfo.Directory.FullName;
                _resolver = new AssemblyDependencyResolver(pluginPath);
            }
            else
            {
                throw new Exception("Wrong dll path.");
            }
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
