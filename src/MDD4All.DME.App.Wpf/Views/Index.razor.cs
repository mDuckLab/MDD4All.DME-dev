using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.DataManager;
using MDD4All.Localization.Contracts;

namespace MDD4All.DME.App.Wpf
{
    public partial class Index
    {
        [Inject]
        public MainViewModel DataContext { get; set; } = null!;

        [Inject]
        public ILanguageSetter LanguageSetter { get; set; } = null!;

        protected override void OnInitialized()
        {
            DataContext.PropertyChanged += OnPropertyChanged;

            LanguageSetter.CultureChanged += OnCultureChanged;
        }

        // The root itself shows no text, but it holds the switch statement that picks the
        // screen - so it has to be redrawn along with everything under it.
        private void OnCultureChanged(object? sender, System.EventArgs e)
        {
            InvokeAsync(StateHasChanged);
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