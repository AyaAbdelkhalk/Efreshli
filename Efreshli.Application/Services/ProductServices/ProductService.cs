using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeValueDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductColorDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductItemDto;
using Efreshli.Application.DTOs.WishlistDTOs.WishlistItemDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.File;
using Efreshli.Application.Services.ProductAttributeValueServices;
using Efreshli.Application.Services.ProductItemServices;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Efreshli.Application.Resources.SharedResourcesKeys.Efreshli;

namespace Efreshli.Application.Services.ProductServices
{
    public class ProductService : IProductService
    {
        #region Ctor
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly IProductItemService _productItemService;
        private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly IProductRepository _productRepository;

        public ProductService(IUnitOfWork unitOfWork, IImageService imageService, IProductItemService productItemService, IProductAttributeValueService productAttributeValueService, IProductRepository productRepository)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _productItemService = productItemService;
            _productAttributeValueService = productAttributeValueService;
            _productRepository = productRepository;
        } 
        #endregion

        public async Task<Response<ProductResponseDTO>> CreateProductAsync(CreateProductDto createProductDto)
        {
            //Generate Method to create a product
            if (createProductDto == null)
            {
                return new Response<ProductResponseDTO>("Create product data is null", false);
            }
            var product = new Efreshli.Domain.Models.Product
            {
                NameAr = createProductDto.NameAr,
                NameEn = createProductDto.NameEn,
                DescriptionAr = createProductDto.DescriptionAr,
                DescriptionEn = createProductDto.DescriptionEn,
                DimensionsOrSize = createProductDto.DimensionsOrSize,
                SKU = createProductDto.SKU,
                CategoryId = createProductDto.CategoryId,
                BrandId = createProductDto.BrandId,
                Category = await _unitOfWork.CategoryRepository.GetByIdAsync(createProductDto.CategoryId),
                Brand = await _unitOfWork.BrandRepository.GetByIdAsync(createProductDto.BrandId)

            };

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            //product images
            if (createProductDto.Images?.Any() == true)
            {
                foreach (var imageFile in createProductDto.Images)
                {
                    var image = await _imageService.UploadImageAsync(imageFile, ImageReferenceType.Product, product.ProductId);
                    if (image != null)
                    {
                        product.ProductImages.Add(image);
                    }
                }
            }

            if (createProductDto.ProductAttributeValues != null)
            {
                foreach (var attrV in createProductDto.ProductAttributeValues)
                {
                    attrV.ProductId = product.ProductId;
                    Console.WriteLine($"{attrV}");
                    await _productAttributeValueService.CreateProductAttributeValueAsync(attrV);

                }
            }

            if (createProductDto.ProductItems != null)
            {
                foreach (var item in createProductDto.ProductItems)
                {
                    await _productItemService.CreateProductItemAsync(product.ProductId, item);
                }
            }


            await _unitOfWork.ProductRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            // Replace this block:
            var addedProduct = await _unitOfWork.ProductRepository.GetByIdWithIncludeAsync(
                product.ProductId,
                includes: new System.Linq.Expressions.Expression<Func<Efreshli.Domain.Models.Product, object>>[]
                {
                    p => p.Category,
                    p => p.Brand,
                    p => p.ProductImages,
                    p => p.AttributeValues,
                    p => p.ProductItems,
                }
            );

            if (addedProduct == null)
            {
                return new Response<ProductResponseDTO>("Product not found after creation", false);
            }
            //var c = addedProduct.Category;
            //var b = addedProduct.Brand;
            //var b2 = addedProduct.ProductImages;
            ////var b3 = addedProduct.AttributeValues;
            //var b4 = addedProduct.ProductItems;

            var productResponse = new ProductResponseDTO
            {
                ProductId = addedProduct.ProductId,
                NameAr = addedProduct.NameAr,
                NameEn = addedProduct.NameEn,
                DescriptionAr = addedProduct.DescriptionAr,
                DescriptionEn = addedProduct.DescriptionEn,
                DimensionsOrSize = addedProduct.DimensionsOrSize,
                SKU = addedProduct.SKU,
                CategoryNameAr = addedProduct.Category.NameAr,
                CategoryNameEn = addedProduct.Category.NameEn,
                BrandNameAr = addedProduct.Brand.NameAr,
                BrandNameEn = addedProduct.Brand.NameEn
            };
            if (addedProduct.ProductItems != null && addedProduct.ProductItems.Any())
            {
                productResponse.ProductItems = addedProduct.ProductItems.Select(pi => new ProductItemResponseDto
                {
                    ProductItemId = pi.ProductItemId,
                    Price = pi.Price,
                    Quantity = pi.Quantity,
                    Discount = pi.Discount,
                    IsPercentage = pi.IsPercentage,
                    FabricColorId = pi.FabricColorId,
                    WoodColorId = pi.WoodColorId,
                    FinalPrice = pi.IsPercentage.HasValue && pi.IsPercentage.Value
                        ? pi.Price - (pi.Price * (pi.Discount ?? 0) / 100)
                        : pi.Price - (pi.Discount ?? 0),

                }).ToList();


            }
            if (addedProduct.ProductImages != null && addedProduct.ProductImages.Any())
            {
                productResponse.ProductImageUrls = addedProduct.ProductImages.Select(img => img.URL).ToList();
            }
            if (addedProduct.AttributeValues != null && addedProduct.AttributeValues.Any())
            {
                productResponse.AttributeValues = addedProduct.AttributeValues.Select(av => new ProductAttributeValueResponseDto
                {
                    ProductAttributeValueId = av.ProductAttributeId,
                    Value = av.Value,
                    ProductAttributeNameAr = av.ProductAttribute.NameAr,
                    ProductAttributeNameEn = av.ProductAttribute.NameEn
                }).ToList();
            }


            return ResponseHandler.Created<ProductResponseDTO>(productResponse, "Product created successfully");
        }

