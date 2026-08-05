using MDD4All.DME.AssemblyTree.ViewModels;
using MDD4All.DME.ViewModels.DataManager;
using Microsoft.AspNetCore.Components;

namespace MDD4All.DME.Views.StartPage
{
    public partial class TypeSelectionView
    {
        [Inject]
        public DataFileManagerViewModel DataContext { get; set; } = null!;

        private void OnSelectionDialogClose(bool args)
        {
            Configurations.DataModelDescriptor? descriptor = null;

            if (args == true)
            {
                AssemblyTreeViewModel? treeViewModel = DataContext.AssemblyTreeViewModel;

                if (treeViewModel != null)
                {
                    if (treeViewModel.SelectedNode is AssemblyElementNodeViewModel)
                    {
                        AssemblyElementNodeViewModel assemblyElementNode = (AssemblyElementNodeViewModel)treeViewModel.SelectedNode;

                        descriptor = new Configurations.DataModelDescriptor()
                        {
                            DllPath = assemblyElementNode.Path,
                            FullTypeName = assemblyElementNode.TypeNameWithNamespace
                        };

                        
                    }

                }
            }

            DataContext.ConfirmOpenDataModelCommand.Execute(descriptor);
        }
    }
}