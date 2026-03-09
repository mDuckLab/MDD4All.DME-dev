using Microsoft.AspNetCore.Components;
using MDD4All.DME.ViewModels;
using MDD4All.DME.ViewModels.EditorViewModels;
using MDD4All.DME.ViewModels.EditorViewModels.Accesses;
using MDD4All.DME.Analyzers;

namespace MDD4All.DME.Views.EditorView
{
    public partial class ObjectEditorView : ComponentBase
    {
        [Parameter] public ObjectEditorViewModel ViewModel { get; set; } = null!;
        [Parameter] public int MaxDepth { get; set; } = 0;
        [Parameter] public int CurrentDepth { get; set; } = 1;

        private EditorState _state = new EditorState();

        private bool IsReferenceType
        {
            get
            {
                bool result = false;
                if (this.ViewModel is ReferenceEditorViewModel)
                {
                    result = true;
                }
                return result;
            }
        }

        protected override void OnParametersSet()
        {
            if (this.ViewModel != null)
            {
                this.InitializeState();
            }
        }

        private void InitializeState()
        {
            // Set basic info
            this._state.Title = this.ViewModel.Title;
            this._state.IsNull = this.ViewModel.IsNull;

            // Handle BadgeText for reference types
            if (this.ViewModel is ReferenceEditorViewModel referenceEditorViewModel)
            {
                this._state.BadgeText = referenceEditorViewModel.BadgeText;
            }

            // Check depth limit explicitly
            if (this.MaxDepth == 0 || this.CurrentDepth < this.MaxDepth)
            {
                this._state.CanRenderChildren = true;
            }
            else
            {
                this._state.CanRenderChildren = false;
            }

            // Logic for reference types within depth limit
            if (this._state.CanRenderChildren == true && this.ViewModel is ReferenceEditorViewModel referenceEditor)
            {
                if (this.ViewModel.Item == null)
                {
                    // Item is deleted -> Reset UI and show Create button
                    this.ResetStateForLimitOrSimpleType();
                    this._state.ShowCreateButton = true;
                }
                else
                {
                    this._state.ShowCreateButton = false;
                    this._state.ShowExpander = this.ViewModel.HasChildNodes;
                    this._state.ShowDeleteButton = this.ViewModel.Parent != null;

                    // Use Enum-Category to determine button visibility
                    switch (this.ViewModel.TypeCategory)
                    {
                        case TypeCategory.IList:
                        case TypeCategory.Array:
                            if (referenceEditor is IndexedCollectionEditorViewModel indexedCollectionEditorViewModel)
                            {
                                this._state.ShowAddButton = true;
                                if (indexedCollectionEditorViewModel.IsUnderlyingTypeSimple == true)
                                {
                                    this._state.ShowDeleteModeButton = true;
                                }
                            }
                            break;

                        case TypeCategory.IDictionary:
                            this._state.ShowAddButton = true;
                            this._state.ShowDeleteModeButton = true;
                            break;
                    }
                }
            }
            else
            {
                // Fallback for simple types or depth limit
                this.ResetStateForLimitOrSimpleType();
            }
        }

        private void ResetStateForLimitOrSimpleType()
        {
            this._state.ShowExpander = false;
            this._state.ShowAddButton = false;
            this._state.ShowDeleteButton = false;
            this._state.ShowCreateButton = false;
            this._state.ShowDeleteModeButton = false;

            if (this._state.CanRenderChildren == false || this.ViewModel.Item == null)
            {
                this._state.IsExpanded = false;
            }
        }

        private void HandleAction(EditorAction action)
        {
            if (action == EditorAction.ToggleExpand)
            {
                this._state.IsExpanded = !this._state.IsExpanded;
            }
            else if (action == EditorAction.ToggleDeleteMode)
            {
                this._state.IsDeleteMode = !this._state.IsDeleteMode;
            }
            else if (action == EditorAction.Select)
            {
                // Select node in tree if available
                if (this.ViewModel.Tree != null)
                {
                    this.ViewModel.Tree.SelectedNode = this.ViewModel;
                }
            }
            else
            {
                // Execute data commands (Create, Add, Delete)
                this.ExecuteViewModelCommand(action);

                // Refresh UI state to handle collapse and button visibility
                this.InitializeState();

                // AUTO-EXPAND Logic:
                // Automatically expand the card after creating an instance or adding an element.
                // This only triggers if we are within the allowed depth limits.
                if ((action == EditorAction.Create || action == EditorAction.Add) && this._state.CanRenderChildren == true)
                {
                    this._state.IsExpanded = true;
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
                                if (action == EditorAction.Create) collection.CreateInstanceCommand.Execute(null);
                                else if (action == EditorAction.Add) collection.AddElementCommand.Execute(null);
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