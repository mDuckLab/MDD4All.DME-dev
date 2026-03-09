using MDD4All.DME.Services.Save_Load_Services.SaveServices.Interface;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace MDD4All.DME.Services
{
    public class BlazorWebFileImportService : IFileImportService
    {
        private readonly IJSRuntime _jsRuntime;

        public BlazorWebFileImportService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task OpenImportDialogAsync(string elementId)
        {
            await _jsRuntime.InvokeVoidAsync("eval", $"document.getElementById('{elementId}').click()");
        }
    }
}