using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.EditorViewModels;
using MDD4All.DME.ViewModels;
using MDD4All.DME.ViewModels.EditorViewModels.Accesses;
using MDD4All.UI.DataModels.Tree;
using System.ComponentModel;

namespace MDD4All.DME.Views.EditorView
{
    public partial class IndexedCollectionEditorView : ComponentBase
    {
        [Parameter]
        public IndexedCollectionEditorViewModel DataContext { get; set; } = null!;

        // Lokaler Zustand für die UI-Steuerung
        public bool IsExpanded { get; set; } = true;
        public bool DeleteMode { get; set; } = false;

        protected override void OnParametersSet()
        {
            if (this.DataContext != null && this.DataContext is INotifyPropertyChanged notify)
            {
                // UI aktualisieren, wenn das ViewModel "StateChanged" meldet
                notify.PropertyChanged += (sender, eventArgs) =>
                {
                    if (eventArgs.PropertyName == "StateChanged")
                    {
                        this.InvokeAsync(this.StateHasChanged);
                    }
                };
            }
        }

        #region Header Event Handlers

        private void OnToggleExpanded()
        {
            this.IsExpanded = !this.IsExpanded;
        }

        private void OnSelectNode()
        {
            if (this.DataContext.Tree != null)
            {
                this.DataContext.Tree.SelectedNode = (ITreeNode)this.DataContext;
            }
        }

        private void OnToggleDeleteMode()
        {
            this.DeleteMode = !this.DeleteMode;
        }

        private void OnAddClick()
        {
            if (this.DataContext.AddElementCommand.CanExecute(null))
            {
                this.DataContext.AddElementCommand.Execute(null);
            }
        }

        private void OnCreateClick()
        {
            if (this.DataContext.CreateInstanceCommand.CanExecute(null))
            {
                this.DataContext.CreateInstanceCommand.Execute(null);
            }
        }

        private void OnDeleteItemClick()
        {
            // Da IndexedCollectionEditorViewModel von ReferenceEditorViewModel erbt
            if (this.DataContext is ReferenceEditorViewModel referenceViewModel)
            {
                if (referenceViewModel.DeleteCommand.CanExecute(null))
                {
                    referenceViewModel.DeleteCommand.Execute(null);
                }
            }
        }

        #endregion

        #region Body Logic

        // Methode für das Löschen einzelner Kacheln (Primitive Typen)
        private void OnSimpleDeleteChildClick(ObjectEditorViewModel childViewModel)
        {
            int index = -1;

            if (childViewModel.Access is ListAccess listAccess)
            {
                index = listAccess.Index;
            }
            else if (childViewModel.Access is ArrayAccess arrayAccess)
            {
                index = arrayAccess.Index;
            }

            if (index != -1 && this.DataContext.DeleteAtIndexCommand.CanExecute(index))
            {
                this.DataContext.DeleteAtIndexCommand.Execute(index);
            }
        }

        #endregion
    }
}