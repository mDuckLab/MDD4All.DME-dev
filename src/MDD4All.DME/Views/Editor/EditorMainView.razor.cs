using MDD4All.DME.ViewModels;
using MDD4All.DME.ViewModels.DataManager;
using MDD4All.DME.ViewModels.Editor.Settings;
using MDD4All.Localization.Contracts;
using MDD4All.UI.DataModels.Tree;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MDD4All.DME.Views.Editor
{
    public partial class EditorMainView
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;


        [Inject]
        public MainViewModel MainViewModel { get; set; } = null!;

        [Inject]
        public DataFileManagerViewModel DataFileManager { get; set; } = null!;

        [Inject]
        public ILanguageSetter LanguageSetter { get; set; } = null!;

        [Inject]
        public EditorAppearanceSettingsViewModel EditorSettings { get; set; } = null!;

        [Inject]
        public ExplorerSettingsViewModel ExplorerSettings { get; set; } = null!;

        private bool _showSettings = false;

        #region Lifecycle
        protected override void OnInitialized()
        {
            if (this.MainViewModel != null)
            {
                this.MainViewModel.PropertyChanged += this.OnMainViewModelPropertyChanged;
            }
            LanguageSetter.CultureChanged += OnCultureChanged;
            EditorSettings.PropertyChanged += OnEditorSettingsPropertyChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ApplyTintIntensity();
            }
        }

        private void OnCultureChanged(object? sender, System.EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (this.MainViewModel != null)
            {
                this.MainViewModel.PropertyChanged -= this.OnMainViewModelPropertyChanged;
            }
            EditorSettings.PropertyChanged -= OnEditorSettingsPropertyChanged;
        }
        #endregion

        #region Event Handlers
        private void OnMainViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            this.InvokeAsync(this.StateHasChanged);
        }

        private void OnTreeSelectionChange(ITreeNode node)
        {
            if (this.MainViewModel.TreeViewModel != null)
            {
                this.MainViewModel.TreeViewModel.SelectedNode = node;
            }
        }

        private async Task StartResizing(MouseEventArgs e)
        {
            await JSRuntime.InvokeVoidAsync("initResizer", "workbench-container");
        }

        private async void OnEditorSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditorAppearanceSettingsViewModel.TintEnabled))
            {
                await ApplyTintIntensity();
            }
        }

        private async Task ApplyTintIntensity()
        {
            string intensity = EditorSettings.TintEnabled ? "6%" : "0%";
            await JSRuntime.InvokeVoidAsync("eval",
                $"document.documentElement.style.setProperty('--tint-intensity', '{intensity}')");
        }

        private void OnSettingsClose(bool confirmed)
        {
            _showSettings = false;
        }
        #endregion
    

    }
}