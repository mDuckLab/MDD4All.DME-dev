using MDD4All.DME.ViewModels.DataManager;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace MDD4All.DME.Views.DataManager
{
    public partial class RawDataView
    {
        [Inject] private IJSRuntime _js { get; set; } = null!;

        [Parameter]
        public DataEditorViewModel DataContext { get; set; } = null!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await _js.InvokeVoidAsync("highlightSnippet");
        }
    }
}