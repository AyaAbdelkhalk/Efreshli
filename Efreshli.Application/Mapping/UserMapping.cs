using Efreshli.Application.DTOs.CouponDTOs;
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
    public static class UserMapping
    {
        public static void Configure()
        {
            TypeAdapterConfig<ApplicationUser, UpdateProfileDto>.NewConfig();
            TypeAdapterConfig<UpdateProfileDto, ApplicationUser>.NewConfig();
        }
    }
}
