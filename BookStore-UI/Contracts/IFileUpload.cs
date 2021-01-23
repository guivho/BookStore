using BlazorInputFile;
using System.IO;
using System.Threading.Tasks;

namespace BookStore_UI.Contracts
{
    public interface IFileUpload
    {
        public Task UploadFile(IFileListEntry file, MemoryStream memoryStream, string picName);
    }
}
