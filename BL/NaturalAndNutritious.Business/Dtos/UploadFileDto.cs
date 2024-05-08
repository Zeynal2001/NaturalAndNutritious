using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Dtos
{
    public class UploadFileDto
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}
