using MDD4All.DME.App.Wpf.Pages;
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

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _services = serviceProvider;
            blazorWebView.Services = serviceProvider;

            // One culture for the whole run - the interface is English and there is nothing left
            // to switch it with. It still matters: the culture decides how numbers and dates are
            // read and written in the editor's fields.
            SetCulture(new CultureInfo("en-US"));
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
