using MDD4All.DME.ViewModels.DataManager;
using Microsoft.AspNetCore.Components;

namespace MDD4All.DME.Views.DataManager
{
    public partial class StartPageView
    {
        [Inject]
        public DataFileManagerViewModel DataContext { get; set; } = null!;

        [Parameter]
        public EventCallback OnSettingsRequested { get; set; }

        //private void OnSelectDataModel()
        //{
        //    SynchronizationContext.Current?.Post((_) =>
        //    {
        //        DataContext.OpenDataModelCommand.Execute(null);
        //        StateHasChanged();
        //    }, null);

        //}

        //private void OnClickRecentDataModelLink(int index)
        //{
        //    DataContext.SetDataModelFromRecentListCommand.Execute(index);
        //}

        //private void OnCreateNewFileClick()
        //{
        //    SynchronizationContext.Current?.Post((_) =>
        //    {
        //        DataContext.NewDataFileCommand.Execute(null);
        //        StateHasChanged();
        //    }, null);
        //}

        //private void OnClickRecentDataFileLink(int index)
        //{
        //    DataContext.OpenRecentDataFileCommand.Execute(index);
        //}
    }
}