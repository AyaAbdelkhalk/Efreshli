using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.File
{
    public interface IImageService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<string> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);

        
    }
}
