using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.DataManager;

namespace MDD4All.DME.App.Wpf
{
    public partial class Index
    {
        [Inject]
        public MainViewModel DataContext { get; set; } = null!;

        protected override void OnInitialized()
        {
            DataContext.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }

        private void OnSettingsClose(bool confirmed)
        {
            DataContext.ActiveOverlay = OverlayState.None;
        }
    }
}