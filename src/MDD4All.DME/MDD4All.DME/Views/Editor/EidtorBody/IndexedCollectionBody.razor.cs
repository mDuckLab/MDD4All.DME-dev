using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.EditorViewModels;
using MDD4All.DME.ViewModels.EditorViewModels.Accesses;
using MDD4All.DME.ViewModels;

namespace MDD4All.DME.Views.EditorView
{
    public partial class IndexedCollectionBody : ComponentBase
    {
        [Parameter] 
        public IndexedCollectionEditorViewModel ViewModel { get; set; } = null!;

        [Parameter] 
        public EditorState State { get; set; } = null!;

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

            // Wenn ein gültiger Index gefunden wurde, den Löschbefehl ausführen
            if (index != -1 && ViewModel.DeleteAtIndexCommand.CanExecute(index))
            {
                ViewModel.DeleteAtIndexCommand.Execute(index);
            }
        }
    }
}