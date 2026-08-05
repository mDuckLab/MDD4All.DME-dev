using MDD4All.DME.ViewModels.DataManager;
using Microsoft.AspNetCore.Components;

namespace MDD4All.DME.Views.StartPage
{
    public partial class StartPageView
    {
        [Inject]
        public DataFileManagerViewModel DataContext { get; set; } = null!;

        [Parameter]
        public EventCallback OnSettingsRequested { get; set; }
    }
}