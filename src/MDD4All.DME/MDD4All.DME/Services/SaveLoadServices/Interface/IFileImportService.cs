using System.Threading.Tasks;

namespace MDD4All.DME.Services.Save_Load_Services.SaveServices.Interface
{
    public interface IFileImportService
    {
        Task OpenImportDialogAsync(string elementId);
    }
}
