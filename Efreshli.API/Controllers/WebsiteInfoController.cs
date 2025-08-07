using Efreshli.Application.DTOs.WebsiteInfoDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.WebsiteInfoServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsiteInfoController : ControllerBase
    {
        private readonly IWebsiteInfoService _websiteInfoService;

        public WebsiteInfoController(IWebsiteInfoService websiteInfoService)
        {
            _websiteInfoService = websiteInfoService;
        }

        [HttpPost]
        public async Task<IActionResult> AddWebsiteInfo([FromForm] CreateWebsiteInfoDto dto)
        {
            var result = await _websiteInfoService.AddWebsiteInfoAsync(dto);
            return this.CreateResponse(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWebsiteInfos()
        {
            var result = await _websiteInfoService.GetAllWebsiteInfosAsync();
            return this.CreateResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWebsiteInfoById(int id)
        {
            var result = await _websiteInfoService.GetWebsiteInfoByIdAsync(id);
            return this.CreateResponse(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWebsiteInfo(int id, [FromForm] UpdateWebsiteInfoDto dto)
        {
            var result = await _websiteInfoService.UpdateWebsiteInfoAsync(id, dto);
            return this.CreateResponse(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWebsiteInfo(int id)
        {
            var result = await _websiteInfoService.DeleteWebsiteInfoAsync(id);
            return this.CreateResponse(result);
        }
    }
}
