using MDD4All.DME.ViewModels.DataManager;
using MDD4All.DME.ViewModels.Editor;
using Microsoft.AspNetCore.Components;

namespace MDD4All.DME.Views.Editor
{
    public partial class EditorMainToolbar
    {
        [Inject]
        public INavigation Navigation { get; set; } = null!;

        [Inject]
        public IEditorState EditorState { get; set; } = null!;

        [Inject]
        public DataFileManagerViewModel DataFileManager { get; set; } = null!;
    }
}
