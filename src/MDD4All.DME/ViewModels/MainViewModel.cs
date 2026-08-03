using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MDD4All.DME.ViewModels.DataManager;
using MDD4All.DME.ViewModels.Editor;
using MDD4All.Localization.Contracts;
using MDD4All.UI.DataModels.Tree;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace MDD4All.DME.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ObjectTreeViewModel? _treeViewModel;

        private ILanguageSetter _languageSetter;

        private DataFileManagerViewModel _dataFileManager;

        public MainViewModel(ILanguageSetter languageSetter, DataFileManagerViewModel dataFileManager)
        {
            _languageSetter = languageSetter;
            _languageSetter.CultureChanged += OnCultureChanged;

            _dataFileManager = dataFileManager;
            _dataFileManager.PropertyChanged += OnDataFileManagerPropertyChanged;

            InitializeCommands();
        }

        private void OnCultureChanged(object? sender, EventArgs e)
        {
            ViewState = EViewState.NewCultureRequested;
        }

        private void OnDataFileManagerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataFileManagerViewModel.DataEditorViewModel))
            {
                RebuildTree();
                ViewState = EViewState.ShowEditor;
            }
            else if (e.PropertyName == nameof(DataFileManagerViewModel.AssemblyTreeViewModel))
            {
                ViewState = _dataFileManager.AssemblyTreeViewModel != null
                    ? EViewState.ShowTypeSelectionView
                    : EViewState.ShowStartPage;
            }
        }

        private void InitializeCommands()
        {
            ShowStartPageCommand = new RelayCommand(ExecuteShowStartPage);
        }

        public ObjectTreeViewModel? TreeViewModel
        {
            get
            {
                return _treeViewModel;
            }
            private set
            {
                if (_treeViewModel != value)
                {
                    _treeViewModel = value;
                    OnPropertyChanged(nameof(TreeViewModel));
                    OnPropertyChanged(nameof(SelectedEditorViewModel));
                }
            }
        }

        private EViewState _viewState = EViewState.ShowStartPage;

        public EViewState ViewState
        {
            get { return _viewState; }

            set
            {
                _viewState = value;
                OnPropertyChanged(nameof(ViewState));
            }
        }

        public ITreeNode? SelectedEditorViewModel
        {
            get
            {
                ITreeNode? result = null;
                if (TreeViewModel != null)
                {
                    if (TreeViewModel.SelectedNode is ObjectEditorViewModel)
                    {
                        ObjectEditorViewModel objectEditorViewModel = (ObjectEditorViewModel)TreeViewModel.SelectedNode;
                        objectEditorViewModel.EditorState.IsExpanded = true;
                    }
                    result = TreeViewModel.SelectedNode;
                }
                return result;
            }
        }

        private bool _showRawData = false;

        public bool ShowRawData
        {
            get
            {
                return _showRawData;
            }

            set
            {
                _showRawData = value;
                OnPropertyChanged(nameof(ShowRawData));
            }
        }

        public ICommand ShowStartPageCommand { get; private set; } = null!;

        private void RebuildTree()
        {
            if (this.TreeViewModel != null)
            {
                this.TreeViewModel.PropertyChanged -= this.OnTreePropertyChanged;
            }

            object? activeObject = _dataFileManager.DataEditorViewModel?.ActiveObject;
            Type? selectedType = _dataFileManager.DataEditorViewModel?.SelectedType;

            if (activeObject != null || selectedType != null)
            {
                ObjectTreeViewModel newTree = new ObjectTreeViewModel(activeObject, selectedType);
                newTree.PropertyChanged += this.OnTreePropertyChanged;
                TreeViewModel = newTree;
            }
            else
            {
                TreeViewModel = null;
            }

            OnPropertyChanged(nameof(DataEditorViewModel.ActiveObject));
        }

        private void OnTreePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedNode")
            {
                OnPropertyChanged(nameof(SelectedEditorViewModel));
            }
            else if (e.PropertyName == "HasBeenProcessed")
            {
                OnPropertyChanged(nameof(SelectedEditorViewModel));
            }
        }

        private void ExecuteShowStartPage()
        {
            // TODO save changes
            ViewState = EViewState.ShowStartPage;
        }
    }
}
