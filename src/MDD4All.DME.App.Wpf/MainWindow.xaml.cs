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

        // Switching the language needs two things, and the second one is the surprising one.
        //
        // CurrentUICulture lives in an AsyncLocal since .NET Core - it belongs to the execution
        // flow, not to the thread. Every way of assigning it writes into the flow doing the
        // assigning, so nothing can reach the flow the renderer is already running in. Setting it
        // from here, from a Blazor callback, or on the dispatcher thread all fail the same way,
        // and all of them fail silently.
        //
        // DefaultThreadCurrentUICulture below is the exception: it applies to flows that have no
        // value of their own yet. Reloading the web view starts exactly such a flow, so the new
        // culture takes hold there.
        //
        // Nothing is lost by it: the view models are singletons in the WPF container, and the web
        // view uses that same container. The component tree is rebuilt, the open document is not.
        private void OnCultureChanged(object? sender, System.EventArgs e)
        {
            SetCulture(_languageSetter.CurrentCulture);
        }

        // Only the two static defaults, and deliberately nothing else.
        //
        // CurrentCulture and CurrentUICulture live in an AsyncLocal. The moment a flow has a value
        // of its own it stops consulting the default - forever. Assigning them once at startup is
        // exactly what pinned the renderer to the language it began with, and no later assignment
        // could reach it, because a flow cannot be written to from outside.
        //
        // Left alone, every flow falls through to these two. They are plain statics, so changing
        // them changes what everything reads.
        private void SetCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }


    }
}
