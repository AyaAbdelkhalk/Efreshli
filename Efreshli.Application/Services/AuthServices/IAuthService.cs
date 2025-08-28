using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.AuthServices
{
    public interface IAuthService
    {
        Task<Response<string>> RegisterAsync(RegisterDto model);
        Task<Response<string>> LoginAsync(LoginDto model);
        Task<Response<bool>> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<Response<bool>> ResetPasswordAsync(ResetPasswordDto model);
        Task<Response<string>> CreateAdminAsync(CreateAdminDto model);
        Task<Response<bool>> ResendConfirmationEmailAsync(string email);
        Task<Response<bool>> ConfirmEmailAsync(string email, string token);
        Task<Response<string>> ExternalLoginAsync(ExternalLoginDto model);

        //Task<Response<string>> OAuthRegisterAsync(OAuthRegisterDto model);
        //Task<Response<string>> OAuthLoginAsync(OAuthRegisterDto model);
    }
}

