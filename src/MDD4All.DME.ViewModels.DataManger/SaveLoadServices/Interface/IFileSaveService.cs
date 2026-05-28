using System.Threading.Tasks;

namespace MDD4All.DME.ViewModels
{
    public interface IFileSaveService
    {
        Task SaveFileAsync(string fileName, string base64Data);
    }
}
