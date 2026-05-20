using MDD4All.DME.AssemblyTree.ViewModels;
using MDD4All.DME.DataAccess;
using MDD4All.DME.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace MDD4All.DME.Views.AssemblyTree
{
    public partial class TypeSelectionView
    {
        [Parameter]
        public MainViewModel DataContext { get; set; } = null!;

        private void OnSelectionDialogClose(bool args)
        {
            if (args == true)
            {
                AssemblyTreeViewModel? treeViewModel = DataContext.AssemblyTreeViewModel;

                if (treeViewModel != null)
                {
                    if (treeViewModel.SelectedNode is AssemblyElementNodeViewModel)
                    {
                        AssemblyElementNodeViewModel assemblyElementNode = (AssemblyElementNodeViewModel)treeViewModel.SelectedNode;

                        Configurations.DataModelDescriptor descriptor = new Configurations.DataModelDescriptor()
                        {
                            DllPath = assemblyElementNode.Path,
                            FullTypeName = assemblyElementNode.TypeNameWithNamespace
                        };

                        DataContext.ConfirmOpenDataModelCommand.Execute(descriptor);
                    }

                }
            }
        }
    }
}