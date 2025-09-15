using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ImageService
{
    public interface IFileService
    {
        Task<string> UploadImage(IFormFile file, string location = "models3d");
    }
}
