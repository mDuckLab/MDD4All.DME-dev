using MDD4All.DME.ViewModels.DataManager;
using Microsoft.AspNetCore.Components;

namespace MDD4All.DME.Views.Dialogs
{
    public partial class CultureChangeDialog
    {
        [Parameter]
        public MainViewModel DataContext { get; set; } = null!;

        public void OnDialogClose()
        {
            DataContext.ActiveOverlay = EOverlayState.None;
        }

    }
}