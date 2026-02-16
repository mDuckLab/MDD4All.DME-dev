using System.Reflection;

namespace MDD4All.DME.ViewModels.EditorViewModels.Accesses
{
    public class PropertyAccess : Access
    {
        public PropertyAccess(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
        }
        public PropertyInfo PropertyInfo { get; private set; }
    }
}
