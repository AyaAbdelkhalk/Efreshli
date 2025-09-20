using Efreshli.Application.DTOs;
using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.Helper.Pagination;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.HomeServices;
using Efreshli.Application.Services.SharedServices;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Efreshli.Application.Resources.SharedResourcesKeys;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Efreshli.Application.Services.FilterServices
{
    public class FilterService : IFilterService
    {
        #region Ctor
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISharedService _sharedService;
        private readonly IUserContext _userContext;

        public FilterService(IUnitOfWork unitOfWork, ISharedService sharedService, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _sharedService = sharedService;
            _userContext = userContext;
        }
        #endregion

        #region FilterBarData

        public async Task<Response<List<DropDownDto>>> GetCategories()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAll()
                .Where(c => c.ParentId == null || c.ParentId == 511)
                .ToListAsync();
            if (categories == null || !categories.Any())
            {
                return ResponseHandler.NotFound<List<DropDownDto>>();
            }
            var result = categories.Select(c => new DropDownDto
            {
                Id = c.CategoryId,
                Name = c.GetLocalized(c.NameAr, c.NameEn) ?? string.Empty
            })
            .ToList();
            return ResponseHandler.Success(result);
        }

        public async Task<Response<List<DropDownDto>>> GetBrandsByCategoryId(int? categoryId)
        {
            var query = _unitOfWork.ProductRepository.GetAll()
                    .Include(p => p.Brand)
                    .Where(p => p.Brand != null);

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var brands = await query
                .GroupBy(p => new { p.Brand.BrandId, p.Brand.NameEn })
                .Select(g => new DropDownDto
                {
                    Id = g.Key.BrandId,
                    Name = g.Key.NameEn,
                    Count = g.Count()
                })
                .OrderBy(b => b.Name) 
                .ToListAsync();

            return ResponseHandler.Success(brands);
        }

        public async Task<Response<List<ColorsDropDownDto>>> GetFabricColorsByCategoryId(int? categoryId)
        {
            var query = _unitOfWork.ProductRepository.GetAll();

            // Filter by category only if categoryId has a value and is greater than 0
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var colors = await query
                   .SelectMany(p => p.ProductItems)
                   .Where(pi => pi.FabricColorId != null ) // ✅ إضافة null check على الصورة
                   .Include(pi => pi.FabricColor)
                   .ThenInclude(fc => fc.Image)
                   .GroupBy(pi => new
                   {
                       pi.FabricColorId,
                       pi.FabricColor.Image.URL
                   })
                   .Select(g => new ColorsDropDownDto
                   {
                       ColorId = (int)g.Key.FabricColorId,
                       ImageUrl = g.Key.URL
                   })
               .ToListAsync();

            return ResponseHandler.Success(colors);
        }

        public async Task<Response<List<ColorsDropDownDto>>> GetWoodColorsByCategoryId(int? categoryId)
        {
            var query = _unitOfWork.ProductRepository.GetAll();
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
            var colors = await query
                .SelectMany(p => p.ProductItems)
                .Where(pi => pi.WoodColorId != null) // ✅ نفس الفكرة هنا
                .Include(pi => pi.WoodColor)
                .ThenInclude(wc => wc.Image)
                .GroupBy(pi => new
                {
                    pi.WoodColorId,
                    pi.WoodColor.Image.URL
                })
                .Select(g => new ColorsDropDownDto
                {
                    ColorId = (int)g.Key.WoodColorId,
                    ImageUrl = g.Key.URL,
                })
                .ToListAsync();


            return ResponseHandler.Success(colors);
        }
        #endregion

        public async Task<Response<PaginatedResult<FilteredProductsDto>>> GetFilteredProductsAsync(ProductFilterRequest request)
        {
            try
             {
                var query = _unitOfWork.ProductRepository.GetAll()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.ProductItems.Where(pi => pi != null))
                        .ThenInclude(pi => pi.FabricColor)
                        .ThenInclude(fc => fc.Image)
                    .Include(p => p.ProductItems)
                        .ThenInclude(pi => pi.WoodColor)
                        .ThenInclude(wc => wc.Image)
                    .Include(p => p.ProductItems)
                        .ThenInclude(pi => pi.ProductItemColors)
                        .ThenInclude(pic => pic.Image)
                    .Include(p => p.ProductImages.Where(img => img != null))
                    .Where(p => p.ProductItems.Any())
                    .AsQueryable();

                query = ApplyFilters(query, request.CategoryId, request.BrandIds, request.FabricColorId, request.WoodColorId, request.MinPrice, request.MaxPrice);

                query = ApplySorting(query, request.SortBy);

                var totalCount = await query.CountAsync();
                if (totalCount == 0)
                {
                    return ResponseHandler.Success(
                        PaginatedResult<FilteredProductsDto>.Empty(request.PageNumber, request.PageSize),
                        "No products found matching the criteria.");
                }

                //var products = await query
                //    .OrderBy(p => p.NameEn)
                //    .Skip((pageNumber - 1) * pageSize)
                //    .Take(pageSize)
                //    .ToListAsync();
                var products = await query.ToPagedList(request.PageNumber, request.PageSize);

                var productIds = products.Select(p => p.ProductId).ToList();
                var wishlistedProductIds = await GetWishlistedProductIdsBatchAsync(productIds);
                var userId = _userContext.CurrentUserId;

                var filteredProducts = ProcessProducts(products, wishlistedProductIds);

                var paginatedResult = new PaginatedResult<FilteredProductsDto>
                {
                    Items = filteredProducts,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                    HasNextPage = request.PageNumber < (int)Math.Ceiling((double)totalCount / request.PageSize),
                    HasPreviousPage = request.PageNumber > 1
                };

                return ResponseHandler.Success(paginatedResult);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<PaginatedResult<FilteredProductsDto>>(
                    $"Error filtering products: {ex.Message}");
            }
        }

        #region Private Helper Methods

        private static IQueryable<Product> ApplyFilters(
            IQueryable<Product> query,
            int? categoryId,
            List<int>? brandIds,
            int? fabricColorId,
            int? woodColorId,
            decimal? fromPrice,
            decimal? toPrice)
        {
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (brandIds?.Any() == true)
            {
                query = query.Where(p => brandIds.Contains(p.BrandId)); 
            }

            if (fabricColorId.HasValue)
            {
                query = query.Where(p => p.ProductItems.Any(pi => pi.FabricColorId == fabricColorId.Value));
            }

            if (woodColorId.HasValue)
            {
                query = query.Where(p => p.ProductItems.Any(pi => pi.WoodColorId == woodColorId.Value));
            }

            if (fromPrice.HasValue || toPrice.HasValue)
            {
                query = ApplyPriceFilter(query, fromPrice, toPrice);
            }




            return query;
        }

        private static IQueryable<Product> ApplyPriceFilter(
            IQueryable<Product> query,
            decimal? fromPrice,
            decimal? toPrice)
        {
            if (fromPrice.HasValue && toPrice.HasValue)
            {
                query = query.Where(p => p.ProductItems.Any(pi =>
                    pi.Price >= fromPrice.Value && pi.Price <= toPrice.Value));
            }
            else if (fromPrice.HasValue)
            {
                query = query.Where(p => p.ProductItems.Any(pi => pi.Price >= fromPrice.Value));
            }
            else if (toPrice.HasValue)
            {
                query = query.Where(p => p.ProductItems.Any(pi => pi.Price <= toPrice.Value));
            }

            return query;
        }

        private async Task<HashSet<int>> GetWishlistedProductIdsBatchAsync(List<int> productIds)
        {
            try
            {
                var result = await _sharedService.GetWishlistedProductIds(productIds);

                if (result.Succeeded && result.Data != null)
                {
                    return result.Data.ToHashSet();
                }

                return new HashSet<int>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wishlist error: {ex.Message}");
                return new HashSet<int>();
            }
        }

        private List<FilteredProductsDto> ProcessProducts(
            List<Product> products,
            HashSet<int> wishlistedProductIds)
        {
            var filteredProducts = new List<FilteredProductsDto>(products.Count);

            foreach (var product in products)
            {
                var productItems = product.ProductItems?.Where(pi => pi != null).ToList();
                if (productItems?.Any() != true)
                    continue;

                var bestItem = HomeService.SelectBestProductItem(productItems);
                if (bestItem == null)
                    continue;

                var finalPrice = HomeService.CalculateFinalPrice(bestItem);
                var colorUrls = GetProductColorUrls(productItems);

                var dto = new FilteredProductsDto
                {
                    ProductId = product.ProductId,
                    Name = product.GetLocalized(product.NameAr, product.NameEn) ?? string.Empty,
                    Description = product.GetLocalized(product.DescriptionAr, product.DescriptionEn) ?? string.Empty,
                    DimensionsOrSize = product.DimensionsOrSize,
                    Category = product.Category?.GetLocalized(product.Category.NameAr, product.Category.NameEn) ?? string.Empty,
                    CategoryId = product.CategoryId,
                    BrandId = product.BrandId,
                    Price = bestItem.Price,
                    FinalPrice = finalPrice,
                    ImageUrl = product.ProductImages?.FirstOrDefault()?.URL ?? string.Empty,
                    ProductItemColorsUrls = colorUrls,
                    Discount = (decimal)(bestItem.Discount ?? 0), 
                    IsWishlisted = wishlistedProductIds.Contains(product.ProductId)
                };

                filteredProducts.Add(dto);
            }

            return filteredProducts;
        }

        public static List<string> GetProductColorUrls(List<ProductItem> productItems)
        {
            var colorUrls = new HashSet<string>();

            foreach (var item in productItems)
            {

                if (item.ProductItemColors?.Any() == true)
                {
                    var itemColorUrls = item.ProductItemColors
                        .Where(pic => !string.IsNullOrEmpty(pic.Image?.URL))
                        .Select(pic => pic.Image.URL);

                    foreach (var url in itemColorUrls)
                    {
                        colorUrls.Add(url);
                    }
                }
            }

            return colorUrls.ToList();
        }

        private static IQueryable<Product> ApplySorting(IQueryable<Product> query, ProductSortBy sortBy)
        {
            return sortBy switch
            {
                ProductSortBy.LatestProducts => query.OrderByDescending(p => p.CreatedDate),

                ProductSortBy.PriceHighToLow => query.OrderByDescending(p =>
                    p.ProductItems.Where(pi => pi != null).Any() ?
                    p.ProductItems.Where(pi => pi != null).FirstOrDefault().Price : 0),

                ProductSortBy.PriceLowToHigh => query.OrderBy(p =>
                    p.ProductItems.Where(pi => pi != null).Any() ?
                    p.ProductItems.Where(pi => pi != null).FirstOrDefault().Price : decimal.MaxValue),

                ProductSortBy.Recommended => query
                    // أولاً: المنتجات الحديثة (آخر 30 يوم لها أولوية)
                   .OrderByDescending(p => p.CreatedDate > DateTime.Now.AddDays(-30))
                   // ثانياً: المنتجات ذات التقييمات العالية
                   .ThenByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rate) : 0)
                   // ثالثاً: المنتجات الأكثر مبيعاً (إذا كان لديك حقل Sales)
                   .ThenByDescending(p => p.ProductItems.Sum(pi => pi.Quantity))
                   // رابعاً: المنتجات ذات الخصومات
                   .ThenByDescending(p => p.ProductItems.Any(pi => pi.Discount > 0))
                   // خامساً: الأسعار المنخفضة
                   .ThenBy(p => p.ProductItems.Any(pi => pi != null) ?
                               p.ProductItems.Where(pi => pi != null).FirstOrDefault().Price : decimal.MaxValue)
                   // أخيراً: الأحدث
                   .ThenByDescending(p => p.CreatedDate),


                _ => query.OrderByDescending(p => p.CreatedDate)
            };
        }

        #endregion

    }
}
