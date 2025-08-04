using Efreshli.Application.DTOs.BrandDTOs;
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
        Task<BrandResponseDto?> GetBrandByIdAsync(int id);
        Task<BrandResponseDto> CreateBrandAsync(CreateBrandDto brandDto);
        Task<BrandResponseDto?> UpdateBrandAsync(int id, UpdateBrandDto brandDto);
        Task<bool> DeleteBrandAsync(int id);
        Task<int> GetTotalCountAsync(string? search = null);
    }
}
