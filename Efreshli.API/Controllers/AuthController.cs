using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.AuthServices;
using Efreshli.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);
            return this.CreateResponse(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            return this.CreateResponse(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var result = await _authService.ForgotPasswordAsync(model);
            return this.CreateResponse(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var result = await _authService.ResetPasswordAsync(model);
            return this.CreateResponse(result);
        }

        [HttpPost("create-admin")]
        //[Authorize(Roles = UserRoles.Admin)] // Only admins can create other admins
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto model)
        {
            var result = await _authService.CreateAdminAsync(model);
            return this.CreateResponse(result);
        }
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            var result = await _authService.ConfirmEmailAsync(request.Email, request.Token);

            if (result.Succeeded)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }

        public class ConfirmEmailRequest
        {
            public string Email { get; set; }
            public string Token { get; set; }
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationRequest request)
        {
            var result = await _authService.ResendConfirmationEmailAsync(request.Email);
            return this.CreateResponse(result);
        }
        // OAuth 
        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto model)
        {
            var result = await _authService.ExternalLoginAsync(model);
            return this.CreateResponse(result);
        }

        #region OAuth
        //[HttpPost("oauth/register")]
        //public async Task<IActionResult> OAuthRegister([FromBody] OAuthRegisterDto model)
        //{
        //    Console.WriteLine($"OAuthLogin Request: {System.Text.Json.JsonSerializer.Serialize(model)}");
        //    var result = await _authService.OAuthRegisterAsync(model);
        //    return this.CreateResponse(result);
        //}

        //[HttpPost("oauth/login")]
        //public async Task<IActionResult> OAuthLogin([FromBody] OAuthRegisterDto model)
        //{
        //    Console.WriteLine($"OAuthLogin Request: {System.Text.Json.JsonSerializer.Serialize(model)}");
        //    var result = await _authService.OAuthLoginAsync(model);
        //    return this.CreateResponse(result);
        //}
        #endregion
    }
}
