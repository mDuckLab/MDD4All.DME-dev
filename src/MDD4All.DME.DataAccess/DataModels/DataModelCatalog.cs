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
        private readonly Assembly _dataModelAssembly;

        public DataModelCatalog(Assembly dataModelAssembly)
        {
            _dataModelAssembly = dataModelAssembly;
        }

        // The usual list: the classes written for this application to edit.
        //
        // Picked by assembly rather than by namespace. A namespace containing "DataModels" looks
        // like the obvious test but catches MDD4All.UI.DataModels too, which holds ITreeNode and
        // friends - library types nobody wants to edit.
        public List<Type> AvailableTypes
        {
            get
            {
                List<Type> result = new List<Type>();

                foreach (Type type in ExportedTypesOf(_dataModelAssembly))
                {
                    if (IsEditable(type))
                    {
                        result.Add(type);
                    }
                }

                SortByName(result);

                return result;
            }
        }

        // Every class written for this application, in any of its own assemblies - the view
        // models, the data access, the models. Not the framework and not the MDD4All libraries:
        // those are somebody else's code and there is nothing to be learned from editing them.
        public List<Type> AllTypes
        {
            get
            {
                List<Type> result = new List<Type>();

                foreach (Assembly assembly in ApplicationAssemblies())
                {
                    foreach (Type type in ExportedTypesOf(assembly))
                    {
                        if (IsEditable(type))
                        {
                            result.Add(type);
                        }
                    }
                }

                SortByName(result);

                return result;
            }
        }

        // What can appear in the list at all, before any question of whether New can build it.
        //
        // Delegates are excluded although they are classes - Action and its relatives descend
        // from MulticastDelegate, so IsClass says yes to every one of them. Open generic types
        // go too: List<T> without a T holds nothing, and those are what produced the names with
        // a backtick and a number in them.
        private bool IsEditable(Type type)
        {
            bool result = false;

            if (type.IsClass && !type.IsNested && !type.ContainsGenericParameters)
            {
                result = !typeof(Delegate).IsAssignableFrom(type);
            }

            return result;
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



        private void SortByName(List<Type> types)
        {
            types.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.Ordinal));
        }

        // The application's own assemblies, told apart by their name. Everything this solution
        // builds is called MDD4All.DME.something; the framework, Newtonsoft and the shared
        // MDD4All libraries are not.
        private List<Assembly> ApplicationAssemblies()
        {
            List<Assembly> result = new List<Assembly>();

            foreach (Assembly assembly in LoadedAssemblies())
            {
                string? name = assembly.GetName().Name;

                if (name != null && name.StartsWith("MDD4All.DME.", StringComparison.Ordinal))
                {
                    result.Add(assembly);
                }
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
