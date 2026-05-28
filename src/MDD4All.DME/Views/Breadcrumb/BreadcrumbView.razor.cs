using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MDD4All.UI.DataModels.Tree;

namespace MDD4All.DME.Views.EditorView
{
    public partial class BreadcrumbView : ComponentBase
    {
        #region Constructors
        public BreadcrumbView()
        {
        }
        #endregion

        #region Properties
        [Parameter]
        public List<ITreeNode>? Path { get; set; }
        #endregion

        #region Protected Methods
        protected List<ITreeNode> GetSafePath()
        {
            List<ITreeNode> result = new List<ITreeNode>();

            if (this.Path != null)
            {
                result = this.Path;
            }

            return result;
        }
        #endregion
    }
}