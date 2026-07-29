using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.Editor;

namespace MDD4All.DME.Views.Editor
{
    public partial class DictionaryEntryView : ComponentBase
    {
        [Parameter]
        public DictionaryEntryViewModel DataContext { get; set; } = null!;

        [Parameter]
        public bool DeleteMode { get; set; } = false;

        [Parameter] public int MaxDepth { get; set; }
        [Parameter] public int CurrentDepth { get; set; }

        /// <summary>
        /// F�hrt den L�schbefehl f�r diesen spezifischen Dictionary-Eintrag aus.
        /// </summary>
        private void OnDeleteEntry()
        {
            if (DataContext.DeleteCommand != null && DataContext.DeleteCommand.CanExecute(null))
            {
                DataContext.DeleteCommand.Execute(null);
            }
        }
    }
}