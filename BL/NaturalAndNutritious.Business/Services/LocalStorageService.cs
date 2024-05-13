using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Extensions;

namespace NaturalAndNutritious.Business.Services
{
    public class LocalStorageService : IStorageService
    {
        public LocalStorageService(IWebHostEnvironment env)
        {
            _storagePath = Path.Combine(env.WebRootPath, "uploads");
        }
        
        private readonly string _storagePath;

        public async Task DeleteFileAsync(string dirPath, string fileName)
        {
            if (!HasFile(dirPath, fileName))
            {
                throw new FileNotFoundException();
            }

            var path = Path.Combine(_storagePath, dirPath, fileName);
            
            await Task.Run(() => File.Delete(path));
        }

        public bool HasFile(string dirPath, string fileName)
        {
            var path = Path.Combine(_storagePath ,dirPath, fileName);

            return File.Exists(path);
        }

        public async Task<UploadFileDto> UploadFileAsync(string dirPath, IFormFile file)
        {
            var path = Path.Combine(_storagePath, dirPath);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var uploadName = file.GenerateUploadName();
            var fileStream = File.Open(Path.Combine(path, uploadName), FileMode.CreateNew);
            
            await file.CopyToAsync(fileStream);
            fileStream.Close();

            return new UploadFileDto()
            {
                FileName = uploadName,
                FullPath = $"{dirPath}/{uploadName}", //Path.Combine(dirPath, uploadName),
                FileExtension = Path.GetExtension(path),
            };
        }

        public async Task<IEnumerable<UploadFileDto>> UploadFilesAsync(string dirPath, IFormFileCollection files)
        {
            var uploadedFiles = new List<UploadFileDto>();

            foreach (var file in files)
            {
                var dtoFileInstance = await UploadFileAsync(dirPath, file);
                uploadedFiles.Add(dtoFileInstance);
            }

            return uploadedFiles;
        }
    }
}
