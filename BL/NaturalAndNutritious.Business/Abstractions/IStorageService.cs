using Microsoft.AspNetCore.Http;
using NaturalAndNutritious.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Abstractions
{
    public interface IStorageService
    {
        Task<UploadFileDto> UploadFileAsync(string dirPath, IFormFile file);
        Task<IEnumerable<UploadFileDto>> UploadFilesAsync(string dirPath, IFormFileCollection files);
        bool HasFile(string dirPath, string fileName);
        Task DeleteFileAsync(string dirPath, string fileName);
    }
}
