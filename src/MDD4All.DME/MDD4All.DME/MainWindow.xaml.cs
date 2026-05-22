using MDD4All.AssemblyLoading.Contracts;
using MDD4All.DME.DataAccess.Assemblies;
using MDD4All.DME.ViewModels;
using MDD4All.DME.ViewModels.Save_Load_Services.SaveServices.Interface;
using MDD4All.DME.ViewModels;
using MDD4All.FileAccess.Contracts;
using MDD4All.FileAccess.WPF;
using MDD4All.UI.BlazorComponents.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MDD4All.DME
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceCollection _services = new ServiceCollection();

        public MainWindow()
        {
            InitializeComponent();

            InitializeServices();
        }

        private void InitializeServices()
        {
            _services.AddWpfBlazorWebView();

            _services.AddBlazorWebViewDeveloperTools();

            _services.AddRazorPages();

            _services.AddLocalization(options =>
            {

                options.ResourcesPath = "Resources";
            });

            _services.AddScoped<DragDropDataProvider>();
            //_services.AddScoped<DataEditorViewModel>();
            _services.AddScoped<IFileSaveService, BlazorWebFileSaveService>();
            _services.AddScoped<IFileImportService, BlazorWebFileImportService>();
            
            _services.AddSingleton<IFileLoader, WpfFileLoader>();
            _services.AddSingleton<IFileSaver, WpfFileSaver>();
            _services.AddScoped<IAssemblyProvider, AssemblyPovider>();
            _services.AddScoped<MainViewModel>();

            Resources.Add("services", _services.BuildServiceProvider());
        }
    }
}
