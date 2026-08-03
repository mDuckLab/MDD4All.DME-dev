using MDD4All.DME.ViewModels;
using MDD4All.DME.ViewModels.DataManager;
using Microsoft.AspNetCore.Components;

namespace MDD4All.DME.Views.Editor
{
    public partial class EditorMainToolbar
    {
        [Parameter]
        public MainViewModel DataContext { get; set; } = null!;

        [Inject]
        public DataFileManagerViewModel DataFileManager { get; set; } = null!;
    }
}