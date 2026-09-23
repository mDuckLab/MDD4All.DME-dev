using MDD4All.DME.ViewModels.DataManager;
using MDD4All.Localization.Contracts;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;

namespace MDD4All.DME.App.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IServiceProvider _services;
        private ILanguageSetter _languageSetter = null!;
        private DataManagerObjectViewModel? _dataManagerObject;
        private DataManagerFileViewModel? _dataFile;

        // Set once the question has been answered, so the second close goes straight through.
        private bool _closeConfirmed = false;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _services = serviceProvider;
            blazorWebView.Services = serviceProvider;

            var languageSetter = serviceProvider.GetService(typeof(ILanguageSetter));

            if (languageSetter != null)
            {
                _languageSetter = (ILanguageSetter)languageSetter;
            }

            _dataManagerObject = serviceProvider.GetService(typeof(DataManagerObjectViewModel))
                                     as DataManagerObjectViewModel;
            _dataFile = serviceProvider.GetService(typeof(DataManagerFileViewModel))
                            as DataManagerFileViewModel;

            if (_dataManagerObject != null)
            {
                _dataManagerObject.PropertyChanged += OnDocumentPropertyChanged;
            }

            if (_dataFile != null)
            {
                // Carries the path as well - it is what changes when a file is opened or saved.
                _dataFile.PropertyChanged += OnDocumentPropertyChanged;
            }

            UpdateTitle();

            if (_languageSetter != null)
            {
                _languageSetter.CultureChanged += OnCultureChanged;

                // Whatever was picked last time - it was read from the configuration when the
                // setter was built.
                SetCulture(_languageSetter.CurrentCulture);
            }
        }

        // Closing throws away whatever was never written, so it asks the same question as
        // opening another file - and with the same dialog, drawn by the editor itself. The
        // first attempt is turned away; the answer brings the second one.
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_closeConfirmed && _dataManagerObject != null && _dataManagerObject.HasUnsavedChanges)
            {
                e.Cancel = true;

                if (_dataFile != null)
                {
                    _dataFile.RequestShutdown(() => Dispatcher.Invoke(() =>
                    {
                        _closeConfirmed = true;
                        this.Close();
                    }));
                }

                return;
            }

            base.OnClosing(e);
        }

        // Both notifications arrive from whatever thread the editor runs on, the title belongs
        // to this one.
        private void OnDocumentPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(UpdateTitle);
        }

        // "Synorvia" alone while nothing is open, the file behind it once there is, and a star
        // as long as what is in memory differs from what is on disk.
        private void UpdateTitle()
        {
            string title = "Synorvia";

            if (_dataManagerObject != null && _dataManagerObject.HasContent)
            {
                if (_dataFile != null && _dataFile.CurrentFilePath != "")
                {
                    title += " - " + Path.GetFileName(_dataFile.CurrentFilePath);
                }

                if (_dataManagerObject.HasUnsavedChanges)
                {
                    title += " *";
                }
            }

            this.Title = title;
        }

        private void OnCultureChanged(object? sender, System.EventArgs e)
        {
            // Remember the choice, so the next start comes up in the same language.
            if (_services.GetService(typeof(DataManagerSettingsViewModel))
                    is DataManagerSettingsViewModel settings)
            {
                settings.DesiredLanguage = _languageSetter.CurrentCulture.Name;
            }

            SetCulture(_languageSetter.CurrentCulture);
        }

        // Only the two static defaults. Assigning CurrentCulture/CurrentUICulture here as well was
        // tried and measured: the renderer keeps reading the language it started with, because it
        // holds its own value in the execution context it captured when it was created. Seven
        // variants were checked, none reached it - see AppTextProvider for what is done instead.
        private void SetCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
