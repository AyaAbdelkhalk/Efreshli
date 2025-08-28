using Efreshli.Application.DTOs.BrandDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.CategoryServices;
using Efreshli.Application.Services.File;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        
        public async Task<Response<BrandResponseDto>> CreateBrandAsync(CreateBrandDto brandDto)
        {
            var brand = brandDto.Adapt<Brand>();
            if (brandDto.BrandImage != null)
            {
                var image = await _imageService.UploadImageAsync(brandDto.BrandImage,ImageReferenceType.Brand,brand.BrandId);
                brand.ImageId = image.Id;
                brand.Image = image;
            }
            var createdBrand = await _brandsRepository.AddAsync(brand);
            await _brandsRepository.SaveChangesAsync();
            var b = new BrandResponseDto 
            {
                BrandId = createdBrand.BrandId,
                NameAr = createdBrand.NameAr,
                NameEn = createdBrand.NameEn,
                ImageUrl = createdBrand.ImageId.HasValue ? _imageService.GetImageUrl((int)createdBrand.ImageId) : null,
                ImageId = createdBrand.ImageId,
            };
            return ResponseHandler.Success(b);
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
            return new BrandResponseDto
            {
                BrandId = brand.BrandId,
                NameAr = brand.NameAr,
                NameEn = brand.NameEn,
                ImageUrl = brand.ImageId.HasValue ? _imageService.GetImageUrl((int)brand.ImageId) : null,
                ImageId = brand.ImageId.HasValue ? brand.ImageId : null,

            };

        }

        public Task<int> GetTotalCountAsync(string? search = null)
        {
            throw new NotImplementedException();
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
            return new BrandResponseDto
            {
                BrandId = updatedBrand.BrandId,
                NameAr = updatedBrand.NameAr,
                NameEn = updatedBrand.NameEn,
                ImageUrl = updatedBrand.ImageId.HasValue ? _imageService.GetImageUrl((int)updatedBrand.ImageId) : null,
                ImageId = updatedBrand.ImageId  
            };
        }
    }
}
