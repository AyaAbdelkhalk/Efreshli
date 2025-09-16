using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductAttributeValueDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductColorDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductItemDto;
using Efreshli.Application.DTOs.WishlistDTOs.WishlistItemDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.File;
using Efreshli.Application.Services.FilterServices;
using Efreshli.Application.Services.HomeServices;
using Efreshli.Application.Services.ImageService;
using Efreshli.Application.Services.ProductAttributeValueServices;
using Efreshli.Application.Services.ProductItemServices;
using Efreshli.Application.Services.SharedServices;
using Efreshli.Application.Services.StabilityServices;
using Efreshli.Application.Services.WishlistServices;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISharedService _sharedService;
        private readonly IStabilityService _stabilityService;
        private readonly IUserContext _userContext;
        private readonly IFilterService _filterService;

        //private readonly IWishlistService _wishlistService;


        public ProductService(IUnitOfWork unitOfWork, IImageService imageService, IProductItemService productItemService, IProductAttributeValueService productAttributeValueService, IProductRepository productRepository, IHttpContextAccessor httpContextAccessor, ISharedService sharedService,
            IStabilityService stabilityService, IFileService fileService, IUserContext userContext, IFilterService filterService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _productItemService = productItemService;
            _productAttributeValueService = productAttributeValueService;
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
            _sharedService = sharedService;
            _stabilityService = stabilityService;
            _userContext = userContext;
            _filterService = filterService;

            //_wishlistService = wishlistService;
        }
        #endregion

        public async Task<Response<ProductResponseDTO>> CreateProductAsync(CreateProductDto createProductDto)
        {
            //Generate Method to create a product
            if (createProductDto == null)
            {
                return new Response<ProductResponseDTO>("Create product data is null", false);
            }
            var tags = new List<string>();
            if (!string.IsNullOrWhiteSpace(createProductDto.TagsInput))
            {
                tags = createProductDto.TagsInput
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim())
                            .ToList();
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
                //Tags = tags,
                Tags = createProductDto.GetTagsList(),
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
            if (createProductDto.Model_3d?.Length>0)
            {
                //var ext= Path.GetExtension(createProductDto.Model_3d.Name);
                var name= Path.GetExtension(createProductDto.Model_3d.FileName);
                if (createProductDto.Model_3d.FileName.Contains(".glb"))
                {
                   var img=await  _imageService.UploadGlbAsync(createProductDto.Model_3d, product.ProductId);
                    product.ProductImages.Add(img);

                }
                else
                {
                  var glb=await  _stabilityService.ConvertToGlbAsync(createProductDto.Model_3d);
                  var img=await _imageService.UploadGlbAsync(glb, product.ProductId);
                    product.ProductImages.Add(img);
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


            //await _unitOfWork.ProductRepository.UpdateAsync(product);
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
                BrandNameEn = addedProduct.Brand.NameEn,
                Tags = addedProduct.Tags != null ? string.Join(", ", addedProduct.Tags) : null
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
            string userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userrole = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
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
                if(userId != null && userrole!="Admin")
                {
                    var isWishlisted = await _sharedService.IsItemWishlisted(product.ProductId);
                    prd.IsWishlisted = isWishlisted.Data;
                }
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
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userrole = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);


            if (CategoryId == null || CategoryId <= 0)
            {
                var products = await _unitOfWork.ProductRepository.GetAllWithIncludeAsync(
                    predicate: null,
                    includes: new System.Linq.Expressions.Expression<Func<Efreshli.Domain.Models.Product, object>>[]
                    {
                        p => p.ProductImages,
                        p => p.ProductItems,
                        p => p.Category

                    }
                );
                if (products == null || !products.Any())
                {
                    return new Response<List<MainProductsDto>>("No products found for the specified category", false);
                }
                foreach (var product in products)
                {
                    var clrs = FilterService.GetProductColorUrls(product.ProductItems?.ToList() ?? new List<Efreshli.Domain.Models.ProductItem>());
                    var mainProduct = new MainProductsDto
                    {
                        ProductId = product.ProductId,
                        NameAr = product.NameAr,
                        NameEn = product.NameEn,
                        DescriptionAr = product.DescriptionAr,
                        DescriptionEn = product.DescriptionEn,
                        DimensionsOrSize = product.DimensionsOrSize,
                        CategoryNameAr = product.Category?.NameAr,
                        CategoryNameEn = product.Category?.NameEn,
                        Discount = product.ProductItems?.FirstOrDefault()?.Discount ?? 0,
                        ImageUrl = product.ProductImages?.FirstOrDefault()?.URL,
                        ProductItemColorsUrls = clrs,
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
                    if (userId != null)
                    {
                        var isWishlisted = await _sharedService.IsItemWishlisted(product.ProductId);
                        mainProduct.IsWishlisted = isWishlisted.Data;
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
                    p => p.ProductItems,
                    p => p.Category
                    }
                );
                if (products == null || !products.Any())
                {
                    return new Response<List<MainProductsDto>>("No products found for the specified category", false);
                }
                foreach (var product in products)
                {
                    var clrs = FilterService.GetProductColorUrls(product.ProductItems?.ToList() ?? new List<Efreshli.Domain.Models.ProductItem>());

                    var mainProduct = new MainProductsDto
                    {
                        ProductId = product.ProductId,
                        NameAr = product.NameAr,
                        NameEn = product.NameEn,
                        DescriptionAr = product.DescriptionAr,
                        DescriptionEn = product.DescriptionEn,
                        DimensionsOrSize = product.DimensionsOrSize,
                        CategoryNameAr = product.Category?.NameAr,
                        CategoryNameEn = product.Category?.NameEn,
                        ProductItemColorsUrls = clrs,
                        ImageUrl = product.ProductImages?.FirstOrDefault()?.URL,
                        Discount = product.ProductItems?.FirstOrDefault()?.Discount ?? 0,
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
                    if (userId != null)
                    {
                        var isWishlisted = await _sharedService.IsItemWishlisted(product.ProductId);
                        mainProduct.IsWishlisted = isWishlisted.Data;
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
                Tags = product.Tags ?? new List<string>(),

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

        public async Task<Response<LocalizedGetWishlistItemDto>> GetWishlistItemsForUserAsync(int productId)
        {
            var clrs = await _productItemService.GetProductItemColorsUrlsAsync(productId);
            var product = await _unitOfWork.ProductRepository.GetByIdWithIncludeAsync(
                productId,
                includes: new System.Linq.Expressions.Expression<Func<Efreshli.Domain.Models.Product, object>>[]
                {
                    p => p.ProductImages,
                    p => p.ProductItems,
                    p => p.Category
                }
            );
            if (product == null)
            {
                return ResponseHandler.NotFound<LocalizedGetWishlistItemDto>();
            }

            var wishlistItem = new LocalizedGetWishlistItemDto
            {
                ProductId = product.ProductId,
                Name = product.GetLocalized(product.NameAr, product.NameEn),
                Description = product.GetLocalized(product.DescriptionAr, product.DescriptionEn),
                DimensionsOrSize = product.DimensionsOrSize,
                Category = product.GetLocalized(product.Category?.NameAr, product.Category?.NameEn),
                ImageUrl = product.ProductImages?.FirstOrDefault()?.URL,
                ProductItemColorsUrls = clrs.Data,
                IsWishlisted = true,
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

        public async Task<Response<LocalizedProductDetailsDto>> GetProductDetailsAsync(int productId)
        {
            if (productId <= 0)
            {
                return ResponseHandler.ValidationError<LocalizedProductDetailsDto>("Invalid product ID");
            }
            var product = await _productRepository.GetProductByIdAsync(productId);

            var userId = _userContext.CurrentUserId;
            if (product != null && product.ProductItems != null && product.ProductItems.Any())
            {
                var best=HomeService.SelectBestProductItem(product.ProductItems.ToList());
                var resdto = new LocalizedProductDetailsDto
                {
                    ProductId = productId,
                    Name = product.GetLocalized(product.NameAr, product.NameEn),
                    Description = product.GetLocalized(product.DescriptionAr, product.DescriptionEn),
                    Category = product.Category != null ? product.GetLocalized(product.Category.NameAr, product.Category.NameEn) : null,
                    Brand = product.Brand != null ? product.GetLocalized(product.Brand.NameAr, product.Brand.NameEn) : null,
                    BrandId = product.BrandId,
                    CategoryId = product.CategoryId,
                    DimensionsOrSize = product.DimensionsOrSize,
                    SKU = product.SKU,
                    ProductImages = product.ProductImages?.Where(img => img != null && img.ReferenceType != ImageReferenceType.Model_3D).Select(img => img.URL).ToList() ?? new List<string>(),
                    Model_3D = product.ProductImages?.Where(img => img != null && img.ReferenceType == ImageReferenceType.Model_3D).Select(img => img.URL).FirstOrDefault(),
                    ProductSpecification = product.AttributeValues != null ? product.AttributeValues.Select(av => new LocalizedProductAttributeValueResponseDto
                    {
                        Name = av.ProductAttribute != null ? product.GetLocalized(av.ProductAttribute.NameAr, av.ProductAttribute.NameEn) : null,
                        Value = av.Value
                    }).ToList() : new List<LocalizedProductAttributeValueResponseDto>(),
                    Fabrics = product.ProductItems != null ? product.ProductItems.Where(pi => pi.FabricColor != null).Select(pi => new LocalizedColorDto
                    {
                        ColorId = pi.FabricColor.Id,
                        Name = pi.FabricColor != null ? product.GetLocalized(pi.FabricColor.NameAr, pi.FabricColor.NameEn) : null,
                        ImageUrl = pi.FabricColor.Image != null ? pi.FabricColor.Image.URL : null
                    }).DistinctBy(c => c.ColorId).ToList() : new List<LocalizedColorDto>(),
                    Woods = product.ProductItems != null ? product.ProductItems.Where(pi => pi.WoodColor != null).Select(pi => new LocalizedColorDto
                    {
                        ColorId = pi.WoodColor.Id,
                        Name = pi.WoodColor != null ? product.GetLocalized(pi.WoodColor.NameAr, pi.WoodColor.NameEn) : null,
                        ImageUrl = pi.WoodColor.Image != null ? pi.WoodColor.Image.URL : null
                    }).DistinctBy(c => c.ColorId).ToList() : new List<LocalizedColorDto>(),
                    MainPrice = best.Price,
                    MainDiscount = best.Discount ?? 0,
                    MainFinalPrice = HomeService.CalculateFinalPrice(best)


                };
                if (userId != null) 
                {
                    var isWishlisted = await _sharedService.IsItemWishlisted(product.ProductId);
                    resdto.IsWishlisted = isWishlisted.Data;
                }
                return ResponseHandler.Success(resdto);
            }
            return ResponseHandler.NotFound<LocalizedProductDetailsDto>("Product not found");
        }

        public async Task<Response<List<MainProductsDto>>> GetRecommendedProducts(int categoryId)
        {
            var res= GetMainProductsAsync(categoryId);
            if (res.Result.Data != null && res.Result.Data.Any())
            {
                var rnd = new Random();
                var recommended = res.Result.Data.OrderBy(x => rnd.Next()).Take(10).ToList();
                return ResponseHandler.Success(recommended, "Recommended products retrieved successfully");
            }
            return ResponseHandler.Success(new List<MainProductsDto>(), "No recommended products available");
        }

        public async Task<Response<PaginatedResult<FilteredProductsDto>>> GetNewArrivals(int pageNumber = 1, int pageSize = 24)
        {
            var products = _unitOfWork.ProductRepository.GetAll();
            if (products == null || !products.Any())
            {
                return ResponseHandler.Success(PaginatedResult<FilteredProductsDto>.Empty(pageNumber, pageSize), "No new arrivals found");
            }
            var allProductItems = products.SelectMany(p => p.ProductItems ?? new List<Efreshli.Domain.Models.ProductItem>()).ToList();
            var colorUrls = FilterService.GetProductColorUrls(allProductItems);

            var newArrivals = products
                .OrderByDescending(p => p.CreatedDate) // Assuming CreatedAt is a DateTime property indicating when the product was added
                .Select(p => new FilteredProductsDto
                {
                    ProductId = p.ProductId,
                    Name = p.GetLocalized(p.NameAr, p.NameEn),
                    Description = p.GetLocalized(p.DescriptionAr, p.DescriptionEn),
                    DimensionsOrSize = p.DimensionsOrSize,
                    Category = p.Category != null ? p.GetLocalized(p.Category.NameAr, p.Category.NameEn) : null,
                    CategoryId = p.CategoryId,
                    BrandId = p.BrandId,
                    ProductItemColorsUrls = colorUrls,
                    ImageUrl = p.ProductImages != null && p.ProductImages.Any() ? p.ProductImages.First().URL : null,
                    Price = p.ProductItems != null && p.ProductItems.Any() ? p.ProductItems.Min(pi => pi.Price) : 0,
                    Discount = p.ProductItems != null && p.ProductItems.Any() ? p.ProductItems.Max(pi => pi.Discount) ?? 0 : 0,
                    FinalPrice = p.ProductItems != null && p.ProductItems.Any() ? HomeService.CalculateFinalPrice(p.ProductItems.OrderBy(pi => pi.Price).First()) : 0
                });
            var paginatedResult = PaginatedResult<FilteredProductsDto>.Create(newArrivals, pageNumber, pageSize);
            return ResponseHandler.Success(paginatedResult, "New arrivals retrieved successfully");
        }

        //public async Task<Response<List<LocalizedProductInfoDto>>> GetFrequentlyBoughtTogether(int productId)
        //{
        //}

    }
}
