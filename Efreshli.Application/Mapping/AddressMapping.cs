using Efreshli.Application.DTOs.AddressDTOs;
using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Mapping
{
    public class AddressMapping
    {
        public AddressMapping()
        {
            // from Entity to DTOs
            TypeAdapterConfig<Address, CreateAddressDto>.NewConfig();
            TypeAdapterConfig<Address, UpdateAddressDto>.NewConfig();
            TypeAdapterConfig<Address, AddressResponseDto>.NewConfig();
            // from DTOs to Enrity
            TypeAdapterConfig< CreateAddressDto,Address>.NewConfig();
            TypeAdapterConfig< UpdateAddressDto,Address>.NewConfig();
            TypeAdapterConfig<AddressResponseDto, Address>.NewConfig();

        }
    }
}
