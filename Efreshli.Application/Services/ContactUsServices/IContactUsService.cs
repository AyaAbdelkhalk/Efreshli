using Efreshli.Application.DTOs.ContactUs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ContactUsServices
{
    public interface IContactUsService
    {
        Task<Response<string>> AddContactUsAsync(CreateContactUsDto model);
        Task<bool> RemoveContactUsByIdAsync(int contactUsId);
        Task<ContactUsDto> GetContactUsByIdAsync(int contactUsId);
        Task<List<ContactUsDto>> GetAllContactUsAsync();
    }
}
