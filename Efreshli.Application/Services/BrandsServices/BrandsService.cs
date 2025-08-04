using Efreshli.Application.DTOs.BrandDTOs;
using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.CategoryServices;
using Efreshli.Application.Services.File;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.BrandsServices
{
    public class BrandsService : IBrandsService
    {
        private readonly IBrandsRepository _brandsRepository;
        private readonly IImageService _imageService;

        public BrandsService(IBrandsRepository brandsRepository, IImageService imageService)
        {
            _brandsRepository = brandsRepository;
            _imageService = imageService;
        }

        public async Task<BrandResponseDto> CreateBrandAsync(CreateBrandDto brandDto)
        {
            var brand = brandDto.Adapt<Brand>();
            if (brandDto.BrandImage != null)
            {
                var image = await _imageService.UploadImageAsync(brandDto.BrandImage,ImageReferenceType.Brand,brand.BrandId);
                brand.ImageId = image.Id;
            }
            var createdBrand = await _brandsRepository.AddAsync(brand);
            await _brandsRepository.SaveChangesAsync();
            return new BrandResponseDto 
            {
                BrandId = createdBrand.BrandId,
                NameAr = createdBrand.NameAr,
                NameEn = createdBrand.NameEn,
                ImageId = createdBrand.ImageId
            };
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _brandsRepository.GetByIdAsync(id); 
            if (brand == null)
            {
                return false; 
            }
            await _brandsRepository.RemoveAsync(id);
            await _brandsRepository.SaveChangesAsync();
            return true; 
        }

        public async Task<IEnumerable<BrandResponseDto>> GetAllBrandsAsync(string? search = null, int page = 1, int pageSize = 10)
        {
            var brands = await _brandsRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                brands = brands.Where(b => b.NameEn.Contains(search, StringComparison.OrdinalIgnoreCase) || b.NameAr.Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            var brandDtos = brands.Select(b => b.Adapt<BrandResponseDto>());
            return brandDtos;
        }

        public async Task<BrandResponseDto?> GetBrandByIdAsync(int id)
        {
            var brand = await _brandsRepository.GetByIdAsync(id);
            if (brand == null)
            {
                return null; 
            }
            return brand.Adapt<BrandResponseDto>();

        }


        public async Task<BrandResponseDto?> UpdateBrandAsync(int id, UpdateBrandDto brandDto)
        {
            var brand = await _brandsRepository.GetByIdAsync(id);
            if (brand == null)
            {
                return null; 
            }
            brand.NameAr = brandDto.NameAr;
            brand.NameEn = brandDto.NameEn;
            if (brandDto.NewImage != null)
            {
                var newImage = await _imageService.UploadImageAsync(brandDto.NewImage, Domain.Enums.ImageReferenceType.Brand, id);
                brand.ImageId = newImage.Id;
            }
            else if (brandDto.OldImageId.HasValue)
            {
                brand.ImageId = brandDto.OldImageId.Value;
            }
            var updatedBrand = await _brandsRepository.UpdateAsync(brand);
            await _brandsRepository.SaveChangesAsync();
            return updatedBrand.Adapt<BrandResponseDto>();
        }
    }
}