        public async Task<Response<List<ProductResponseDTO>>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllWithIncludeAsync(
                predicate: null,
                includes: new System.Linq.Expressions.Expression<Func<Efreshli.Domain.Models.Product, object>>[]
                {
                    p => p.Category,
                    p => p.Brand,
                    p => p.ProductImages,
                    p => p.AttributeValues,
                    p => p.ProductItems
                }
            );
            if (products == null || !products.Any())
            {
                return new Response<List<ProductResponseDTO>>("No products found", false);
            }
            var prds = new List<ProductResponseDTO>();
            foreach (var product in products)
            {
                var prdResponse =await _productItemService.GetAllProductItemsAsync(product.ProductId);
                var prditems = prdResponse.Data;

                var prd = new ProductResponseDTO()
                {
                    ProductId = product.ProductId,
                    NameAr = product.NameAr,
                    NameEn = product.NameEn,
                    DescriptionAr = product.DescriptionAr,
                    DescriptionEn = product.DescriptionEn,
                    DimensionsOrSize = product.DimensionsOrSize,
                    CategoryNameAr = product.Category?.NameAr,
                    CategoryNameEn = product.Category?.NameEn,
                    BrandNameAr = product.Brand?.NameAr,
                    BrandNameEn = product.Brand?.NameEn,
                    ProductItems = prditems,
                    ProductImageUrls = product.ProductImages?.Where(img => img != null).Select(img => img.URL).ToList(),
                    AttributeValues = product.AttributeValues?.Select(av => new ProductAttributeValueResponseDto
                    {
                        ProductAttributeValueId = av.ProductAttributeId,
                        Value = av.Value,
                        ProductAttributeNameAr = _unitOfWork.ProductAttributeRepository.GetByIdAsync(av.ProductAttributeId).Result?.NameAr
                            ?? string.Empty,
                        ProductAttributeNameEn = _unitOfWork.ProductAttributeRepository.GetByIdAsync(av.ProductAttributeId).Result?.NameEn ?? string.Empty
                    }).ToList()
                };
                prds.Add(prd);

            }

            #region Old
            //var productResponses = products.Select(async p => new ProductResponseDTO
            //{
            //    ProductId = p.ProductId,
            //    NameAr = p.NameAr,
            //    NameEn = p.NameEn,
            //    DescriptionAr = p.DescriptionAr,
            //    DescriptionEn = p.DescriptionEn,
            //    DimensionsOrSize = p.DimensionsOrSize,
            //    CategoryNameAr = p.Category?.NameAr,
            //    CategoryNameEn = p.Category?.NameEn,
            //    BrandNameAr = p.Brand?.NameAr,
            //    BrandNameEn = p.Brand?.NameEn,
            //    ProductItems = await _productItemService.GetAllProductItemsAsync(p.ProductId),
            //    ProductImageUrls = p.ProductImages?.Where(img => img != null).Select(img => img.URL).ToList(),
            //    AttributeValues = p.AttributeValues?.Select(av => new ProductAttributeValueResponseDto
            //    {
            //        ProductAttributeValueId = av.ProductAttributeId,
            //        Value = av.Value,
            //        ProductAttributeNameAr = _unitOfWork.ProductAttributeRepository.GetByIdAsync(av.ProductAttributeId).Result?.NameAr
            //            ?? string.Empty,
            //        ProductAttributeNameEn = _unitOfWork.ProductAttributeRepository.GetByIdAsync(av.ProductAttributeId).Result?.NameEn ?? string.Empty
            //    }).ToList()
            //}).ToList(); 
            #endregion

