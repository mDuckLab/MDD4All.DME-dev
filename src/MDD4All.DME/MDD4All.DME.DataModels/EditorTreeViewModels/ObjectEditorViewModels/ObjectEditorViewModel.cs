using CommunityToolkit.Mvvm.ComponentModel;
using MDD4All.DME.Analyzers;
using MDD4All.DME.ViewModels.EditorViewModels.Accesses;
using MDD4All.UI.DataModels.Tree;
using System;
using System.Collections.ObjectModel;

namespace MDD4All.DME.ViewModels
{
    public abstract class ObjectEditorViewModel : ObservableObject, ITreeNode
    {
        #region Constructors and Initialization
        public ObjectEditorViewModel(ITree tree, Access access, object? item, Type? targetType, string? title = "",
                                        ITreeNode? parent = null, TypeAnalyzer? preAnalyzedResult = null)
        {
            this.Access = access;
            this.Item = item;

            this.Tree = tree;
            this.Parent = parent;
            this.Children = new ObservableCollection<ITreeNode>();

            /* * NOTE: The following TypeAnalyzer initialization logic is technically redundant when 
             * this ViewModel is created via ReferenceEditorViewModel.CreateChildViewModel, 
             * as the factory already ensures a valid TypeAnalyzer is provided. 
   
             * It is kept here as a safety measure (fallback) to guarantee that the TypeAnalyzer 
             * is always correctly initialized, even if the constructor is called directly 
             * or the factory's pre-analysis result is missing.
            */

            if (preAnalyzedResult != null)
            {
                this.TypeAnalyzer = new TypeAnalyzer(preAnalyzedResult);
            }
            else
            {
                TypeAnalyzer = new TypeAnalyzer();
                //type Analyzer is set new when Type is setted 
                if (this.Item != null)
                {
                    this.TypeAnalyzer.Analyze(this.Item);
                }
                else if (targetType != null)
                {
                    this.TypeAnalyzer.Analyze(targetType);
                }
                else
                {
                    this.TypeAnalyzer.Analyze(typeof(object));
                }
            }

            if (!string.IsNullOrEmpty(title))
            {
                Title = title;
            }
        }

        public ObjectEditorViewModel(ITree tree, Access access, TypeAnalyzer preAnalyzedResult, string? title = null, ITreeNode? parent = null)
            : this(tree, access, null, null, title, parent, preAnalyzedResult) { }

        public ObjectEditorViewModel(ITree tree, Access access, object item, TypeAnalyzer preAnalyzedResult, string? title = null, ITreeNode? parent = null)
            : this(tree, access, item, null, title, parent, preAnalyzedResult) { }

        public ObjectEditorViewModel(ITree tree, Access access, object item, string? title = null, ITreeNode? parent = null)
            : this(tree, access, item, null, title, parent, null) { }

        public ObjectEditorViewModel(ITree tree, Access access, Type targetType, string? title = null, ITreeNode? parent = null)
            : this(tree, access, null, targetType, title, parent, null) { }
        #endregion

        #region properties
        public virtual object? Item { get; set; }

        public Type Type
        {
            get
            {
                Type result = typeof(object);
                if (TypeAnalyzer != null && TypeAnalyzer.AnalyzeType != null)
                {
                    result = TypeAnalyzer.AnalyzeType;
                }
                return result;
            }
            protected set
            {
                if (value != null)
                {
                    TypeAnalyzer.Analyze(value);
                }
                else
                {
                    TypeAnalyzer.Analyze(typeof(object));
                }
            }
        }

        public TypeCategory TypeCategory
        {
            get
            {
                return TypeAnalyzer.TypeCategory;
            }
        }

        public TypeAnalyzer TypeAnalyzer { get; set; } = null!;

        public Access Access { get; set; } = null!;

        private string? _title;

        public string Title
        {
            get
            {
                string result = string.Empty;
                if (!string.IsNullOrEmpty(_title))
                {
                    result = _title;
                }
                else
                {
                    result = DefaultTitle;
                }
                return result;
            }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        protected virtual string DefaultTitle
        {
            get
            {
                string result;

                if (Access is PropertyAccess propertyAccess)
                {
                    result = propertyAccess.PropertyInfo.Name;
                }
                else
                {
                    result = Type?.Name ?? "Object";
                }

                return result;
            }
        }

        public ITreeNode? Parent { get; set; }

        public ObservableCollection<ITreeNode> Children { get; set; } = null!;

        public bool StateChanged
        {
            set
            {
                if (value == true)
                {
                    OnPropertyChanged(nameof(StateChanged));
                }
            }
        }

        public ITree? Tree { get; set; }

        public int Index
        {
            get;
        }

        public bool HasChildNodes
        {
            get
            {
                return Children.Count > 0;
            }
        }

        public bool IsExpanded { get; set; }

        public bool IsSelected
        {
            get
            {
                bool result = false;
                if (Tree != null)
                {
                    result = Tree.SelectedNode == this;
                }
                return result;
            }
            set
            {
            }
        }

        public bool IsLoading
        {
            get
            {
                return false;
            }
        }

        public bool IsDisabled { get; set; } = false;

        public event EventHandler? TreeStateChanged;

        public string DragDropOperationInformation { get; set; } = string.Empty;
        #endregion
    }
}
