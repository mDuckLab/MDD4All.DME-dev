using CommunityToolkit.Mvvm.ComponentModel;
using MDD4All.UI.DataModels.Tree;
using System.ComponentModel;
using System;
using MDD4All.DME.Services;
using MDD4All.DME.Services.Save_Load_Services.SaveServices.Interface;

namespace MDD4All.DME.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ObjectTreeViewModel? _treeViewModel;

        public MainViewModel(ObjectJsonManager dataManager, IFileSaveService saveService, IFileImportService importService)
        {
            this.DataManagerViewModel = new DataManagerViewModel(dataManager, saveService/*, importService*/);
            this.DataManagerViewModel.PropertyChanged += OnDataManagerViewModelPropertyChanged;
        }

        public DataManagerViewModel DataManagerViewModel { get; private set; }

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

        public object? DataContext
        {
            get
            {
                return DataManagerViewModel.ActiveObject;
            }
        }

        public ITreeNode? SelectedEditorViewModel
        {
            get
            {
                ITreeNode? result = null;
                if (this.TreeViewModel != null)
                {
                    result = this.TreeViewModel.SelectedNode;
                }
                return result;
            }
        }

        private void OnDataManagerViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveObject" || e.PropertyName == "SelectedType")
            {
                RebuildTree();
            }
        }

        private void RebuildTree()
        {
            if (this.TreeViewModel != null)
            {
                this.TreeViewModel.PropertyChanged -= this.OnTreePropertyChanged;
            }

            object? activeObject = DataManagerViewModel.ActiveObject;
            Type? selectedType = DataManagerViewModel.SelectedType;

            if (activeObject != null || selectedType != null)
            {
                ObjectTreeViewModel newTree = new ObjectTreeViewModel(activeObject, selectedType);
                newTree.PropertyChanged += this.OnTreePropertyChanged;
                this.TreeViewModel = newTree;
            }
            else
            {
                this.TreeViewModel = null;
            }

            OnPropertyChanged(nameof(DataContext));
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
    }
}