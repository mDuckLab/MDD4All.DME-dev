using MDD4All.DME.ViewModels.Editor.Settings;
using Microsoft.AspNetCore.Components;

namespace MDD4All.DME.Views.SettingsDialog
{
    public partial class SettingsDialog
    {
        [Inject]
        public EditorAppearanceSettingsViewModel EditorSettings { get; set; } = null!;

        [Inject]
        public ExplorerSettingsViewModel ExplorerSettings { get; set; } = null!;

        [Parameter]
        public EventCallback<bool> OnClose { get; set; }

        private SettingsCategory ActiveCategory { get; set; } = SettingsCategory.Global;

        private enum SettingsCategory
        {
            Global,
            Editor,
            Explorer
        }
    }
}
