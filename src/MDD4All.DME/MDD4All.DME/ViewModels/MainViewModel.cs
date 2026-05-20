using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MDD4All.AssemblyLoading.Contracts;
using MDD4All.Configuration;
using MDD4All.Configuration.Contracts;
using MDD4All.DME.AssemblyTree.ViewModels;
using MDD4All.DME.Configurations;
using MDD4All.DME.Services;
using MDD4All.DME.Services.Save_Load_Services.SaveServices.Interface;
using MDD4All.FileAccess.Contracts;
using MDD4All.UI.DataModels.Tree;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Input;

namespace MDD4All.DME.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ObjectTreeViewModel? _treeViewModel;

        private IConfigurationReaderWriter<DmeConfiguration> _configurationReaderWriter;

        private IFileLoader _fileLoader;
        private IFileSaver _fileSaver;
        private IAssemblyProvider _assemblyProvider;

        public MainViewModel(ObjectJsonManager dataManager, 
                             IFileSaveService saveService, 
                             IFileImportService importService,
                             IFileLoader fileLoader,
                             IFileSaver fileSaver,
                             IAssemblyProvider assemblyProvider)
        {
            DataManagerViewModel = new DataManagerViewModel(dataManager, saveService/*, importService*/);
            DataManagerViewModel.PropertyChanged += OnDataManagerViewModelPropertyChanged;

            _fileLoader = fileLoader;
            _fileSaver = fileSaver;
            _assemblyProvider = assemblyProvider;

            _configurationReaderWriter = new FileConfigurationReaderWriter<DmeConfiguration>("DME");

            _configuration = _configurationReaderWriter.GetConfiguration();

            if (_configuration == null)
            {
                _configuration = new DmeConfiguration();
            }

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            OpenDataModelCommand = new RelayCommand(ExeecuteOpenDataModel);
            ConfirmOpenDataModelCommand = new RelayCommand<DataModelDescriptor>(ExecuteConfirmOpenDataModelCommand);
        }

        

        private DmeConfiguration _configuration;

        public DmeConfiguration Configuration
        {
            get { return _configuration; }
            set { _configuration = value; }
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

        public AssemblyTreeViewModel? AssemblyTreeViewModel { get; private set; }

        public ICommand OpenDataModelCommand { get; private set; } = null!;

        public ICommand ConfirmOpenDataModelCommand { get; private set; } = null!;

        public ICommand NewDataFileCommand { get; private set; } = null!;

        public ICommand OpenDataFileCommand { get; private set; } = null!;

        public ICommand OpenRecentDataModelCommand { get; private set; } = null!;

        public ICommand OpenRecentDataFileCommand { get; private set; } = null!;



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

        #region COMMAND_IMPLEMENTATIONS

        private void ExeecuteOpenDataModel()
        {
            string filename = "";
            bool openResult = _fileLoader.ShowOpenFileDialog(out filename,
                                                             filter: "DLL Files (*.dll)|*.dll",
                                                             title: "Open Data Model library file...");

            if (openResult == true)
            {
                AssemblyTreeViewModel = new AssemblyTreeViewModel(filename, _assemblyProvider);
                ViewState = EViewState.ShowTypeSelectionView;
            }
        }

        private void ExecuteConfirmOpenDataModelCommand(DataModelDescriptor? descriptor)
        {
            if(descriptor != null)
            {
                Configuration.CurrentDataModel = descriptor;

                if(Configuration.RecentDataModels.Count == 5)
                {
                    Configuration.RecentDataModels.RemoveAt(4);
                    
                }
                Configuration.RecentDataModels.Insert(0, descriptor);

                _configurationReaderWriter.StoreConfiguration(Configuration);
            }

            ViewState = EViewState.ShowStartPage;
        }

        #endregion


    }
}