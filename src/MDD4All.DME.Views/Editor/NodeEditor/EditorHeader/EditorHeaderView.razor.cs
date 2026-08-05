using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Threading.Tasks;
using MDD4All.DME.ViewModels.Editor;
using MDD4All.DME.ViewModels.Editor.Settings;

namespace MDD4All.DME.Views.Editor
{
    public partial class EditorHeaderView : ComponentBase, IDisposable
    {
        [Parameter]
        public EventCallback<EditorAction> OnAction { get; set; }

        [Parameter]
        public ObjectEditorViewModel DataContext { get; set; } = null!;

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

        protected async Task Notify(EditorAction action)
        {
            await OnAction.InvokeAsync(action);
        }

        private async Task OnSelectLabel()
        {
            // Wir f�hren die Aktion nur aus, wenn das Objekt NICHT null ist
            if (!DataContext.IsNull)
            {
                await Notify(EditorAction.Select);
            }
        }
    }
}