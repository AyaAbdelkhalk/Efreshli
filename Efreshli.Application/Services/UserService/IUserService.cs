using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.UserService
{
    public interface IUserService
    {

        Task<int> TotalCount();
        Task<int> CustomersCount();
        Task<int> AdminsCount();
        Task<Response<UserProfileResponseDto>> GetProfileDataAsync();
        //Task<PaginatedResult<ProfileResponseDto>> PageResult(string userRole=null);
        Task<Response<string>> UpdateProfileAsync(UpdateProfileDto updateProfileDto);

    }
}
