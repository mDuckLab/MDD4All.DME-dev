using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.EditorViewModels;
using System.ComponentModel;
using System;

namespace MDD4All.DME.Views.EditorView
{
    public partial class DictionaryEditorView : ComponentBase, IDisposable
    {
        [Parameter]
        public DictionaryEditorViewModel DataContext { get; set; } = null!;

        public bool IsExpanded { get; set; } = true;

        #region Lifecycle
        protected override void OnInitialized()
        {
            if (DataContext != null)
            {
                // Wir lauschen auf Änderungen (z.B. wenn sich das Item-Objekt ändert)
                DataContext.PropertyChanged += OnViewModelPropertyChanged;
            }
        }

        public void Dispose()
        {
            if (DataContext != null)
            {
                DataContext.PropertyChanged -= OnViewModelPropertyChanged;
            }
        }
        #endregion

        #region Event Handlers
        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Bei StateChanged oder Title-Änderungen UI aktualisieren
            if (e.PropertyName == "StateChanged" || e.PropertyName == "Title")
            {
                InvokeAsync(StateHasChanged);
            }
        }

        private void OnSelectNode()
        {
            if (DataContext.Tree != null)
            {
                DataContext.Tree.SelectedNode = DataContext;
            }
        }

        private void OnToggleExpanded()
        {
            if (DataContext.Item != null)
            {
                IsExpanded = !IsExpanded;
            }
        }

        private void OnAddElementClick()
        {
            // Nutzt dein RelayCommand AddElementCommand
            DataContext.AddElementCommand.Execute(null);
        }

        private void OnCreateInstanceClick()
        {
            // Nutzt dein RelayCommand CreateInstanceCommand
            DataContext.CreateInstanceCommand.Execute(null);
        }

        private void OnDeleteDictionaryClick()
        {
            // Da DictionaryEditorViewModel von ReferenceEditorViewModel erbt,
            // nutzen wir das dort definierte DeleteCommand
            if (DataContext is ReferenceEditorViewModel refVm)
            {
                refVm.DeleteCommand.Execute(null);
            }
        }
        #endregion
    }
}