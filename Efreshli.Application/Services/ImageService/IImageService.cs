using CloudinaryDotNet.Actions;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
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

        Task<Image> UploadImageAsync(IFormFile file, ImageReferenceType referenceType, int referenceId);
        Task<bool> DeleteImageAsync(int imageId); 
        string GetImageUrl(int imageId);
    }
}
