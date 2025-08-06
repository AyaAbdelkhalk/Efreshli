using Efreshli.Application.DTOs.WebsiteInfoDTOs;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.WebsiteInfoServices
{
    public interface IWebsiteInfoService
    {
        Task<Response<GetWebsiteInfoDto>> AddWebsiteInfoAsync(CreateWebsiteInfoDto dto);
        Task<Response<IEnumerable<GetWebsiteInfoDto>>> GetAllWebsiteInfosAsync();
        Task<Response<GetWebsiteInfoDto>> GetWebsiteInfoByIdAsync(int id);
        Task<Response<GetWebsiteInfoDto>> UpdateWebsiteInfoAsync(int id, UpdateWebsiteInfoDto dto);
        Task<Response<bool>> DeleteWebsiteInfoAsync(int id);



    }
}
