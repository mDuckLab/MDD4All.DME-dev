using MDD4All.AssemblyLoading.Contracts;
using MDD4All.DME.DataAccess.Assemblies;
using MDD4All.DME.ViewModels.DataManager;
using MDD4All.DME.ViewModels.Editor;
using MDD4All.DME.ViewModels.Editor.Settings;
using MDD4All.FileAccess.Contracts;
using MDD4All.FileAccess.WPF;
using MDD4All.Localization;
using MDD4All.Localization.Contracts;
using MDD4All.UI.BlazorComponents.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.IO;
using System.Windows;

namespace MDD4All.DME
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost Host { get; private set; } = null!;

        

        public App()
        {
        }

        

        private void OnStartup(object sender, StartupEventArgs e)
        {
            Host = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(e.Args)
                .ConfigureServices((context, services) =>
                {
                    // Services
                    services.AddSingleton<MainWindow>();


                    // Blazor Hybrid
                    services.AddWpfBlazorWebView();

                    services.AddBlazorWebViewDeveloperTools();

                    services.AddLocalization(options =>
                    {

                        options.ResourcesPath = "Resources";
                    });


                    // Supported Cultures definieren
                    CultureInfo[] supportedCultures = new[]
                    {
                        new CultureInfo("de-DE"),
                        new CultureInfo("en-US")
                    };

                    // Optionen erstellen
                    RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
                    {
                        DefaultRequestCulture = new RequestCulture("en-US"),
                        SupportedCultures = supportedCultures,
                        SupportedUICultures = supportedCultures
                    };

                    // Optional registrieren (für DI verfügbar)
                    services.Configure<RequestLocalizationOptions>(options =>
                    {
                        options.DefaultRequestCulture = localizationOptions.DefaultRequestCulture;
                        options.SupportedCultures = supportedCultures;
                        options.SupportedUICultures = supportedCultures;
                    });


                    services.AddScoped<DragDropDataProvider>();
                    services.AddScoped<IFileSaveService, BlazorWebFileSaveService>();
                    services.AddScoped<IFileImportService, BlazorWebFileImportService>();

                    services.AddSingleton<ILanguageSetter>(setter => new LanguageSetter());

                    services.AddSingleton<IFileLoader, WpfFileLoader>();
                    services.AddSingleton<IFileSaver, WpfFileSaver>();
                    services.AddTransient<IAssemblyProvider>(provider => {
                        AssemblyPovider assemblyPovider = new AssemblyPovider();
                        assemblyPovider.ProxiesDllPath = Path.Combine(AppContext.BaseDirectory, "MDD4All.DME.Proxies.dll");
                        return assemblyPovider;
                    });
                    services.AddSingleton<DataFileManagerViewModel>();
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<INavigation>(provider => provider.GetRequiredService<MainViewModel>());
                    services.AddSingleton<EditorViewModel>();
                    services.AddSingleton<IEditorState>(provider => provider.GetRequiredService<EditorViewModel>());
                    services.AddSingleton<EditorAppearanceSettingsViewModel>();
                    services.AddSingleton<ExplorerSettingsViewModel>();
                })
                .Build();

            Host.Start();

            var mainWindow = Host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Host?.Dispose();
            base.OnExit(e);
        }

    }
}
