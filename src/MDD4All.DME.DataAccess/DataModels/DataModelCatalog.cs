using System.Reflection;

namespace MDD4All.DME.DataAccess.DataModels
{
    // The data models the editor can work with: every public class in the assembly it is handed.
    //
    // On this branch the models are compiled into the solution rather than loaded from a DLL at
    // runtime, so there is no assembly to resolve, no load context to keep apart, and a type name
    // out of a file resolves with a plain Type.GetType. Which assembly counts is decided by
    // whoever constructs this - the application - not in here.
    public class DataModelCatalog
    {
        private readonly Assembly _modelAssembly;

        public DataModelCatalog(Assembly modelAssembly)
        {
            _modelAssembly = modelAssembly;
        }

        // The types a new object can actually be built from. Abstract classes and classes without
        // a parameterless constructor are left out rather than offered and then failing.
        public List<Type> AvailableTypes
        {
            get
            {
                List<Type> result = CollectTypes(onlyBuildable: true);

                return result;
            }
        }

        // Every public class in the assembly, buildable or not. For looking at what a model
        // actually contains - New will fail on some of these, and says so when it does.
        public List<Type> AllTypes
        {
            get
            {
                List<Type> result = CollectTypes(onlyBuildable: false);

                return result;
            }
        }

        public bool CanCreateInstance(Type type)
        {
            bool result = (!type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null);

            return result;
        }

        private List<Type> CollectTypes(bool onlyBuildable)
        {
            List<Type> result = new List<Type>();

            foreach (Type type in _modelAssembly.GetExportedTypes())
            {
                if (type.IsClass)
                {
                    if (!onlyBuildable || this.CanCreateInstance(type))
                    {
                        result.Add(type);
                    }
                }
            }

            result.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.Ordinal));

            return result;
        }

        // Turns the "$type" out of a file back into a type. The name is assembly-qualified
        // ("Namespace.Type, AssemblyName"), and only the part before the comma is used: the
        // assembly is known, and a file written by an older build may well name a different one.
        public Type? ResolveTypeName(string qualifiedTypeName)
        {
            Type? result = null;

            string typeName = qualifiedTypeName.Split(',')[0].Trim();

            if (typeName.Length > 0)
            {
                result = _modelAssembly.GetType(typeName);
            }

            return result;
        }
    }
}
