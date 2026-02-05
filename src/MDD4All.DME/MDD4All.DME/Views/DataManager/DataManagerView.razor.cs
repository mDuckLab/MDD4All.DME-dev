using MDD4All.DME.Services.Save_Load_Services.SaveServices.Interface;
using MDD4All.DME.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace MDD4All.DME.Views
{
    public partial class DataManagerView
    {
        [Parameter]
        public DataManagerViewModel ViewModel { get; set; } = null!;

        [Inject]
        public IFileImportService FileImportService { get; set; } = null!;

        private void OnNewClick()
        {
            ViewModel.CreateNew();
        }

        private async Task OnSaveClick()
        {
            await ViewModel.Export();
        }

        // Triggers the process to open the browser's file upload dialog.
        private async Task OnImportClick()
        {
            await FileImportService.OpenImportDialogAsync("importInput");
        }

        //Handles the file selection event and triggers the import logic in the ViewModel.
        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            await ViewModel.Import(e.File);
        }
    }
}