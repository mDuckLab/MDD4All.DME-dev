using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels.Editor;
using MDD4All.Reflection;

namespace MDD4All.DME.Views.Editor
{
    public partial class ObjectEditorView : ComponentBase
    {
        [Parameter] 
        public ObjectEditorViewModel ViewModel { get; set; } = null!;
        
        [Parameter] 
        public int MaxDepth { get; set; } = 0;
        
        [Parameter] 
        public int CurrentDepth { get; set; } = 1;

        private bool IsReferenceType
        {
            get
            {
                bool result = false;
                if (ViewModel is ReferenceEditorViewModel)
                {
                    result = true;
                }
                return result;
            }
        }

        private string CssBackgroundExtension
        {
            get
            {
                string result = "";
                if(ViewModel is ListEditorViewModel)
                {
                    result = "background-color:#EFF9EB;";
                }
                return result;
            }
        }

        protected override void OnInitialized()
        {
            ViewModel.EditorState.CurrentDepth = CurrentDepth;
            ViewModel.EditorState.MaxDepth = MaxDepth;
        }

        

        private void HandleAction(EditorAction action)
        {
            if (action == EditorAction.ToggleExpand)
            {
                ViewModel.EditorState.IsExpanded = !ViewModel.EditorState.IsExpanded;
            }
            else if (action == EditorAction.ToggleDeleteMode)
            {
                ViewModel.EditorState.IsDeleteMode = !ViewModel.EditorState.IsDeleteMode;
            }
            else if (action == EditorAction.Select)
            {
                // Select node in tree if available
                if (ViewModel.Tree != null)
                {
                    ViewModel.Tree.SelectedNode = ViewModel;
                }
            }
            else
            {
                // Execute data commands (Create, Add, Delete)
                ExecuteViewModelCommand(action);

                // Refresh UI state to handle collapse and button visibility
               // this.InitializeState();

                // AUTO-EXPAND Logic:
                // Automatically expand the card after creating an instance or adding an element.
                // This only triggers if we are within the allowed depth limits.
                if ((action == EditorAction.Create || action == EditorAction.Add) && ViewModel.EditorState.CanRenderChildren == true)
                {
                    //ViewModel.EditorState. IsExpanded = true;
                }
            }
        }

        private void ExecuteViewModelCommand(EditorAction action)
        {
            if (this.ViewModel is ReferenceEditorViewModel referenceEditorViewModel)
            {
                if (action == EditorAction.Delete)
                {
                    referenceEditorViewModel.DeleteCommand.Execute(null);
                }
                else
                {
                    switch (this.ViewModel.TypeCategory)
                    {
                        case TypeCategory.None:
                            if (referenceEditorViewModel is ComplexObjectEditorViewModel complex && action == EditorAction.Create)
                            {
                                complex.CreateInstanceCommand.Execute(null);
                            }
                            break;

                        case TypeCategory.IList:
                        case TypeCategory.Array:
                            if (referenceEditorViewModel is IndexedCollectionEditorViewModel collection)
                            {
                                if (action == EditorAction.Create)
                                {
                                    collection.CreateInstanceCommand.Execute(null);
                                }
                                else if (action == EditorAction.Add)
                                {
                                    collection.AddElementCommand.Execute(null);
                                    
                                }
                            }
                            break;

                        case TypeCategory.IDictionary:
                            if (referenceEditorViewModel is DictionaryEditorViewModel dictionary)
                            {
                                if (action == EditorAction.Create) dictionary.CreateInstanceCommand.Execute(null);
                                else if (action == EditorAction.Add) dictionary.AddElementCommand.Execute(null);
                            }
                            break;
                    }
                }
            }
        }
    }
}