using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Efreshli.Application.Helper.Cloudinary;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Efreshli.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.File
{
    public class ImageService : IImageService
    {
        private readonly ICloudinaryHelper _cloudinary;
        private readonly IGenericRepository<Image> _imageRepo;

        public ImageService(ICloudinaryHelper cloudinary, IGenericRepository<Image> imageRepo)
        {
            _cloudinary = cloudinary;
            _imageRepo = imageRepo;
        }

        public async Task<Image> UploadImageAsync(IFormFile file, ImageReferenceType referenceType, int referenceId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid image file.");

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                //Transformation = new Transformation()
                //    .Height(600).Width(600)
            };

            var uploadResult = await _cloudinary.UploadImageAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");

            var image = new Image
            {
                URL = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId,
                ReferenceType = referenceType,
                ReferenceId = referenceId
            };

            await _imageRepo.AddAsync(image);
            await _imageRepo.SaveChangesAsync();

            return image;
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var image = await _imageRepo.GetByIdAsync(imageId);
            if (image == null) return false;
            var deleteResult = await _cloudinary.DeleteAsync(image.PublicId);
            if (deleteResult.Error != null)
                throw new Exception(deleteResult.Error.Message);

            await _imageRepo.RemoveAsync(image.Id);
            await _imageRepo.SaveChangesAsync();
            return true;
        }

        public string GetImageUrl(int imageId)
        {
            var image = _imageRepo.GetByIdQueryable(imageId).FirstOrDefault();
            if (image == null) return null;
            return image.URL;
        }
    }
}