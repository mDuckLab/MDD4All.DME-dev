using MDD4All.DME.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MDD4All.DME.Views.Tree
{
    public partial class EditorTreeIcon
    {
        [Parameter]
        public ObjectEditorViewModel DataContext { get; set; } = null!;
    }
}