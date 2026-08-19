using MDD4All.DME.App.Wpf.Pages;
using MDD4All.Localization;
using MDD4All.Localization.Contracts;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using System;
using System.Globalization;
using System.Threading;
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

                //SetCulture(_languageSetter.CurrentCulture, true);
            }

            SetCulture(new CultureInfo("de-DE"));
        }

        private void OnCultureChanged(object? sender, System.EventArgs e)
        {
            //SetCulture(_languageSetter.CurrentCulture);

        }

        private void SetCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            Application.Current.Dispatcher.Thread.CurrentCulture = culture;
            Application.Current.Dispatcher.Thread.CurrentUICulture = culture;

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }


    }
}
