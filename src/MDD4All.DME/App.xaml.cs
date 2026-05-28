using MDD4All.AssemblyLoading.Contracts;
using MDD4All.DME.DataAccess.Assemblies;
using MDD4All.DME.ViewModels;
using MDD4All.DME.ViewModels.Save_Load_Services.SaveServices.Interface;
using MDD4All.FileAccess.Contracts;
using MDD4All.FileAccess.WPF;
using MDD4All.UI.BlazorComponents.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace MDD4All.DME
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost Host { get; private set; } = null!;

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

                    services.AddScoped<DragDropDataProvider>();
                    //_services.AddScoped<DataEditorViewModel>();
                    services.AddScoped<IFileSaveService, BlazorWebFileSaveService>();
                    services.AddScoped<IFileImportService, BlazorWebFileImportService>();

                    services.AddSingleton<IFileLoader, WpfFileLoader>();
                    services.AddSingleton<IFileSaver, WpfFileSaver>();
                    services.AddTransient<IAssemblyProvider>(provider => {
                        AssemblyPovider assemblyPovider = new AssemblyPovider();
                        assemblyPovider.ProxiesDllPath = @"c:\work\DME_fork\src\MDD4All.DME.Proxies\bin\Debug\netstandard2.0\MDD4All.DME.Proxies.dll";
                        return assemblyPovider;
                    });
                    services.AddScoped<MainViewModel>();

                    //Resources.Add("services", _services.BuildServiceProvider());

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
