using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MDD4All.AssemblyLoading.Contracts;
using MDD4All.Configuration;
using MDD4All.Configuration.Contracts;
using MDD4All.DME.AssemblyTree.ViewModels;
using MDD4All.DME.Configurations;
using MDD4All.DME.ViewModels.DataManager;
using MDD4All.DME.ViewModels.Editor;
using MDD4All.DME.Views;
using MDD4All.FileAccess.Contracts;
using MDD4All.Localization.Contracts;
using MDD4All.UI.DataModels.Tree;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using System.Xml.Serialization;

namespace MDD4All.DME.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ObjectTreeViewModel? _treeViewModel;

        private IConfigurationReaderWriter<DmeConfiguration> _configurationReaderWriter;

        private IFileLoader _fileLoader;
        private IFileSaver _fileSaver;
        private IAssemblyProvider _assemblyProvider;
        private ILanguageSetter _languageSetter;

        public MainViewModel(IFileSaveService saveService,
                             IFileImportService importService,
                             IFileLoader fileLoader,
                             IFileSaver fileSaver,
                             IAssemblyProvider assemblyProvider,
                             ILanguageSetter languageSetter)
        {
            //DataEditorViewModel = new DataManagerViewModel(dataManager, saveService/*, importService*/);
            //DataEditorViewModel.PropertyChanged += OnDataManagerViewModelPropertyChanged;

            _fileLoader = fileLoader;
            _fileSaver = fileSaver;
            _assemblyProvider = assemblyProvider;
            _languageSetter = languageSetter;

            _languageSetter.CultureChanged += OnCultureChanged;

            _configurationReaderWriter = new FileConfigurationReaderWriter<DmeConfiguration>("DME");

            _configuration = _configurationReaderWriter.GetConfiguration();

            if (_configuration == null)
            {
                _configuration = new DmeConfiguration();
            }

            InitializeCommands();
        }

        private void OnCultureChanged(object? sender, EventArgs e)
        {
            ViewState = EViewState.NewCultureRequested;
        }

        private void InitializeCommands()
        {
            OpenDataModelCommand = new RelayCommand(ExecuteOpenDataModel);
            ConfirmOpenDataModelCommand = new RelayCommand<DataModelDescriptor>(ExecuteConfirmOpenDataModelCommand);
            SetDataModelFromRecentListCommand = new RelayCommand<int>(ExecuteSetDataModelFromRecentList);
            NewDataFileCommand = new RelayCommand(ExecuteNewDataFile);
            ShowStartPageCommand = new RelayCommand(ExecuteShowStartPage);
            OpenRecentDataFileCommand = new RelayCommand<int>(ExecuteOpenRecentDataFile);
            OpenDataFileCommand = new RelayCommand(ExecuteOpenDataFile);
            ConfirmOpenDataFileCommand = new RelayCommand(ExecuteConfirmOpenDataFile);
            SaveDataFileCommand = new RelayCommand(ExecuteSaveDataFile);
            SaveDataFileAsCommand = new RelayCommand(ExecuteSaveDataFileAs);
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

        public string StatusText
        {
            get
            {
                string result = "";
                if (DataEditorViewModel != null)
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

        public ICommand ConfirmOpenDataFileCommand { get; private set; } = null!;

        public ICommand OpenRecentDataFileCommand { get; private set; } = null!;

        public ICommand SetDataModelFromRecentListCommand { get; private set; } = null!;

        public ICommand ShowStartPageCommand { get; private set; } = null!;

        public ICommand SaveDataFileCommand { get; private set; } = null!;

        public ICommand SaveDataFileAsCommand { get; private set; } = null!;

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

        private void ExecuteOpenDataModel()
        {
            SynchronizationContext.Current?.Post((_) =>
            {
                string filename = "";
                bool openResult = _fileLoader.ShowOpenFileDialog(out filename,
                                                                 filter: "DLL Files (*.dll)|*.dll",
                                                                 title: "Open Data Model library file...",
                                                                 initialDirectory: Configuration.LastUsedDataModelPath
                                                                 );

                if (openResult == true)
                {
                    AssemblyTreeViewModel = new AssemblyTreeViewModel(filename, _assemblyProvider);
                    ViewState = EViewState.ShowTypeSelectionView;
                }
            }, null);
        }

        private void ExecuteConfirmOpenDataModelCommand(DataModelDescriptor? descriptor)
        {
            if (descriptor != null)
            {
                Configuration.CurrentDataModel = descriptor;

                if (Configuration.RecentDataModels.Find(dm => dm.DllPath == descriptor.DllPath && dm.FullTypeName == descriptor.FullTypeName) == null)
                {
                    if (Configuration.RecentDataModels.Count == 5)
                    {
                        Configuration.RecentDataModels.RemoveAt(4);

                    }
                    Configuration.RecentDataModels.Insert(0, descriptor);

                    
                }

                FileInfo fileInfo = new FileInfo(descriptor.DllPath);

                if (fileInfo.DirectoryName != null)
                {
                    Configuration.LastUsedDataModelPath = fileInfo.DirectoryName;
                }
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
            SynchronizationContext.Current?.Post((_) =>
            {
                string fileName = "";

                bool dialogResult = _fileSaver.ShowFileSaveDialog(out fileName,
                                                                  initialDirectory: Configuration.LastUsedDataFilePath,
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
                            DataEditorViewModel = new DataEditorViewModel(fileName, type, _fileSaver);

                            DataEditorViewModel.CreateNewInstance();

                            SaveDataFileCommand.Execute(null);

                            DataFileDescriptor dataFileDescriptor = new DataFileDescriptor
                            {
                                FilePath = fileName,
                                DataModelDescription = new DataModelDescriptor
                                {
                                    DllPath = Configuration.CurrentDataModel!.DllPath,
                                    FullTypeName = Configuration.CurrentDataModel.FullTypeName
                                }
                            };

                            if (Configuration.RecentDataFiles.Count == 5)
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
            }, null);
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
                    SetRecentDataFileToTop(index);
                    Configuration.CurrentDataModel = descriptor.DataModelDescription;

                    SetTopRecentDataModel(descriptor.DataModelDescription);
                    _configurationReaderWriter.StoreConfiguration(Configuration);

                    DataEditorViewModel = new DataEditorViewModel(descriptor.FilePath, type, _fileSaver);

                    DataEditorViewModel.LoadFromFile();

                    RebuildTree();

                    ShowRawData = false;

                    ViewState = EViewState.ShowEditor;
                }
            }
        }


        private void ExecuteOpenDataFile()
        {
            SynchronizationContext.Current?.Post((_) =>
            {
                if (Configuration.CurrentDataModel != null)
                {
                    string filename = "";
                    bool openResult = _fileLoader.ShowOpenFileDialog(out filename,
                                                                     initialDirectory: Configuration.LastUsedDataFilePath,
                                                                     filter: "JSON file (*.json)|*.json|XML file (*.xml)|*.xml|All files (*.*)|*.*",
                                                                     title: "Open data file...",
                                                                     defaultFileExtension: "json");

                    if (openResult == true)
                    {
                        Assembly assembly = _assemblyProvider.GetAssemblyByPath(Configuration.CurrentDataModel!.DllPath);

                        Type? type = assembly.GetType(Configuration.CurrentDataModel!.FullTypeName);

                        if (type != null)
                        {
                            DataFileDescriptor dataFileDescriptor = new DataFileDescriptor
                            {
                                DataModelDescription = Configuration.CurrentDataModel,
                                FilePath = filename
                            };

                            AddNewRecentDataFile(dataFileDescriptor);

                            _configurationReaderWriter.StoreConfiguration(Configuration);

                            DataEditorViewModel = new DataEditorViewModel(dataFileDescriptor.FilePath, type, _fileSaver);

                            DataEditorViewModel.LoadFromFile();

                            RebuildTree();

                            ShowRawData = false;

                            ViewState = EViewState.ShowEditor;
                        }
                    }
                }
            }, null);
        }

        private void ExecuteShowStartPage()
        {
            // TODO save changes
            ViewState = EViewState.ShowStartPage;
        }

        private void ExecuteConfirmOpenDataFile()
        {
            throw new NotImplementedException();
        }

        private void ExecuteSaveDataFile()
        {
            FileInfo fileInfo = new FileInfo(DataEditorViewModel!.FileName);

            if (fileInfo.DirectoryName != null)
            {
                Configuration.LastUsedDataFilePath = fileInfo.DirectoryName;
                _configurationReaderWriter.StoreConfiguration(Configuration);
            }
            
            if (fileInfo.Extension.ToLower() == ".xml")
            {
                SerializeToXml();
            }
            else
            {
                File.WriteAllText(DataEditorViewModel!.FileName, DataEditorViewModel.ActiveObjectJsonString);
            }
        }

        private void ExecuteSaveDataFileAs()
        {
            SynchronizationContext.Current?.Post((_) =>
            {
                string fileName = "";

                bool dialogResult = _fileSaver.ShowFileSaveDialog(out fileName,
                                                                  initialDirectory: Configuration.LastUsedDataFilePath,
                                                                  title: "Save data file as...",
                                                                  filter: "JSON file (*.json)|*.json|XML file (*.xml)|*.xml|All files (*.*)|*.*");

                if (dialogResult == true)
                {
                    FileInfo fileInfo = new FileInfo(fileName);

                    DataEditorViewModel!.FileName = fileName;

                    if (fileInfo.Extension.ToLower() == ".xml")
                    {
                        SerializeToXml();
                    }
                    else
                    {
                        File.WriteAllText(fileName, DataEditorViewModel.ActiveObjectJsonString);
                    }

                    DataFileDescriptor dataFileDescriptor = new DataFileDescriptor()
                    {
                        DataModelDescription = Configuration.CurrentDataModel!,
                        FilePath = fileName
                    };

                    AddNewRecentDataFile(dataFileDescriptor);

                    if (fileInfo.DirectoryName != null)
                    {
                        Configuration.LastUsedDataFilePath = fileInfo.DirectoryName;
                    }

                    _configurationReaderWriter.StoreConfiguration(Configuration);

                    OnPropertyChanged(nameof(StatusText));
                }

            }, null);
        }

        #endregion

        private void SerializeToXml()
        {

            // Insert code to set properties and fields of the object.
            XmlSerializer mySerializer = new
            XmlSerializer(DataEditorViewModel!.SelectedType!);
            // To write to a file, create a StreamWriter object.
            StreamWriter myWriter = new StreamWriter(DataEditorViewModel.FileName);
            mySerializer.Serialize(myWriter, DataEditorViewModel.ActiveObject);
            myWriter.Close();
        }


        private void SetTopRecentDataModel(DataModelDescriptor modelDescriptor)
        {
            if (Configuration.RecentDataModels.Find(dm => dm.DllPath == modelDescriptor.DllPath && 
                                                          dm.FullTypeName == modelDescriptor.FullTypeName) == null)
            {
                if (Configuration.RecentDataModels.Count == 5)
                {
                    Configuration.RecentDataModels.RemoveAt(4);
                }

                Configuration.RecentDataModels.Insert(0, modelDescriptor);
            }
            else
            {
                int searchIndex = -1;
                for (int index = 0; index < Configuration.RecentDataModels.Count; index++)
                {
                    DataModelDescriptor currentDescriptor = Configuration.RecentDataModels[index];
                    if (currentDescriptor.FullTypeName == modelDescriptor.FullTypeName &&
                       currentDescriptor.DllPath == modelDescriptor.DllPath)
                    {
                        searchIndex = index;
                        break;
                    }
                }

                if (searchIndex >= 0)
                {
                    DataModelDescriptor existingDescriptor = Configuration.RecentDataModels[searchIndex];
                    Configuration.RecentDataModels.RemoveAt(searchIndex);
                    Configuration.RecentDataModels.Insert(0, existingDescriptor);
                }
            }
        }

        private void SetRecentDataFileToTop(int index)
        {
            DataFileDescriptor fileDescriptor = Configuration.RecentDataFiles[index];

            Configuration.RecentDataFiles.RemoveAt(index);
            Configuration.RecentDataFiles.Insert(0, fileDescriptor);
        }

        private void AddNewRecentDataFile(DataFileDescriptor dataFileDescriptor)
        {
            if (Configuration.RecentDataFiles.Count == 5)
            {
                Configuration.RecentDataFiles.RemoveAt(4);

                Configuration.RecentDataFiles.Insert(0, dataFileDescriptor);
            }
        }

    }
}