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
            SetCulture(_languageSetter.CurrentCulture);

            RebuildWebView();
        }

        // Throws the whole component tree away and builds it again, so every text is read afresh.
        //
        // This is here for the redraw, not for the culture. Three attempts were made to carry
        // CurrentUICulture into the renderer and all three were measured to fail: assigning it in
        // the switching flow, starting a brand new renderer, and starting one with the execution
        // context suppressed so it could inherit nothing. In this host CurrentUICulture cannot be
        // reached from outside, which is why the texts are looked up through AppTextProvider with
        // the picked language handed over explicitly.
        //
        // What the rebuild buys is that nothing has to remember anything: no subscriptions in
        // twenty components, no @key that discards a dialog while its own click is still running.
        // One white flash, everything current. Switching happens rarely enough for that to be the
        // cheaper deal.
        //
        // The document survives: the view models are singletons in the WPF container, and the web
        // view is handed that same container. Only the component tree is rebuilt.
        private void RebuildWebView()
        {
            WebViewContainer.Children.Remove(blazorWebView);

            // The control does not advertise IDisposable, so ask before letting go of it.
            if (blazorWebView is IDisposable disposable)
            {
                disposable.Dispose();
            }

            blazorWebView = new BlazorWebView
            {
                HostPage = @"wwwroot\index.html",
                Services = _services
            };

            blazorWebView.RootComponents.Add(new RootComponent
            {
                Selector = "#app",
                ComponentType = typeof(Pages.App)
            });

            WebViewContainer.Children.Add(blazorWebView);
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
