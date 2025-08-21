using Efreshli.Application.DTOs.BrandDTOs;
using Efreshli.Application.DTOs.CartDTOs;
using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.DTOs.WebsiteInfoDTOs;
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
            TypeAdapterConfig<Category, GetCategoryDto>.NewConfig()
                .Map(dest => dest.ImageUrl, src => src.Image.URL)
                .Map(destinationMember => destinationMember.ImageUrl, src => src.Image.URL)
                .Map(destinationMember => destinationMember.ParentId, src => src.ParentId);
            TypeAdapterConfig<AddCategoryDto,Category>.NewConfig();
            TypeAdapterConfig<UpdateCategoryDto, Category>.NewConfig();
            #endregion

            #region Brand
            TypeAdapterConfig<Brand, BrandResponseDto>.NewConfig();

            TypeAdapterConfig<CreateBrandDto, Brand>.NewConfig();

            TypeAdapterConfig<UpdateBrandDto, Brand>.NewConfig();
            TypeAdapterConfig<Brand, UpdateBrandDto>
                .NewConfig()
                .Map(dest => dest.OldImageId, src => src.ImageId);

            #endregion

            #region WebsiteInfo
            TypeAdapterConfig<WebsiteInfo, UpdateWebsiteInfoDto>.NewConfig();
            TypeAdapterConfig<UpdateWebsiteInfoDto, WebsiteInfo>.NewConfig();

            TypeAdapterConfig<CreateWebsiteInfoDto, WebsiteInfo>.NewConfig();
            TypeAdapterConfig<WebsiteInfo, CreateWebsiteInfoDto>.NewConfig();

            TypeAdapterConfig<WebsiteInfo, GetWebsiteInfoDto>.NewConfig();

            #endregion

            #region Cart
            TypeAdapterConfig<Cart, CartDto>.NewConfig()
                .Map(dest => dest.Items, src => src.Items);
            TypeAdapterConfig<CartItem, CartItemDto>.NewConfig();
            #endregion

        }
    }
}
