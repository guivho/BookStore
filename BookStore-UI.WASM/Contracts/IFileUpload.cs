using BlazorInputFile;
using System.IO;
using System.Threading.Tasks;

namespace BookStore_UI.WASM.Contracts
{
    public interface IFileUpload
    {
        public Task UploadFile(IFileListEntry file, MemoryStream memoryStream, string picName);
        public void RemoveFile(string picName);
    }
}
