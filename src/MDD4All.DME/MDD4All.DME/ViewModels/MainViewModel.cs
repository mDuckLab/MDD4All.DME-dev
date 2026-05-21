using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MDD4All.AssemblyLoading.Contracts;
using MDD4All.Configuration;
using MDD4All.Configuration.Contracts;
using MDD4All.DME.AssemblyTree.ViewModels;
using MDD4All.DME.Configurations;
using MDD4All.DME.ViewModels;
using MDD4All.DME.ViewModels.Save_Load_Services.SaveServices.Interface;
using MDD4All.FileAccess.Contracts;
using MDD4All.UI.DataModels.Tree;
using System;
using System.ComponentModel;
using System.Reflection;
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

        public MainViewModel(IFileSaveService saveService, 
                             IFileImportService importService,
                             IFileLoader fileLoader,
                             IFileSaver fileSaver,
                             IAssemblyProvider assemblyProvider)
        {
            //DataEditorViewModel = new DataManagerViewModel(dataManager, saveService/*, importService*/);
            //DataEditorViewModel.PropertyChanged += OnDataManagerViewModelPropertyChanged;

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
            SetDataModelFromRecentListCommand = new RelayCommand<int>(ExecuteSetDataModelFromRecentList);
            NewDataFileCommand = new RelayCommand(ExecuteNewDataFile);
            ShowStartPageCommand = new RelayCommand(ExecuteShowStartPage);
            OpenRecentDataFileCommand = new RelayCommand<int>(ExecuteOpenRecentDataFile);
        }

        

        private DmeConfiguration _configuration;

        public DmeConfiguration Configuration
        {
            get { return _configuration; }
            set { _configuration = value; }
        }


        public DataEditorViewModel? DataEditorViewModel { get; private set; }

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
                    if(TreeViewModel.SelectedNode is ObjectEditorViewModel)
                    {
                        ObjectEditorViewModel objectEditorViewModel = (ObjectEditorViewModel)TreeViewModel.SelectedNode;
                        objectEditorViewModel.EditorState.IsExpanded = true;
                    }
                    result = TreeViewModel.SelectedNode;
                }
                return result;
            }
        }

        public string StatusText
        {
            get
            {
                string result = "";
                if(DataEditorViewModel != null)
                {
                    result = "Filename: " + DataEditorViewModel.FileName;
                    result += " ● Data Model: " + Configuration.CurrentDataModel!.FullTypeName;
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

        public AssemblyTreeViewModel? AssemblyTreeViewModel { get; private set; }

        public ICommand OpenDataModelCommand { get; private set; } = null!;

        public ICommand ConfirmOpenDataModelCommand { get; private set; } = null!;

        public ICommand NewDataFileCommand { get; private set; } = null!;

        public ICommand OpenDataFileCommand { get; private set; } = null!;

        public ICommand OpenRecentDataModelCommand { get; private set; } = null!;

        public ICommand OpenRecentDataFileCommand { get; private set; } = null!;

        public ICommand SetDataModelFromRecentListCommand {  get; private set; } = null!;

        public ICommand ShowStartPageCommand { get; private set; } = null!;

        

        //private void OnDataManagerViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "ActiveObject" || e.PropertyName == "SelectedType")
        //    {
        //        RebuildTree();
        //    }
        //}

        private void RebuildTree()
        {
            if (this.TreeViewModel != null)
            {
                this.TreeViewModel.PropertyChanged -= this.OnTreePropertyChanged;
            }

            object? activeObject = DataEditorViewModel?.ActiveObject;
            Type? selectedType = DataEditorViewModel?.SelectedType;

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

        private void ExecuteSetDataModelFromRecentList(int index)
        {
            DataModelDescriptor descriptor = Configuration.RecentDataModels[index];

            Configuration.CurrentDataModel = descriptor;

            Configuration.RecentDataModels.RemoveAt(index);
            Configuration.RecentDataModels.Insert(0, descriptor);
            _configurationReaderWriter.StoreConfiguration(Configuration);
        }

        private void ExecuteNewDataFile()
        {
            string fileName = "";

            bool dialogResult = _fileSaver.ShowFileSaveDialog(out fileName, 
                                                              title: "New data file...",
                                                              filter: "JSON file (*.json)|*.json|XML file (*.xml)|*.xml|All files (*.*)|*.*",
                                                              defaultFileExtension: "json");

            if (dialogResult == true)
            {
                DataModelDescriptor? currentType = Configuration.CurrentDataModel;

                if (currentType != null)
                {
                    Assembly assembly = _assemblyProvider.GetAssemblyByPath(currentType.DllPath);

                    Type? type = assembly.GetType(currentType.FullTypeName);

                    if (type != null)
                    {
                        DataEditorViewModel = new DataEditorViewModel(fileName, type);

                        DataEditorViewModel.CreateNewInstance();

                        DataEditorViewModel.SaveDataFileCommand.Execute(null);

                        DataFileDescriptor dataFileDescriptor = new DataFileDescriptor
                        {
                            FilePath = fileName,
                            DataModelDescription = new DataModelDescriptor
                            {
                                DllPath = Configuration.CurrentDataModel!.DllPath,
                                FullTypeName = Configuration.CurrentDataModel.FullTypeName
                            }
                        };

                        if(Configuration.RecentDataFiles.Count == 5)
                        {
                            Configuration.RecentDataFiles.RemoveAt(4);
                        }

                        Configuration.RecentDataFiles.Insert(0, dataFileDescriptor);

                        _configurationReaderWriter.StoreConfiguration(Configuration);

                        RebuildTree();
                        ViewState = EViewState.ShowEditor;
                    }
                }
                
            }
        }

        private void ExecuteOpenRecentDataFile(int index)
        {
            DataFileDescriptor descriptor = Configuration.RecentDataFiles[index];

            if (descriptor != null)
            {
                Assembly assembly = _assemblyProvider.GetAssemblyByPath(descriptor.DataModelDescription.DllPath);

                Type? type = assembly.GetType(descriptor.DataModelDescription.FullTypeName);

                if (type != null)
                {
                    DataEditorViewModel = new DataEditorViewModel(descriptor.FilePath, type);

                    DataEditorViewModel.LoadFromFile();

                    RebuildTree();
                    ViewState = EViewState.ShowEditor;
                }
            }
        }


        private void ExecuteShowStartPage()
        {
            // TODO save changes
            ViewState = EViewState.ShowStartPage;
        }

        

        #endregion


    }
}