using Microsoft.AspNetCore.Components;
using MDD4All.UI.DataModels.Tree;
using MDD4All.DME.ViewModels;

namespace MDD4All.DME.Views.EditorView
{
    public partial class BreadcrumbNodeView : ComponentBase
    {
        public BreadcrumbNodeView() { }

        [Parameter]
        public ObjectEditorViewModel TreeNode { get; set; } = null!;

        [Parameter]
        public bool IsLast { get; set; }

        private void OnSelectNode()
        {
            if (this.TreeNode != null && this.TreeNode.Tree != null)
            {
                this.TreeNode.Tree.SelectedNode = this.TreeNode;
            }
        }
    }
}