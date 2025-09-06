using Efreshli.Application.DTOs;
using Efreshli.Application.DTOs.ContactUs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.ContactUsServices;
using Efreshli.Application.Services.EmailService;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactUsService _contactUsService;
        private readonly IEmailService _emailService;

        public ContactController(IContactUsService contactUsService,IEmailService emailService)
        {
            _contactUsService = contactUsService;
           _emailService = emailService;
        }

        [HttpPost("contact-us")] 
        public async Task<IActionResult> ContactUs(CreateContactUsDto model)
        {
          var result= await _contactUsService.AddContactUsAsync(model);
            return this.CreateResponse(result);
        }
        [HttpPost("become-vendor")] 
        public async Task<IActionResult> BeccomeAvendor(BecomeAVendorRequestDto model)
        {
          var result=await  _emailService.SendBecomeAVendordNotificationAsync(model);
            return this.CreateResponse(result);
        }
    }
}
