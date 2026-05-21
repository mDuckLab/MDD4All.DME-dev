using MDD4All.DME.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MDD4All.DME.Views.RawData
{
    public partial class RawDataView
    {
        [Parameter]
        public DataEditorViewModel DataContext { get; set; } = null!;
    }
}