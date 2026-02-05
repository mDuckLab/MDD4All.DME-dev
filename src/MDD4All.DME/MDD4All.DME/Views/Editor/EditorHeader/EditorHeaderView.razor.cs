using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using MDD4All.DME.ViewModels; 
using MDD4All.DME.Views.EditorView; 

namespace MDD4All.DME.Views.EditorView
{
    public partial class EditorHeaderView : ComponentBase
    {
        [Parameter] public EditorState State { get; set; } = null!;
        [Parameter] public EventCallback<EditorAction> OnAction { get; set; }

        protected async Task Notify(EditorAction action)
        {
            await OnAction.InvokeAsync(action);
        }
    }
}