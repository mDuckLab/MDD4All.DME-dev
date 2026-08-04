using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MDD4All.DME.ViewModels.DataManager;
using MDD4All.Localization.Contracts;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace MDD4All.DME.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region constructor
        public MainViewModel(ILanguageSetter languageSetter, DataFileManagerViewModel dataFileManager)
        {
            _languageSetter = languageSetter;
            _languageSetter.CultureChanged += OnCultureChanged;

            _dataFileManager = dataFileManager;
            _dataFileManager.PropertyChanged += OnDataFileManagerPropertyChanged;

            InitializeCommands();
        }
        #endregion

        #region Properties

        private ILanguageSetter _languageSetter;

        private DataFileManagerViewModel _dataFileManager;

        private EViewState _viewState = EViewState.ShowStartPage;

        public EViewState ViewState
        {
            get
            { 
                return _viewState; 
            }

            set
            {
                _viewState = value;
                OnPropertyChanged(nameof(ViewState));
            }
        }

        // At most one overlay is ever open at once (modals block everything else).
        private EOverlayState _activeOverlay = EOverlayState.None;

        public EOverlayState ActiveOverlay
        {
            get
            {
                return _activeOverlay;
            }

            set
            {
                _activeOverlay = value;
                OnPropertyChanged(nameof(ActiveOverlay));
            }
        }

        #endregion

        #region Commands

        public ICommand ShowStartPageCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            ShowStartPageCommand = new RelayCommand(ExecuteShowStartPage);
        }

        private void ExecuteShowStartPage()
        {
            // TODO save changes
            ViewState = EViewState.ShowStartPage;
        }

        #endregion

        #region Event Handlers

        private void OnCultureChanged(object? sender, EventArgs e)
        {
            ActiveOverlay = EOverlayState.CultureChange;
        }

        private void OnDataFileManagerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataFileManagerViewModel.DataEditorViewModel))
            {
                ViewState = EViewState.ShowEditor;
            }
            else if (e.PropertyName == nameof(DataFileManagerViewModel.AssemblyTreeViewModel))
            {
                ActiveOverlay = _dataFileManager.AssemblyTreeViewModel != null
                    ? EOverlayState.TypeSelection
                    : EOverlayState.None;
            }
        }

        #endregion
    }
}
