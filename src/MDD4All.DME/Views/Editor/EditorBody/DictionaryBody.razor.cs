using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.EditorViewModels;

namespace MDD4All.DME.Views.EditorView
{
    public partial class DictionaryBody : ComponentBase
    {
        [Parameter] public DictionaryEditorViewModel ViewModel { get; set; } = null!;
        [Parameter] public bool DeleteMode { get; set; } = false;
        [Parameter] public int MaxDepth { get; set; }
        [Parameter] public int CurrentDepth { get; set; }
    }
}