using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.StabilityServices
{
    public class StabilityService:IStabilityService
    {
        private readonly string _apiKey = "sk-KynpBs9iDRhoCF3hQbJTnncCDgZ31TtLyUXNB9F8bCgFibIx";
        private readonly string _stabilityUrl = "https://api.stability.ai/v2beta/3d/stable-fast-3d";

        public async Task<IFormFile> ConvertToGlbAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required");

            using var httpClient = new HttpClient();
            using var form = new MultipartFormDataContent();
            using var imageStream = imageFile.OpenReadStream();

            var content = new StreamContent(imageStream);
            content.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"image\"",
                FileName = $"\"{imageFile.FileName}\""
            };

            form.Add(content);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await httpClient.PostAsync(_stabilityUrl, form);
            response.EnsureSuccessStatusCode();

            var glbBytes = await response.Content.ReadAsByteArrayAsync();

            // Convert the response into an IFormFile
            return FileHelpers.ByteArrayToFormFile(glbBytes, "model.glb", "model/gltf-binary");
        }
    }
}
