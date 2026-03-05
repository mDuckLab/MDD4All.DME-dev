using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MDD4All.UI.BlazorComponents.Services;
using MDD4All.DME.Services;
using MDD4All.DME.ViewModels;
using MDD4All.DME.Services.Save_Load_Services.SaveServices.Interface;
using KMRD.KamcosRelease.DataModels;

namespace MDD4All.DME
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SystemReleaseInfo systemReleaseInfo = new SystemReleaseInfo();

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddScoped<DragDropDataProvider>();
            builder.Services.AddScoped<ObjectJsonManager>();
            builder.Services.AddScoped<IFileSaveService, BlazorWebFileSaveService>();
            builder.Services.AddScoped<IFileImportService, BlazorWebFileImportService>();
            builder.Services.AddScoped<MainViewModel>();

            WebApplication app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}