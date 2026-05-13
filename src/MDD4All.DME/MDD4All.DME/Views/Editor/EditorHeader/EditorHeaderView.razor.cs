using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using MDD4All.DME.ViewModels;
using MDD4All.DME.ViewModels.Editor.EditorTreeViewModels.ObjectEditorViewModels;

namespace MDD4All.DME.Views.EditorView
{
    public partial class EditorHeaderView : ComponentBase
    {        
        [Parameter] 
        public EventCallback<EditorAction> OnAction { get; set; }

        [Parameter]
        public ObjectEditorViewModel DataContext { get; set; } = null!;

        protected async Task Notify(EditorAction action)
        {
            await OnAction.InvokeAsync(action);
        }

        private async Task OnSelectLabel()
        {
            // Wir führen die Aktion nur aus, wenn das Objekt NICHT null ist
            if (!DataContext.IsNull)
            {
                await Notify(EditorAction.Select);
            }
        }
    }
}