namespace MDD4All.DME.Views.EditorView
{
    public class EditorState
    {
        public string Title { get; set; } = string.Empty;
        public string? BadgeText { get; set; }
        public bool IsNull { get; set; }

        public bool ShowCreateButton { get; set; }
        public bool ShowAddButton { get; set; }
        public bool ShowDeleteModeButton { get; set; }
        public bool ShowDeleteButton { get; set; }
        public bool ShowExpander { get; set; }
        public bool IsExpanded { get; set; } = true;
        public bool IsDeleteMode { get; set; } = false;

        public bool CanRenderChildren { get; set; } = true;
    }
}