using MDD4All.DME.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Threading;

namespace MDD4All.DME.Views.Editor
{
    public partial class StartPageView
    {
        [Parameter]
        public MainViewModel DataContext { get; set; } = null!;

        private void OnSelectDataModel()
        {
            SynchronizationContext.Current?.Post((_) =>
            {
                DataContext.OpenDataModelCommand.Execute(null);
                StateHasChanged();
            }, null);

        }
    }
}