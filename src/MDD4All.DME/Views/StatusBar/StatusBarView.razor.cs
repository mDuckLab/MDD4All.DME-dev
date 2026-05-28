using MDD4All.DME.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MDD4All.DME.Views.StatusBar
{
    public partial class StatusBarView
    {
        [Parameter]
        public MainViewModel DataContext { get; set; } = null!;
    }
}