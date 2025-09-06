using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Resources;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IUserContext _userContext;
        public UserService(IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _userContext = userContext;
        }
        public async Task<int> AdminsCount()
        {
           var addminCount= await _unitOfWork.UserRepository.CountAsync(x=>x.Role.Equals(UserRoles.Admin));
            return addminCount;
        }

        public async Task<int> CustomersCount()
        {
            var customersCount = await _unitOfWork.UserRepository.CountAsync(x => x.Role.Equals(UserRoles.Customer));
            return customersCount;
        }

        public Task<PaginatedResult<UserDto>> PageResult(string userRole = null)
        {
            throw new NotImplementedException();
        }

        public Task<int> TotalCount()
        {
            throw new NotImplementedException();
        }
        public async Task<Response<string>> UpdateProfileAsync( UpdateProfileDto updateProfileDto)
        {
            var userId = _userContext.CurrentUserId;
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(x=>x.Id==userId);
            
            if (user == null) return ResponseHandler.NotFound<string>(_localizer[SharedResourcesKeys.Error.DataNotFound]);
            var mappedUser = updateProfileDto.Adapt(user);
            var update =await _unitOfWork.UserRepository.UpdateAsync(mappedUser);
            if (user == null) return ResponseHandler.BadRequest<string>(_localizer[SharedResourcesKeys.Error.BadRequest]);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Success<string>(_localizer[SharedResourcesKeys.Success.Updated]);

        }

    }
}
