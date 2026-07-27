using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels;

namespace MDD4All.DME.Views
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
    }
}