using Efreshli.Application.Services.File;
using Efreshli.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IImageService _imageService;

        public TestController(IImageService imageService)
        {
            this._imageService = imageService;
        }
        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var s = await _imageService.UploadImageAsync(file,ImageReferenceType.Category,4);

            return Ok(s);
        }
    }
}
