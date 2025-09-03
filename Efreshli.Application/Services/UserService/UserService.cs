using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
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

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
