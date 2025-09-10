using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.ProductServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            var response = await _productService.CreateProductAsync(createProductDto);
            return this.CreateResponse(response);
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            return this.CreateResponse(response);
        }
        [HttpGet("GetMainProducts/{categoryId:int?}")]
          
        public async Task<IActionResult> GetMainProducts(int? categoryId= null)
        {
            var response = await _productService.GetMainProductsAsync(categoryId);
            return this.CreateResponse(response);
        }
        [HttpGet("GetMainProducts")]
        public async Task<IActionResult> GetMainProducts()
        {
            var response = await _productService.GetMainProductsAsync(null);
            return this.CreateResponse(response);
        }

        [HttpPost("test-3d")]
        public async Task<IActionResult> Test3D(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var apiKey = "tsk_lCR2QA_l_qNJaEwJPdGBPUxa-Z3V-RcRBpu7STdQV03"; // Replace with your actual API key
            var uploadUrl = "https://api.tripo3d.ai/v2/openapi/upload/sts";

            using var httpClient = new HttpClient();

            // Add Authorization header
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            using var form = new MultipartFormDataContent();

            // Convert IFormFile to StreamContent
            using var fileStream = file.OpenReadStream();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            // Add the file content to the form
            form.Add(streamContent, "file", file.FileName);

            // Send the request
            var response = await httpClient.PostAsync(uploadUrl, form);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, responseContent);
            }

            // Return JSON response
            return Content(responseContent, "application/json");
        }
        [HttpPost("generate-3d")]
        public async Task<IActionResult> Generate3D(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var apiKey = "tsk_FDaCnYphnOJf37GntAJSHpm8KMNYYllj0NEFNvgedBQ";
            var uploadUrl = "https://api.tripo3d.ai/v2/openapi/upload/sts";
            var taskUrl = "https://api.tripo3d.ai/v2/openapi/task"; // ✅ Correct endpoint

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            httpClient.Timeout = TimeSpan.FromMinutes(5);

            // Step 1: Upload the file
            using var form = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            form.Add(streamContent, "file", file.FileName);

            var uploadResponse = await httpClient.PostAsync(uploadUrl, form);
            var uploadResult = await uploadResponse.Content.ReadAsStringAsync();

            if (!uploadResponse.IsSuccessStatusCode)
                return StatusCode((int)uploadResponse.StatusCode, uploadResult);

            // Parse image_token
            var json = System.Text.Json.JsonDocument.Parse(uploadResult);
            var imageToken = json.RootElement.GetProperty("data").GetProperty("image_token").GetString();

            // Step 2: Request model generation with correct format
            var requestData = new
            {
                type = "image_to_model", // ✅ Required field
                file = new
                {
                    type = file.ContentType.Split('/')[1], // Extract file extension from ContentType
                    file_token = imageToken // ✅ Use file_token from upload
                }
                // You can add optional parameters here:
                // model_version = "v2.5-20250123",
                // texture = true,
                // pbr = true,
                // etc.
            };

            var jsonData = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json"
            );

            var generateResponse = await httpClient.PostAsync(taskUrl, jsonData);
            var generateResult = await generateResponse.Content.ReadAsStringAsync();

            if (!generateResponse.IsSuccessStatusCode)
                return StatusCode((int)generateResponse.StatusCode, generateResult);

            return Content(generateResult, "application/json");
        }

        // Stability AI Free API Example
        [HttpPost("generate-3d-free")]
        public async Task<IActionResult> Generate3DFree(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var apiKey = "sk-t1sASYVhLeOq9knHqurgv2O5XAOtU9kLOobQkzR73yCJ0sdh";

                // ✅ CORRECT ENDPOINT for 3D generation
                var apiUrl = "https://api.stability.ai/v2beta/3d/model/create";

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Convert image to base64
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var base64Image = Convert.ToBase64String(memoryStream.ToArray());

                var requestData = new StabilityAIRequest
                {
                    image = base64Image,
                    prompt = "Generate a 3D model from this image",
                    output_format = "glb"
                };

                var jsonContent = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestData, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync(apiUrl, jsonContent);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Stability AI API Error: {result}");
                }

                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }


        [HttpPost("generate-stability-3d")]
        public async Task<IActionResult> Generate3DFromStability(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("Image file is required.");

            var apiKey = "sk-vN6r1orqJ6ytlAFKaRQewXd0mDtnR1rrM3Sa0s62bg3OhTrQ"; // Replace with your key
            var stabilityUrl = "https://api.stability.ai/v2beta/3d/stable-fast-3d";

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


            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await httpClient.PostAsync(stabilityUrl, form);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, error);
            }

            var glbBytes = await response.Content.ReadAsByteArrayAsync();
            return File(glbBytes, "model/gltf-binary", "model.glb");
        }

        // Add this class definition somewhere in your file
        public class StabilityAIRequest
        {
            public string image { get; set; }
            public string prompt { get; set; }
            public string output_format { get; set; }
        }

        // Hugging Face Free API Example
        [HttpPost("generate-3d-hf")]
        public async Task<IActionResult> Generate3DHuggingFace(IFormFile file)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/stabilityai/stable-diffusion-3d";
            var apiKey = "sk-t1sASYVhLeOq9knHqurgv2O5XAOtU9kLOobQkzR73yCJ0sdh"; // Free from huggingface.co

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            using var form = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            var streamContent = new StreamContent(fileStream);
            form.Add(streamContent, "image", file.FileName);

            var response = await httpClient.PostAsync(apiUrl, form);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

    }
}
