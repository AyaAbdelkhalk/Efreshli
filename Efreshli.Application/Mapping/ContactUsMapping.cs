using Efreshli.Application.DTOs.ContactUs;
using Efreshli.Application.DTOs.CouponDTOs;
using Efreshli.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Mapping
{
    public static class ContactUsMapping
    {
        public static void Configure()
        {
         
            TypeAdapterConfig<ContactUs, ContactUsDto>.NewConfig();
            TypeAdapterConfig<ContactUsDto, ContactUs>.NewConfig();
            

            TypeAdapterConfig<ContactUs, CreateContactUsDto>.NewConfig();
            TypeAdapterConfig<CreateContactUsDto, ContactUs>.NewConfig();
  


        }
    }
}
