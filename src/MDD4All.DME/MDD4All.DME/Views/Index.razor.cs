using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using MDD4All.DME.ViewModels;
using MDD4All.UI.DataModels.Tree;
using System.ComponentModel;
using System.Collections.Generic;

namespace MDD4All.DME.Views
{
    public partial class Index : ComponentBase, System.IDisposable
    {
        [Inject]
        public MainViewModel MainViewModel { get; set; } = null!;

        private int _maxDepth = 5;

        private const int Limit = 9;

        private void IncreaseDepth()
        {
            if (_maxDepth == 0) _maxDepth = 2; // Von Alle zu 2
            else if (_maxDepth >= Limit) _maxDepth = 0; // Von 15 zu Alle
            else _maxDepth++;
        }

        private void DecreaseDepth()
        {
            if (_maxDepth == 0) _maxDepth = Limit; // Von Alle zu 15
            else if (_maxDepth <= 2) _maxDepth = 0; // Von 2 zu Alle
            else _maxDepth--;
        }

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


        private void OnTreeSelectionChange(ITreeNode node) { 
            if (this.MainViewModel.TreeViewModel != null)
            {
                this.MainViewModel.TreeViewModel.SelectedNode = node;
            }
        }
        #endregion
    }
}