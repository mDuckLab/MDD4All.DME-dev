using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using MDD4All.DME.ViewModels.Editor;

namespace MDD4All.DME.Views.Editor
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
            // Wir f�hren die Aktion nur aus, wenn das Objekt NICHT null ist
            if (!DataContext.IsNull)
            {
                await Notify(EditorAction.Select);
            }
        }
    }
}