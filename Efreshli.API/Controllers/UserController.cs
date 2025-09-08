using Efreshli.Application.DTOs.AddressDTOs;
using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.AddressServices;
using Efreshli.Application.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;

        public UserController(IUserService userService, IAddressService addressService)
        {
            _userService = userService;
            _addressService = addressService;
        }
        #region Profile
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            var response = await _userService.UpdateProfileAsync(updateProfileDto);
            return this.CreateResponse(response);
        } 
        #endregion

        #region Address

        [Authorize]
        [HttpPost("add-address")]
        public async Task<IActionResult> AddAddress(CreateAddressDto createAddressDto)
        {
            var result = await _addressService.CreateAsync(createAddressDto);
            return this.CreateResponse(result);
        }

        [Authorize]
        [HttpPut("update-address")]
        public async Task<IActionResult> UpdateAddress(UpdateAddressDto updateAddressDto)
        {
            var result = await _addressService.UpdateAsync(updateAddressDto);
            return this.CreateResponse(result);
        }
        [Authorize]
        [HttpDelete("{addressId:int}")]
        public async Task<IActionResult> DeleteAddress(int AddressId)
        {
            var result = await _addressService.DeleteAsync(AddressId);
            return this.CreateResponse(result);
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<IActionResult> GetAddress()
        {
            var result =await _addressService.GetCurrentUserAddressesAsync();
            return this.CreateResponse(result);
        }

        #endregion


    }
}
