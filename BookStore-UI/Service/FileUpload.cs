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


        public async Task UploadFile(IFileListEntry file, MemoryStream memoryStream, string picName)
        {
            try
            {
                await file.Data.CopyToAsync(memoryStream);

                var path = $"{_webHostEnvironment.WebRootPath}\\uploads\\{picName}";

                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    memoryStream.WriteTo(fileStream);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
