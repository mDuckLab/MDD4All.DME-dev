using System.Reflection;

namespace MDD4All.DME.DataAccess.DataModels
{
    // The types the editor can be pointed at.
    //
    // On this branch the models are compiled into the solution rather than loaded from a DLL at
    // runtime, so there is no assembly to resolve and no load context to keep apart - everything
    // that matters is already in the process. The catalogue simply reads what is there.
    public class DataModelCatalog
    {
        // What a namespace has to contain to count as a data model. Deliberately a substring
        // rather than an exact name, so DataModels, DME.DataModels and anything similar match.
        private const string DataModelNamespaceMarker = "datamodel";

        // The usual list: everything sitting in a namespace that looks like a data model.
        public List<Type> AvailableTypes
        {
            get
            {
                List<Type> result = CollectTypes(onlyDataModelNamespaces: true);

                return result;
            }
        }

        // Every public class in every loaded assembly - the editor's own view models, the UI
        // components, the framework. Thousands of entries, and most of them make no sense to
        // edit. It exists to show that the editor does not care what it is handed.
        public List<Type> AllTypes
        {
            get
            {
                List<Type> result = CollectTypes(onlyDataModelNamespaces: false);

                return result;
            }
        }

        public bool CanCreateInstance(Type type)
        {
            bool result = !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null;

            return result;
        }

        // Turns the "$type" out of a file back into a type. The name is assembly-qualified
        // ("Namespace.Type, AssemblyName"), and only the part before the comma is used - a file
        // written by an older build may well name a different assembly than the one it is in now.
        public Type? ResolveTypeName(string qualifiedTypeName)
        {
            Type? result = null;

            string typeName = qualifiedTypeName.Split(',')[0].Trim();

            if (typeName.Length > 0)
            {
                foreach (Assembly assembly in LoadedAssemblies())
                {
                    result = assembly.GetType(typeName);

                    if (result != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        private List<Type> CollectTypes(bool onlyDataModelNamespaces)
        {
            List<Type> result = new List<Type>();

            foreach (Assembly assembly in LoadedAssemblies())
            {
                foreach (Type type in ExportedTypesOf(assembly))
                {
                    if (type.IsClass && !type.IsNested)
                    {
                        if (!onlyDataModelNamespaces || IsDataModelNamespace(type.Namespace))
                        {
                            result.Add(type);
                        }
                    }
                }
            }

            result.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.Ordinal));

            return result;
        }

        private bool IsDataModelNamespace(string? namespaceName)
        {
            bool result = false;

            if (namespaceName != null)
            {
                result = namespaceName.ToLowerInvariant().Contains(DataModelNamespaceMarker);
            }

            return result;
        }

        private List<Assembly> LoadedAssemblies()
        {
            List<Assembly> result = new List<Assembly>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                {
                    result.Add(assembly);
                }
            }

            return result;
        }

        // An assembly can refuse to list its types - a missing dependency is enough. Skipping it
        // is the only sensible answer here, since one unreadable assembly must not take the whole
        // list down.
        private List<Type> ExportedTypesOf(Assembly assembly)
        {
            List<Type> result = new List<Type>();

            try
            {
                result.AddRange(assembly.GetExportedTypes());
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}
