using Efreshli.Application.DTOs.ContactUs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Resources;
using Efreshli.Application.Services.EmailService;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Mapster;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Efreshli.Application.Resources.SharedResourcesKeys.Efreshli;
using CS= Efreshli.Domain.Models.ContactUs;
namespace Efreshli.Application.Services.ContactUsServices
{
    public class ContactUsService : IContactUsService
    {
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<CS> _contactUsRepo;
       private readonly IStringLocalizer<SharedResources> _localizer;

        public ContactUsService(IEmailService emailService,IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> localized)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _contactUsRepo = unitOfWork.ContactUsRepository;
            _localizer = localized;
        }

        public async Task<Response<string>> AddContactUsAsync(CreateContactUsDto model)
        {
            var contactUs = model.Adapt<CS>();
            var result =await _contactUsRepo.AddAsync(contactUs);
            if (result == null) ResponseHandler.BadRequest<string>(_localizer[SharedResourcesKeys.Error.ServerError]);
            
            await _emailService.SendContactUsNotificationAsync(result);
            return ResponseHandler.Success<string>(_localizer[SharedResourcesKeys.Success.Sent]);
        
        }



        public async Task<List<ContactUsDto>> GetAllContactUsAsync()
        {
            var result = await _contactUsRepo.GetAllAsync();
            if (result == null) return null;

            var mappedResult= result.Adapt<List<ContactUsDto>>();
            return mappedResult;
        }

        public async Task<ContactUsDto> GetContactUsByIdAsync(int contactUsId)
        {
            var result = await _contactUsRepo.GetByIdAsync(contactUsId);
            if (result == null) return null;
            var mappedResult=result.Adapt<ContactUsDto>();
            return mappedResult;
        }

        public async Task<bool> RemoveContactUsByIdAsync(int contactUsId)
        {
            var contactUs = await _contactUsRepo.GetByIdAsync(contactUsId);
            if(contactUs == null ) return false;
            return true;
            
        }
    }
}
