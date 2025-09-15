using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.StabilityServices
{
    public interface IStabilityService
    {
        Task<IFormFile> ConvertToGlbAsync(IFormFile imageFile);
    }
}
