using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels;
using MDD4All.UI.DataModels.Tree;
using System.ComponentModel;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Threading.Tasks;


namespace MDD4All.DME.Views
{
    public partial class Index : ComponentBase, System.IDisposable
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

        [Inject]
        public MainViewModel MainViewModel { get; set; } = null!;

        private bool _showTreeIcons = true;
        private int _maxDepth = 5;

        #region Lifecycle
        protected override void OnInitialized()
        {
            if (this.MainViewModel != null)
            {
                this.MainViewModel.PropertyChanged += this.OnMainViewModelPropertyChanged;
            }
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