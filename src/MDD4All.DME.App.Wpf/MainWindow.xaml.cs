using MDD4All.DME.ViewModels.DataManager;
using MDD4All.Localization.Contracts;
using System;
using System.Globalization;
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

            if (_languageSetter != null)
            {
                _languageSetter.CultureChanged += OnCultureChanged;

                // Whatever was picked last time - it was read from the configuration when the
                // setter was built.
                SetCulture(_languageSetter.CurrentCulture);
            }
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
