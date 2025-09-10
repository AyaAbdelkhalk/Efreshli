using Efreshli.Application.DTOs.AddressDTOs;
using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.AddressServices
{
    public interface IAddressService
    {
        Task<Response<AddressResponseDto>> CreateAsync(CreateAddressDto addressDto);
        Task<Response<AddressResponseDto>> UpdateAsync(UpdateAddressDto addressDto);
        Task<AddressResponseDto> GetByIdAsync(int  addressId);
        Task<Response<IEnumerable<AddressResponseDto>>> GetByUserIdAsync(string userId);
        Task<Response<IEnumerable<AddressResponseDto>>> GetCurrentUserAddressesAsync();
        Task<Response<bool>> DeleteAsync(int  addressId);
    }
}
