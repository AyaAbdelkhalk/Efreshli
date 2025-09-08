using CloudinaryDotNet;
using Efreshli.Application.DTOs.ProductDTOs.ProductColorDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductItemDto;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.File;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.ProductItemServices
{
    public class ProductItemService : IProductItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        public ProductItemService(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<Response<ProductItemResponseDto>> CreateProductItemAsync(int PrdId, CreateProductItemDto createProductItemDto)
        {
            try
            {
                //if (createProductItemDto == null || PrdId <= 0)
                //{
                //    return ResponseHandler.BadRequest<ProductItemResponseDto>("Invalid product item data or product ID.");
                //}
                var productItem = new Domain.Models.ProductItem
                {
                    ProductId = PrdId,
                    Price = createProductItemDto.Price,
                    Quantity = createProductItemDto.Quantity,
                    Discount = createProductItemDto.Discount,
                    IsPercentage = createProductItemDto.IsPercentage
                };
                await _unitOfWork.ProductItemRepository.AddAsync(productItem);
                await _unitOfWork.SaveChangesAsync();

                try
                {
                    //add fabric
                    if (createProductItemDto.FabricColorImage != null && createProductItemDto.FabricColorImage.ColorImg != null)
                    {
                        var img = await _imageService.UploadImageAsync(createProductItemDto.FabricColorImage.ColorImg, Domain.Enums.ImageReferenceType.Color, productItem.ProductItemId);
                        var fabricColor = new Domain.Models.Color
                        {
                            NameAr = createProductItemDto.FabricColorImage.NameAr,
                            NameEn = createProductItemDto.FabricColorImage.NameEn,
                            ImageId = img.Id,
                            Image = img,
                            ColorType = Domain.Enums.ColorType.FabricColor
                        };
                        await _unitOfWork.ColorRepository.AddAsync(fabricColor);
                        await _unitOfWork.SaveChangesAsync();
                        productItem.FabricColorId = fabricColor.Id;
                        productItem.FabricColor = fabricColor;
                    }
                    if (createProductItemDto.WoodColorImage != null && createProductItemDto.WoodColorImage.ColorImg != null)
                    {
                        var img = await _imageService.UploadImageAsync(createProductItemDto.WoodColorImage.ColorImg, Domain.Enums.ImageReferenceType.Color, productItem.ProductItemId);
                        var woodColor = new Domain.Models.Color
                        {
                            NameAr = createProductItemDto.WoodColorImage.NameAr,
                            NameEn = createProductItemDto.WoodColorImage.NameEn,
                            ImageId = img.Id,
                            Image = img,
                            ColorType = Domain.Enums.ColorType.WoodColor
                        };
                        await _unitOfWork.ColorRepository.AddAsync(woodColor);
                        await _unitOfWork.SaveChangesAsync();
                        productItem.WoodColorId = woodColor.Id;
                        productItem.WoodColor = woodColor;
                    }
                    //add item colors
                    if (createProductItemDto.ProductItemColors != null && createProductItemDto.ProductItemColors.Any())
                    {
                        foreach (var colorDto in createProductItemDto.ProductItemColors)
                        {
                            var img = await _imageService.UploadImageAsync(colorDto.ColorImg, Domain.Enums.ImageReferenceType.Color, productItem.ProductItemId);
                            var itemColor = new Domain.Models.Color
                            {
                                NameAr = colorDto.NameAr,
                                NameEn = colorDto.NameEn,
                                ImageId = img.Id,
                                Image = img,
                                ColorType = Domain.Enums.ColorType.GenericColor,
                                ProductItem = productItem,
                                ProductItemId = productItem.ProductItemId

                            };
                            await _unitOfWork.ColorRepository.AddAsync(itemColor);
                            productItem.ProductItemColors.Add(itemColor);
                            await _unitOfWork.SaveChangesAsync();

                        }
                        await _unitOfWork.SaveChangesAsync();

                    }

                    await _unitOfWork.ProductItemRepository.UpdateAsync(productItem);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return ResponseHandler.BadRequest<ProductItemResponseDto>($"An error occurred while uploading images or creating colors: {ex.Message}");
                }
                //var prd = await _unitOfWork.ProductRepository.GetByIdWithIncludeAsync(productItem.ProductId,
                //    includes: 

                //    );
                var responseDto = new ProductItemResponseDto
                {
                    ProductItemId = productItem.ProductItemId,
                    Price = productItem.Price,
                    Quantity = productItem.Quantity,
                    Discount = productItem.Discount,
                    IsPercentage = productItem.IsPercentage,
                    FabricColorId = productItem.FabricColorId,
                    WoodColorId = productItem.WoodColorId,
                    FabricColorImage = productItem.FabricColor != null
                        ? new ProductColorDto
                        {
                            NameAr = productItem.FabricColor.NameAr,
                            NameEn = productItem.FabricColor.NameEn,
                            ImageUrl = productItem.FabricColor.Image != null ? productItem.FabricColor.Image.URL : null
                        }
                        : null,

                    WoodColorImage = productItem.WoodColor != null
                        ? new ProductColorDto
                        {
                            NameAr = productItem.WoodColor.NameAr,
                            NameEn = productItem.WoodColor.NameEn,
                            ImageUrl = productItem.WoodColor.Image != null ? productItem.WoodColor.Image.URL : null
                        }
                        : null,

                    FinalPrice = productItem.Discount.HasValue && productItem.IsPercentage.HasValue
                        ? productItem.IsPercentage.Value
                            ? productItem.Price - (productItem.Price * (productItem.Discount.Value / 100))
                            : productItem.Price - productItem.Discount.Value
                        : productItem.Price


                };
                return ResponseHandler.Success(responseDto, "Product item created successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<ProductItemResponseDto>($"An error occurred while creating the product item: {ex.Message}");
            }

        }

        public async Task<Response<List<ProductItemResponseDto>>> GetAllProductItemsAsync(int productId)
        {
            try
            {
                var productItems = await _unitOfWork.ProductItemRepository.GetAllWithIncludeAsync(p => p.ProductId == productId,
                    includes: new Expression<Func<Domain.Models.ProductItem, object>>[]
                    {
                        p => p.FabricColor,
                        p => p.WoodColor,
                        p => p.Product
                    }
                    );
                if (productItems == null || !productItems.Any())
                {
                    return ResponseHandler.NotFound<List<ProductItemResponseDto>>("No product items found for the specified product.");
                }
                var responseDtos = productItems.Select(item => new ProductItemResponseDto
                {
                    ProductItemId = item.ProductItemId,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Discount = item.Discount,
                    IsPercentage = item.IsPercentage,
                    FabricColorId = item.FabricColorId,
                    WoodColorId = item.WoodColorId,
                    FabricColorImage = item.FabricColor != null
                        ? new ProductColorDto
                        {
                            NameAr = item.FabricColor.NameAr,
                            NameEn = item.FabricColor.NameEn,
                            ImageUrl = item.FabricColor.Image != null ? item.FabricColor.Image.URL : null
                        }
                        : null,
                    WoodColorImage = item.WoodColor != null
                        ? new ProductColorDto
                        {
                            NameAr = item.WoodColor.NameAr,
                            NameEn = item.WoodColor.NameEn,
                            ImageUrl = item.WoodColor.Image != null ? item.WoodColor.Image.URL : null
                        }
                        : null,
                    FinalPrice = item.Discount.HasValue && item.IsPercentage.HasValue
                        ? item.IsPercentage.Value
                            ? item.Price - (item.Price * (item.Discount.Value / 100))
                            : item.Price - item.Discount.Value
                        : item.Price
                }).ToList();
                return ResponseHandler.Success(responseDtos, "Product items retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<List<ProductItemResponseDto>>($"An error occurred while retrieving product items: {ex.Message}");
            }
        }



        public async Task<Response<bool>> DeleteProductItemAsync(int productItemId)
        {
            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetByIdWithThenIncludeAsync(productItemId,
                    includes: new string[]
                    {
                        "FabricColor.Image",
                        "WoodColor.Image",
                        "Product"
                    }

                    );
                if (productItem == null)
                {
                    return ResponseHandler.NotFound<bool>("Product item not found.");
                }
                if (productItem.FabricColor != null && productItem.FabricColor.ImageId != null)
                {
                    await _imageService.DeleteImageAsync(productItem.FabricColor.ImageId.Value);
                    await _unitOfWork.ColorRepository.RemoveAsync(productItem.FabricColor.Id);
                }
                if (productItem.WoodColor != null && productItem.WoodColor.ImageId != null)
                {
                    await _imageService.DeleteImageAsync(productItem.WoodColor.ImageId.Value);
                    await _unitOfWork.ColorRepository.RemoveAsync(productItem.WoodColor.Id);
                }
                await _unitOfWork.ProductItemRepository.RemoveAsync(productItem.ProductId);
                await _unitOfWork.SaveChangesAsync();
                return ResponseHandler.Success(true, "Product item deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<bool>($"An error occurred while deleting the product item: {ex.Message}");
            }
        }

        public async Task<Response<List<string>>> GetProductItemColorsUrlsAsync(int productItemId)
        {
            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetByIdWithThenIncludeAsync(productItemId,
                    includes:
                    new string[]
                    {
                        "ProductItemColors.Image"
                    }
                    );
                if (productItem == null)
                {
                    return ResponseHandler.NotFound<List<string>>("Product item not found.");
                }
                var colorUrls = new List<string>();
                if (productItem.ProductItemColors != null && productItem.ProductItemColors.Any())
                {
                    colorUrls.AddRange(productItem.ProductItemColors.Select(c => c.Image?.URL).Where(url => url != null));
                }
                return ResponseHandler.Success(colorUrls, "Product item colors URLs retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<List<string>>($"An error occurred while retrieving product item colors URLs: {ex.Message}");
            }
        }

        public async Task<Response<List<ProductItemDetailsDto>>> GetProductItemsDetailsForAdminAsync(int productId)
        {
            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetAllWithIncludeAsync(
                    p => p.ProductId == productId,
                    includes: new Expression<Func<Domain.Models.ProductItem, object>>[]
                    {
                        p => p.FabricColor,
                        p => p.WoodColor,
                        p => p.ProductItemColors,
                        p => p.ProductItemColors,
                        p => p.Product
                    }
                );
                if (productItem == null || !productItem.Any())
                {
                    return ResponseHandler.NotFound<List<ProductItemDetailsDto>>("Product item not found.");
                }
                List<ProductItemDetailsDto> responseDtos = new List<ProductItemDetailsDto>();
                foreach (var item in productItem)
                {
                    var fabricColorImage = new ProductDetailsColorDto
                    {
                        ColorId = (int)item.FabricColor?.Id,
                        NameAr = item.FabricColor?.NameAr,
                        NameEn = item.FabricColor?.NameEn,
                        ImageUrl = item.FabricColor?.Image?.URL,
                        ColorType = ColorType.FabricColor.ToString()
                    };
                    var woodColorImage = new ProductDetailsColorDto
                    {
                        ColorId = (int)item.WoodColor?.Id,
                        NameAr = item.WoodColor?.NameAr,
                        NameEn = item.WoodColor?.NameEn,
                        ImageUrl = item.WoodColor?.Image?.URL,
                        ColorType = ColorType.WoodColor.ToString()
                    };
                    var colorDetailsResponse = await GetProductItemColorsDetailsAsync(item.ProductItemId);

                    var responseDto = new ProductItemDetailsDto
                    {
                        ProductItemId = item.ProductItemId,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Discount = item.Discount,
                        IsPercentage = item.IsPercentage,
                        FinalPrice = item.Discount.HasValue && item.IsPercentage.HasValue
                            ? item.IsPercentage.Value
                                ? item.Price - (item.Price * (item.Discount.Value / 100))
                                : item.Price - item.Discount.Value
                            : item.Price,
                        FabricColorImage = fabricColorImage,
                        WoodColorImage = woodColorImage,
                        ProductItemColors = colorDetailsResponse.Data
                    };
                    responseDtos.Add(responseDto);
                }

                return ResponseHandler.Success(responseDtos, "Product item details retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<List<ProductItemDetailsDto>>($"An error occurred while retrieving product item details: {ex.Message}");
            }
        }

        public async Task<Response<List<ProductDetailsColorDto>>> GetProductItemColorsDetailsAsync(int productItemId)
        {
            try
            {
                var productItem = await _unitOfWork.ProductItemRepository.GetByIdWithThenIncludeAsync(productItemId,
                    includes: new string[]
                    {
                        "ProductItemColors.Image"
                    }
                    );
                if (productItem == null)
                {
                    return ResponseHandler.NotFound<List<ProductDetailsColorDto>>("Product item not found.");
                }
                var colorDetails = productItem?.ProductItemColors?.Select(c => new ProductDetailsColorDto
                {
                    ColorId = c.Id,
                    NameAr = c.NameAr,
                    NameEn = c.NameEn,
                    ImageUrl = c.Image?.URL,
                    ColorType = ColorType.GenericColor.ToString()
                }).ToList();
                return ResponseHandler.Success(colorDetails, "Product item colors details retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<List<ProductDetailsColorDto>>($"An error occurred while retrieving product item colors details: {ex.Message}");
            }
        }

        public async Task<Dictionary<int, List<string>>> GetProductsColorsUrlsDictionaryAsync(List<int> productIds)
        {
            if (!productIds.Any())
                return new Dictionary<int, List<string>>();

            // الطريقة الأولى: استعلام مباشر مع Include صحيح
            var productItems = await _unitOfWork.ProductItemRepository.GetAllWithIncludeAsync(
                predicate: pi => productIds.Contains(pi.ProductId),
                includes: new Expression<Func<ProductItem, object>>[]
                {
                pi => pi.ProductItemColors,  // ✅ جلب الألوان
                pi => pi.Product             // ✅ جلب المنتج للتأكد
                }
            );

            var result = new Dictionary<int, List<string>>();

            foreach (var item in productItems)
            {
                if (!result.ContainsKey(item.ProductId))
                {
                    result[item.ProductId] = new List<string>();
                }

                if (item.ProductItemColors?.Any() == true)
                {
                    foreach (var color in item.ProductItemColors)
                    {
                        // نحتاج نجيب الصورة بشكل منفصل لأن العلاقة معقدة
                        var colorWithImage = await GetColorWithImageAsync(color.Id);

                        if (colorWithImage != null)
                        {
                            result[item.ProductId].Add(colorWithImage.Image?.URL ?? string.Empty);
                        }
                    }
                }
            }

            // إزالة الألوان المكررة لكل منتج
            foreach (var productId in result.Keys.ToList())
            {
                result[productId] = result[productId].Distinct().ToList();

            }

            return result;
        }
        private async Task<Color?> GetColorWithImageAsync(int colorId)
        {
            return await _unitOfWork.ColorRepository.GetByIdWithIncludeAsync(
                id: colorId,
                includes: new Expression<Func<Color, object>>[]
                {
                c => c.Image  // ✅ جلب الصورة
                }
            );
        }
    }

}