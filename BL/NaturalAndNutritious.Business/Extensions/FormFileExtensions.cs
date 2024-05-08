using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Extensions
{
    public static class FormFileExtensions
    {
        public static string GenerateUploadName(this IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var bytes = new byte[20];
            var rng = RandomNumberGenerator.Create();

            rng.GetBytes(bytes);

            var newName = Convert.ToBase64String(bytes);

            newName = newName.Replace("\\", "_");
            newName = newName.Replace("/", "_");

            return $"{newName}{extension}";
        }


        public static long GetSizeKb(this IFormFile file) => file.Length / 1024;
        /*
        public static long GetSizeKb(this IFormFile file)
        {
            return file.Length / 1024;
        }
        */
    }
}
