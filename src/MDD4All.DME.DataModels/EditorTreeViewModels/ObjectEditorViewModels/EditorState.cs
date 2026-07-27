using MDD4All.DME.ViewModels.EditorViewModels;

namespace MDD4All.DME.ViewModels.Editor.EditorTreeViewModels.ObjectEditorViewModels
{
    public class EditorState
    {
        private ObjectEditorViewModel _viewModel;

        public EditorState(ObjectEditorViewModel editorViewModel)
        {
            _viewModel = editorViewModel;
        }

        public int MaxDepth { get; set; } = 0;

        public int CurrentDepth { get; set; } = 1;

        public string? BadgeText { get; set; }

        public bool ShowCreateButton
        {
            get
            {
                bool result = false;

                if (_viewModel.Item == null)
                {
                    result = true;
                }

                return result;
            }
        }

        public bool ShowAddButton
        {
            get
            {
                bool result = false;

                if (_viewModel.TypeCategory == Analyzers.TypeCategory.IList || _viewModel.TypeCategory == Analyzers.TypeCategory.Array)
                {
                    if (_viewModel is IndexedCollectionEditorViewModel)
                    {
                        result = true;
                    }
                }
                else if (_viewModel.TypeCategory == Analyzers.TypeCategory.IDictionary)
                {
                    result = true;
                }

                return result;
            }
        }

        public bool ShowDeleteModeButton
        {
            get
            {
                bool result = false;

                if (_viewModel.TypeCategory == Analyzers.TypeCategory.IList || _viewModel.TypeCategory == Analyzers.TypeCategory.Array)
                {
                    if (_viewModel is IndexedCollectionEditorViewModel)
                    {
                        IndexedCollectionEditorViewModel indexedCollectionEditorViewModel = (IndexedCollectionEditorViewModel)_viewModel;
                        if (indexedCollectionEditorViewModel.IsUnderlyingTypeSimple)
                        {
                            result = true;
                        }
                    }
                }
                else if (_viewModel.TypeCategory == Analyzers.TypeCategory.IDictionary)
                {
                    result = true;
                }

                return result;
            }

        }

        public bool ShowDeleteButton
        {
            get
            {
                bool result = false;

                if (_viewModel.Parent != null)
                {
                    result = true;
                }

                return result;
            }
        }

        public bool ShowExpander
        {
            get
            {
                bool result = false;

                if (_viewModel.HasChildNodes)
                {
                    result = true;
                }

                return result;
            }
        }

        public bool IsExpanded { get; set; } = false;

        public bool IsDeleteMode { get; set; } = false;



        public bool CanRenderChildren
        {
            get
            {
                bool result = false;

                // Check depth limit explicitly
                if (MaxDepth == 0 || CurrentDepth < MaxDepth)
                {
                    result = true;
                }

                return result;
            }
        }


    }
}