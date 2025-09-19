using Efreshli.Application.DTOs.BrandDTOs;
using Efreshli.Application.Helper.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.BrandsServices
{
    public interface IBrandsService
    {
        Task<IEnumerable<BrandResponseDto>> GetAllBrandsAsync(string? search = null, int page = 1, int pageSize = 10);
        Task<Response<IEnumerable<LocalizedBrandResponseDto>>> GetAllBrandsForUserAsync(string? search = null, int page = 1, int pageSize = 10);

        Task<BrandResponseDto?> GetBrandByIdAsync(int id);
        Task<Response<BrandResponseDto>> CreateBrandAsync(CreateBrandDto brandDto);
        Task<BrandResponseDto?> UpdateBrandAsync(int id, UpdateBrandDto brandDto);
        Task<bool> DeleteBrandAsync(int id);
    }
}
