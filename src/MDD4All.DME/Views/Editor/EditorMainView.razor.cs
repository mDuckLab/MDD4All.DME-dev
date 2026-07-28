using MDD4All.DME.ViewModels;
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
        public ILanguageSetter LanguageSetter { get; set; } = null!;

        private int _maxDepth = 5;

        #region Lifecycle
        protected override void OnInitialized()
        {
            if (this.MainViewModel != null)
            {
                this.MainViewModel.PropertyChanged += this.OnMainViewModelPropertyChanged;
            }
            LanguageSetter.CultureChanged += OnCultureChanged;
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
        #endregion
    

    }
}