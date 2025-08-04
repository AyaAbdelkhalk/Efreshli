using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Efreshli.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Efreshli.Application.Helper.Cloudinary
{
    public class CloudinaryHelper:ICloudinaryHelper
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryHelper(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new CloudinaryDotNet.Cloudinary(acc);
        }

        public async Task<DeletionResult> DeleteAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result;
        }
        public async Task<ImageUploadResult> UploadImageAsync(ImageUploadParams imageUploadParams)
        {
            ImageUploadResult uploadResult = await _cloudinary.UploadAsync(imageUploadParams);

            return uploadResult; //?? throw new InvalidOperationException("Image upload failed.");
        }  
    }
}
