using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.EditorViewModels;

namespace MDD4All.DME.Views.EditorView
{
    public partial class ComplexObjectEditorView : ComponentBase
    {
        [Parameter]
        public ComplexObjectEditorViewModel DataContext { get; set; } = null!;

        public bool IsExpanded { get; set; } = true;

        public bool ShowBadge { get; set; } = false;

        protected override void OnParametersSet()
        {
            if (DataContext != null)
            {
                if (DataContext.Item != null) IsExpanded = true;

                DataContext.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "StateChanged") StateHasChanged();
                };
            }
        }

        private void OnSelectNode()
        {
            if (this.DataContext.Tree != null)
            {
                this.DataContext.Tree.SelectedNode = this.DataContext;
            }
        }

        private void OnToggleExpanded()
        {
            this.IsExpanded = !this.IsExpanded;
        }

        private void OnCreateClick()
        {
            this.DataContext.CreateInstanceCommand.Execute(null);
        }

        private void OnDeleteClick()
        {
            this.DataContext.DeleteCommand.Execute(null);
        }
    }
}