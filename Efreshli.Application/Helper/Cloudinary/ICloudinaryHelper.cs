using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Helper.Cloudinary
{
    public interface ICloudinaryHelper
    {
        Task<ImageUploadResult> UploadImageAsync(ImageUploadParams imageUploadParams);
        Task<RawUploadResult> UploadRawAsync(RawUploadParams rawUploadParams);
        //Task<string> UploadImageAsync(IFormFile file);
        Task<DeletionResult> DeleteAsync(string publicId);
    }
}
