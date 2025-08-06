using Efreshli.Application.DTOs.WebsiteInfoDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Validators.WebsiteInfoValidators;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Mapster;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.WebsiteInfoServices
{
    public class WebsiteInfoService : IWebsiteInfoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WebsiteInfoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<GetWebsiteInfoDto>> AddWebsiteInfoAsync(CreateWebsiteInfoDto dto)
        {
            var validator = new CreateWebsiteInfoDtoValidator();
            var validationResult = validator.Validate(dto);

            if (!validationResult.IsValid)
            {
                return new Response<GetWebsiteInfoDto>
                {
                    Succeeded = false,
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                };
            }

            var entity = dto.Adapt<WebsiteInfo>();
            await _unitOfWork.WebsiteInfoRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new Response<GetWebsiteInfoDto>
            {
                Succeeded = true,
                Data = entity.Adapt<GetWebsiteInfoDto>()
            };
        }

        public async Task<Response<IEnumerable<GetWebsiteInfoDto>>> GetAllWebsiteInfosAsync()
        {
            var entities = await _unitOfWork.WebsiteInfoRepository.GetAllAsync();

            return new Response<IEnumerable<GetWebsiteInfoDto>>
            {
                Succeeded = true,
                Data = entities.Adapt<IEnumerable<GetWebsiteInfoDto>>()
            };
        }

        public async Task<Response<GetWebsiteInfoDto>> GetWebsiteInfoByIdAsync(int id)
        {
            var entity = await _unitOfWork.WebsiteInfoRepository.GetByIdAsync(id);

            if (entity == null)
            {
                return new Response<GetWebsiteInfoDto>
                {
                    Succeeded = false,
                    Errors = new List<string> { "WebsiteInfo not found" }
                };
            }

            return new Response<GetWebsiteInfoDto>
            {
                Succeeded = true,
                Data = entity.Adapt<GetWebsiteInfoDto>()
            };
        }

        public async Task<Response<GetWebsiteInfoDto>> UpdateWebsiteInfoAsync(int id, UpdateWebsiteInfoDto dto)
        {
            var validator = new UpdateWebsiteInfoDtoValidator();
            var validationResult = validator.Validate(dto);

            if (!validationResult.IsValid)
            {
                return new Response<GetWebsiteInfoDto>
                {
                    Succeeded = false,
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                };
            }

            var entity = await _unitOfWork.WebsiteInfoRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new Response<GetWebsiteInfoDto>
                {
                    Succeeded = false,
                    Errors = new List<string> { "WebsiteInfo not found" }
                };
            }

            dto.Adapt(entity);
            await _unitOfWork.WebsiteInfoRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return new Response<GetWebsiteInfoDto>
            {
                Succeeded = true,
                Data = entity.Adapt<GetWebsiteInfoDto>()
            };
        }

        public async Task<Response<bool>> DeleteWebsiteInfoAsync(int id)
        {
            var entity = await _unitOfWork.WebsiteInfoRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return new Response<bool>
                {
                    Succeeded = false,
                    Errors = new List<string> { "WebsiteInfo not found" }
                };
            }

            await _unitOfWork.WebsiteInfoRepository.RemoveAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return new Response<bool>
            {
                Succeeded = true,
                Data = true
            };
        }
    }
}
