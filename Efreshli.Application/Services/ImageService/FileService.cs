//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Hosting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Efreshli.Application.Services.File
//{
//    public class FileService : IFileService
//    {
//        private readonly IWebHostEnvironment _webHostEnvironment;

//        public FileService(IWebHostEnvironment webHostEnvironment)
//        {
//            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
//        }

//        public async Task<string> UploadImage(string location, IFormFile file)
//        {
//            // Validate inputs
//            if (string.IsNullOrWhiteSpace(location))
//                throw new ArgumentException("Location cannot be null or empty", nameof(location));

//            if (file == null || file.Length == 0)
//                throw new ArgumentException("Invalid file.", nameof(file));

//            // Validate file extension
//            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
//            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

//            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
//                throw new ArgumentException("Unsupported file type.");

//            // Ensure WebRootPath exists
//            if (string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
//            {
//                _webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
//            }

//            // Create full upload path
//            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, location);

//            // Create directory if it doesn't exist
//            if (!Directory.Exists(uploadPath))
//            {
//                Directory.CreateDirectory(uploadPath);
//            }

//            // Generate unique filename
//            var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
//            var filePath = Path.Combine(uploadPath, uniqueFileName);

//            // Save the file
//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            // Return relative path
//            return $"/{location}/{uniqueFileName}";
//        }
//    }
//}