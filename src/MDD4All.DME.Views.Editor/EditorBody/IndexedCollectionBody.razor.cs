using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.Editor;
using MDD4All.ObjectGraph.Access;

namespace MDD4All.DME.Views.Editor
{
    public partial class IndexedCollectionBody : ComponentBase
    {
        [Parameter] 
        public IndexedCollectionEditorViewModel ViewModel { get; set; } = null!;

        [Parameter] 
        public int MaxDepth { get; set; }

        [Parameter]
        public int CurrentDepth { get; set; }

        [Parameter] public bool IsCompact { get; set; } = false;

        private void OnDeleteChild(ObjectEditorViewModel childVm)
        {
            int index = -1;

            if (childVm.Access is ListAccess listAccess)
            {
                index = listAccess.Index;
            }
            else if (childVm.Access is ArrayAccess arrayAccess)
            {
                index = arrayAccess.Index;
            }

            // Wenn ein g�ltiger Index gefunden wurde, den L�schbefehl ausf�hren
            if (index != -1 && ViewModel.DeleteAtIndexCommand.CanExecute(index))
            {
                ViewModel.DeleteAtIndexCommand.Execute(index);
            }
        }
    }
}