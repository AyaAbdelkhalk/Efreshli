using Efreshli.Application.DTOs.AddressDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Resources;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Efreshli.Application.Resources.SharedResourcesKeys.Efreshli;
using Addr = Efreshli.Domain.Models.Address;
namespace Efreshli.Application.Services.AddressServices
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IUserContext _userContext;
        #region Ctors & Fileds
        public AddressService(IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localizer,IUserContext userContext)

        {
           _unitOfWork = unitOfWork;
            _localizer = localizer;
            _userContext = userContext;
        }
        #endregion

        public async Task<Response<AddressResponseDto>> CreateAsync(CreateAddressDto createAddressDto)
        {
            var userId=_userContext.CurrentUserId;
            var addressToAdd = createAddressDto.Adapt<Addr>();
            addressToAdd.ApplicationUserId = userId;

           var result = await  _unitOfWork.AddressRepository.AddAsync(addressToAdd);
            if (result == null) ResponseHandler.BadRequest<AddressResponseDto>(_localizer[SharedResourcesKeys.Error.BadRequest]);
           await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Created<AddressResponseDto>(result.Adapt<AddressResponseDto>(), _localizer[SharedResourcesKeys.Success.Created]);

        }

        public async Task<Response<bool>> DeleteAsync(int addressId)
        {
           var address=await _unitOfWork.AddressRepository.GetByIdAsync(addressId);
            if(address ==null) return ResponseHandler.NotFound<bool>(_localizer[SharedResourcesKeys.Error.DataNotFound]);
            var result =await _unitOfWork.AddressRepository.RemoveAsync(address.AddressId);
            if (result == null) return ResponseHandler.BadRequest<bool>(_localizer[SharedResourcesKeys.Error.BadRequest]);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Success<bool>(true, _localizer[SharedResourcesKeys.Success.Deleted]);
        }

        public async Task<AddressResponseDto> GetByIdAsync(int addressId)
        {
            var result = await _unitOfWork.AddressRepository.GetByIdAsync(addressId);

            if (result == null)
                return null;
            
            return result.Adapt<AddressResponseDto>();
        }


        public async Task<Response<IEnumerable<AddressResponseDto>>> GetByUserIdAsync(string userId)
        {
          var addresses=await _unitOfWork.AddressRepository.GetAllAsync(x=>x.ApplicationUserId == userId);
            if (addresses == null || !addresses.Any()) return ResponseHandler.NotFound<IEnumerable<AddressResponseDto>>(_localizer[SharedResourcesKeys.Error.DataNotFound]);

            var mappedAddress = addresses.Adapt<IEnumerable<AddressResponseDto>>();
            return ResponseHandler.Success(mappedAddress);

        }

        public async Task<Response<IEnumerable<AddressResponseDto>>> GetCurrentUserAddressesAsync()
        {
           var addresses = await  _unitOfWork.AddressRepository.GetAll().Where(x => x.ApplicationUserId == _userContext.CurrentUserId).ToListAsync();
            var mappedAddress = addresses.Adapt<IEnumerable<AddressResponseDto>>();
           
            if (mappedAddress == null || !mappedAddress.Any()) return ResponseHandler.NotFound<IEnumerable<AddressResponseDto>>(_localizer[SharedResourcesKeys.Error.DataNotFound]);
            return ResponseHandler.Success(mappedAddress);
        
        }

        public async Task<Response<AddressResponseDto>> UpdateAsync(UpdateAddressDto updateAddressDto)
        {
            var address =await _unitOfWork.AddressRepository.GetByIdAsync(updateAddressDto.AddressId);
            if(address == null) return ResponseHandler.NotFound<AddressResponseDto>(_localizer[SharedResourcesKeys.Error.DataNotFound]);

           updateAddressDto.Adapt(address);
            var result=await _unitOfWork.AddressRepository.UpdateAsync(address);
            if(result == null) return ResponseHandler.BadRequest<AddressResponseDto>(_localizer[SharedResourcesKeys.Error.BadRequest]);
           
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Success<AddressResponseDto>(result.Adapt<AddressResponseDto>(), _localizer[SharedResourcesKeys.Success.Uploaded]);
        }
    }
}
