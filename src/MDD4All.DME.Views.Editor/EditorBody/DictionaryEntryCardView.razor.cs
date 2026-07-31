using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.Editor;
using MDD4All.DME.ViewModels.Editor.Settings;
using System.ComponentModel;

namespace MDD4All.DME.Views.Editor
{
    public partial class DictionaryEntryCardView : ComponentBase, System.IDisposable
    {
        [Parameter]
        public DictionaryEntryViewModel DataContext { get; set; } = null!;

        [Parameter] public int MaxDepth { get; set; }
        [Parameter] public int CurrentDepth { get; set; }

        [Inject]
        public EditorAppearanceSettingsViewModel Settings { get; set; } = null!;

        protected override void OnInitialized()
        {
            Settings.PropertyChanged += OnSettingsPropertyChanged;
        }

        public void Dispose()
        {
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
        }

        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        protected override void OnParametersSet()
        {
            // The nested ObjectEditorView only sets these on its own OnInitialized,
            // which runs after this header has already been rendered once - set them
            // here too so ShowExpander is correct from the very first render.
            if (DataContext.ValueEditor != null)
            {
                DataContext.ValueEditor.EditorState.CurrentDepth = CurrentDepth + 1;
                DataContext.ValueEditor.EditorState.MaxDepth = MaxDepth;
            }
        }

        private bool ShowExpander => DataContext.ValueEditor?.EditorState.ShowExpander ?? false;

        private bool IsExpanded => DataContext.ValueEditor?.EditorState.IsExpanded ?? false;

        private void OnDeleteEntry()
        {
            if (DataContext.DeleteCommand != null && DataContext.DeleteCommand.CanExecute(null))
            {
                DataContext.DeleteCommand.Execute(null);
            }
        }

        private void ToggleExpand()
        {
            if (DataContext.ValueEditor != null && ShowExpander)
            {
                DataContext.ValueEditor.EditorState.IsExpanded = !DataContext.ValueEditor.EditorState.IsExpanded;
            }
        }
    }
}
