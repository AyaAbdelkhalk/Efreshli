using Efreshli.Application.DTOs.BrandDTOs;
using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Helper.Mapping
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            // Add your mapping configurations here
            // For example:
            // TypeAdapterConfig<Product, ProductDto>.NewConfig()
            //     .Map(dest => dest.CategoryName, src => src.Category.Name);

            #region Category
            TypeAdapterConfig<Category, GetCategoryDto>.NewConfig();
            #endregion

            #region Brand
            TypeAdapterConfig<Brand, BrandResponseDto>.NewConfig();

            TypeAdapterConfig<CreateBrandDto, Brand>.NewConfig();

            TypeAdapterConfig<UpdateBrandDto, Brand>.NewConfig();
            TypeAdapterConfig<Brand, UpdateBrandDto>
                .NewConfig()
                .Map(dest => dest.OldImageId, src => src.ImageId);

            #endregion
        }
    }
}
