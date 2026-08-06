using MDD4All.Localization.Contracts;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace MDD4All.DME.App.Wpf.Pages
{
    public partial class App
    {
        [Inject]
        ILanguageSetter LanguageSetter { get; set; } = null!;

        protected override void OnInitialized()
        {
            if (LanguageSetter != null)
            {
                CultureInfo culture = LanguageSetter.CurrentCulture;


                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            } 
        }
    }
}