using BlazorInputFile;
using BookStore_UI.Contracts;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BookStore_UI.Service
{
    public class FileUpload : IFileUpload
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUpload(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public void RemoveFile(string picName)
        {
            if(picName != null)
            {
                var path = $"{_webHostEnvironment.WebRootPath}/uploads/{picName}";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        public async Task UploadFile(Stream stream, string picName)
        {
            try
            {
                var path = $"{_webHostEnvironment.WebRootPath}/uploads/{picName}";
                var buffer = new byte[4 * 1096];
                int bytesRead;
                double totalRead = 0;
                
                using var fileStream = new FileStream(path, FileMode.Create);
                while ((bytesRead = await stream.ReadAsync(buffer)) != 0)
                {
                    totalRead += bytesRead;
                    await fileStream.WriteAsync(buffer);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