            return ResponseHandler.Success<List<ProductResponseDTO>>(prds, "Products retrieved successfully");
            //return new Response<List<ProductResponseDTO>>(productResponses);
        }

        public async Task<Response<List<MainProductsDto>>> GetMainProductsAsync(int? CategoryId)
        {
            var main = new List<MainProductsDto>();

            if (CategoryId == null || CategoryId <= 0)
            {
                var products = await _unitOfWork.ProductRepository.GetAllWithIncludeAsync(
                    predicate: null,
                    includes: new System.Linq.Expressions.Expression<Func<Efreshli.Domain.Models.Product, object>>[]
                    {
                        p => p.ProductImages,
                        p => p.ProductItems,

                    }
                );
                if (products == null || !products.Any())
                {
                    return new Response<List<MainProductsDto>>("No products found for the specified category", false);
                }
                foreach (var product in products)
                {
                    var clrs = await _productItemService.GetProductItemColorsUrlsAsync(product.ProductId);
                    var mainProduct = new MainProductsDto
                    {
                        ProductId = product.ProductId,
                        NameAr = product.NameAr,
                        NameEn = product.NameEn,
                        DescriptionAr = product.DescriptionAr,
                        DescriptionEn = product.DescriptionEn,
                        DimensionsOrSize = product.DimensionsOrSize,
                        ImageUrl = product.ProductImages?.FirstOrDefault()?.URL,
                        ProductItemColorsUrls = clrs.Data,
                        Price = product.ProductItems?.FirstOrDefault()?.Price ?? 0,
                        FinalPrice = product.ProductItems?.FirstOrDefault()?.Price != null && product.ProductItems?.FirstOrDefault()?.Discount != null
                            ? (product.ProductItems.FirstOrDefault().IsPercentage.HasValue && product.ProductItems.FirstOrDefault().IsPercentage.Value
                                ? product.ProductItems.FirstOrDefault().Price - (product.ProductItems.FirstOrDefault().Price * (product.ProductItems.FirstOrDefault().Discount.Value / 100))
                                : product.ProductItems.FirstOrDefault().Price - product.ProductItems.FirstOrDefault().Discount.Value)
                            : product.ProductItems?.FirstOrDefault()?.Price
                    };
                    if (mainProduct.FinalPrice == null || mainProduct.FinalPrice <= 0)
                    {
                        mainProduct.FinalPrice = product.ProductItems?.FirstOrDefault()?.Price ?? 0;
                    }
                    
                    main.Add(mainProduct);
                }
            }
            else
            {

                var products = await _unitOfWork.ProductRepository.GetAllWithIncludeAsync(
                    predicate: p => p.CategoryId == CategoryId,
                    includes: new System.Linq.Expressions.Expression<Func<Efreshli.Domain.Models.Product, object>>[]
                    {
                    p => p.ProductImages,
                    p => p.ProductItems
                    }
                );
                if (products == null || !products.Any())
                {
                    return new Response<List<MainProductsDto>>("No products found for the specified category", false);
                }
                foreach (var product in products)
                {
                    var clrs= await _productItemService.GetProductItemColorsUrlsAsync(product.ProductId);
                    var mainProduct = new MainProductsDto
                    {
                        ProductId = product.ProductId,
                        NameAr = product.NameAr,
                        NameEn = product.NameEn,
                        DescriptionAr = product.DescriptionAr,
                        DescriptionEn = product.DescriptionEn,
                        DimensionsOrSize = product.DimensionsOrSize,
                        ProductItemColorsUrls = clrs.Data,
                        ImageUrl = product.ProductImages?.FirstOrDefault()?.URL,
                        Price = product.ProductItems?.FirstOrDefault()?.Price ?? 0,
                        FinalPrice = product.ProductItems?.FirstOrDefault()?.Price != null && product.ProductItems?.FirstOrDefault()?.Discount != null
                            ? (product.ProductItems.FirstOrDefault().IsPercentage.HasValue && product.ProductItems.FirstOrDefault().IsPercentage.Value
                                ? product.ProductItems.FirstOrDefault().Price - (product.ProductItems.FirstOrDefault().Price * (product.ProductItems.FirstOrDefault().Discount.Value / 100))
                                : product.ProductItems.FirstOrDefault().Price - product.ProductItems.FirstOrDefault().Discount.Value)
                            : product.ProductItems?.FirstOrDefault()?.Price

                    };
                    if (mainProduct.FinalPrice == null || mainProduct.FinalPrice <= 0)
                    {
                        mainProduct.FinalPrice = product.ProductItems?.FirstOrDefault()?.Price ?? 0;
                    }
                    main.Add(mainProduct);
                }
            }

            return ResponseHandler.Success<List<MainProductsDto>>(main, "Main products retrieved successfully");
        }

        #region Old
        //public async Task<Response<ProductDetailsDto>> GetProductDetailsForAdminAsync(int productId)
        //{
        //    if (productId <= 0)
        //    {
        //        return ResponseHandler.BadRequest<ProductDetailsDto>("Invalid product ID");
        //    }
        //    var product = await _unitOfWork.ProductRepository.GetByIdWithThenIncludeAsync(
        //        productId,
        //        includes: new string[]
        //        {
        //            "Category",
        //            "Brand",
        //            "ProductImages",
        //            "AttributeValues.ProductAttribute",
        //            "ProductItems"
        //        }
        //    );
        //    if (product == null)
        //    {
        //        return ResponseHandler.NotFound<ProductDetailsDto>("Product not found");
        //    }
        //    var clrs = await _productItemService.GetProductItemColorsDetailsAsync(product.ProductId);
        //    var Attv= await _productAttributeValueService.GetProductSpecificationAsync(product.ProductId);
        //    var productDetails = new ProductDetailsDto
        //    {
        //        ProductId = product.ProductId,
        //        NameAr = product.NameAr,
        //        NameEn = product.NameEn,
        //        CategoryEn = product.Category?.NameEn,
        //        CategoryAr = product.Category?.NameAr,
        //        BrandEn = product.Brand?.NameEn,
        //        BrandAr = product.Brand?.NameAr,
        //        DescriptionAr = product.DescriptionAr,
        //        DescriptionEn = product.DescriptionEn,
        //        DimensionsOrSize = product.DimensionsOrSize,
        //        SKU = product.SKU,
        //        ProductSpecificatoion = Attv.Data != null ? Attv.Data : new List<ProductAttributeValueResponseDto>(),
        //        ProductImages = product.ProductImages != null ? product.ProductImages.Select(img => img.URL).ToList() : new List<string>(),
        //        ProductItems = product.ProductItems != null ? product.ProductItems.Select(pi => new ProductItemDetailsDto
        //        {
        //            ProductItemId = pi.ProductItemId,
        //            Price = pi.Price,
        //            Quantity = pi.Quantity,
        //            Discount = pi.Discount,
        //            IsPercentage = pi.IsPercentage,
        //            FinalPrice = pi.IsPercentage.HasValue && pi.IsPercentage.Value
        //                ? pi.Price - (pi.Price * (pi.Discount ?? 0) / 100)
        //                : pi.Price - (pi.Discount ?? 0),
        //        }).ToList() : new List<ProductItemDetailsDto>(),

        //    };
        //    return ResponseHandler.Success<ProductDetailsDto>(productDetails, "Product details retrieved successfully");
        //} 
        #endregion





        public async Task<bool> DeleteProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Invalid product ID", nameof(productId));
            }
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return false; // Product not found
            }
            // Delete associated images
            foreach (var image in product.ProductImages)
            {
                await _imageService.DeleteImageAsync(image.Id);
            }
            // Delete associated product items
            var productItems = await _unitOfWork.ProductItemRepository.GetAllAsync(pi => pi.ProductId == productId);
            foreach (var item in productItems)
            {
                await _productItemService.DeleteProductItemAsync(item.ProductItemId);
            }
            // Delete associated attribute values
            var attributeValues = await _unitOfWork.ProductAttributeValueRepository.GetAllAsync(av => av.ProductId == productId);
            foreach (var attrValue in attributeValues)
            {
                await _productAttributeValueService.DeleteProductAttributeValueAsync(attrValue.Id);
            }
            //delete colors
            await _unitOfWork.ProductRepository.RemoveAsync(product.ProductId);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<Response<ProductDetailsDto>> GetProductDetailsForAdminAsync(int productId)
        {
            if (productId <= 0)
            {
                return ResponseHandler.ValidationError<ProductDetailsDto>("Invalid product ID");
            }
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return ResponseHandler.NotFound<ProductDetailsDto>("Product not found");
            }
            var Attv = await _productAttributeValueService.GetProductSpecificationAsync(product.ProductId);
            var productDetails = new ProductDetailsDto
            {
                ProductId = product.ProductId,
                NameAr = product.NameAr,
                NameEn = product.NameEn,
                CategoryEn = product.Category?.NameEn,
                CategoryAr = product.Category?.NameAr,
                BrandEn = product.Brand?.NameEn,
                BrandAr = product.Brand?.NameAr,
                DescriptionAr = product.DescriptionAr,
                DescriptionEn = product.DescriptionEn,
                DimensionsOrSize = product.DimensionsOrSize,
                SKU = product.SKU,
                ProductSpecificatoion = Attv.Data != null ? Attv.Data : new List<ProductAttributeValueResponseDto>(),
                ProductImages = product.ProductImages != null ? product.ProductImages.Select(img => img.URL).ToList() : new List<string>(),
                ProductItems = product.ProductItems != null ? product.ProductItems.Select(pi => new ProductItemDetailsDto
                {
                    ProductItemId = pi.ProductItemId,
                    Price = pi.Price,
                    Quantity = pi.Quantity,
                    Discount = pi.Discount,
                    IsPercentage = pi.IsPercentage,
                    FinalPrice = pi.IsPercentage.HasValue && pi.IsPercentage.Value
                        ? pi.Price - (pi.Price * (pi.Discount ?? 0) / 100)
                        : pi.Price - (pi.Discount ?? 0),
                    WoodColorImage = pi.WoodColor != null ? new ProductDetailsColorDto
                    {
                        ColorId = pi.WoodColor.Id,
                        NameAr = pi.WoodColor.NameAr,
                        NameEn = pi.WoodColor.NameEn,
                        ImageUrl = pi.WoodColor.Image?.URL,
                        ColorType = ColorType.WoodColor.ToString()

                    } : null,
                    FabricColorImage = pi.FabricColor != null ? new ProductDetailsColorDto
                    {
                        ColorId = pi.FabricColor.Id,
                        NameAr = pi.FabricColor.NameAr,
                        NameEn = pi.FabricColor.NameEn,
                        ImageUrl = pi.FabricColor.Image?.URL,
                        ColorType = ColorType.FabricColor.ToString()
                    } : null,
                    ProductItemColors = pi.ProductItemColors != null ? pi.ProductItemColors.Select(pic => new ProductDetailsColorDto
                    {
                        ColorId = pic.Id,
                        NameAr = pic.NameAr,
                        NameEn = pic.NameEn,
                        ImageUrl = pic.Image?.URL,
                        ColorType = ColorType.GenericColor.ToString()
                    }).ToList() : new List<ProductDetailsColorDto>()




                }).ToList() : new List<ProductItemDetailsDto>(),
            };
            return ResponseHandler.Success<ProductDetailsDto>(productDetails, "Product details retrieved successfully");
        }

        public async Task<Response<GetWishlistItemDto>> GetWishlistItemsForUserAsync(int productId)
        {
            var clrs = await _productItemService.GetProductItemColorsUrlsAsync(productId);
            var product = await _unitOfWork.ProductRepository.GetByIdWithIncludeAsync(
                productId,
                includes: new System.Linq.Expressions.Expression<Func<Efreshli.Domain.Models.Product, object>>[]
                {
                    p => p.ProductImages,
                    p => p.ProductItems,
                }
            );
            if (product == null)
            {
                return ResponseHandler.NotFound<GetWishlistItemDto>();
            }

            var wishlistItem = new GetWishlistItemDto
            {
                ProductId = product.ProductId,
                NameAr = product.NameAr,
                NameEn = product.NameEn,
                DescriptionAr = product.DescriptionAr,
                DescriptionEn = product.DescriptionEn,
                DimensionsOrSize = product.DimensionsOrSize,
                ImageUrl = product.ProductImages?.FirstOrDefault()?.URL,
                ProductItemColorsUrls = clrs.Data,
                Discount = product.ProductItems?.FirstOrDefault()?.Discount ?? 0,
                Price = product.ProductItems?.FirstOrDefault()?.Price ?? 0,
                FinalPrice = product.ProductItems?.FirstOrDefault()?.Price != null && product.ProductItems?.FirstOrDefault()?.Discount != null
                    ? (product.ProductItems.FirstOrDefault().IsPercentage.HasValue && product.ProductItems.FirstOrDefault().IsPercentage.Value
                        ? product.ProductItems.FirstOrDefault().Price - (product.ProductItems.FirstOrDefault().Price * (product.ProductItems.FirstOrDefault().Discount.Value / 100))
                        : product.ProductItems.FirstOrDefault().Price - product.ProductItems.FirstOrDefault().Discount.Value)
                    : product.ProductItems?.FirstOrDefault()?.Price
            };

            return ResponseHandler.Success(wishlistItem);
        }
    }
}
