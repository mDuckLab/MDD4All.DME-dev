using MDD4All.Localization.Contracts;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace MDD4All.DME.App.Wpf.Pages
{
    public partial class App
    {
        [Inject]
        ILanguageSetter LanguageSetter { get; set; } = null!;

        // ILanguageSetter records which language was picked and says so - it deliberately does not
        // touch CultureInfo itself, because what "apply a culture" means differs between WPF, a
        // web app and WebAssembly. Carrying the choice over is the application's job.
        //
        // Only the two defaults here. Assigning CurrentUICulture would give this flow a value of
        // its own, and a flow that has one stops reading the default - which is exactly what made
        // later switching impossible.
        protected override void OnInitialized()
        {
            if (LanguageSetter != null)
            {
                CultureInfo culture = LanguageSetter.CurrentCulture;

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }
        }
    }
}
