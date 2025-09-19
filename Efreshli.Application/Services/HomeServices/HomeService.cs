using Efreshli.Application.DTOs;
using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.FilterServices;
using Efreshli.Application.Services.ProductItemServices;
using Efreshli.Application.Services.SharedServices;
using Efreshli.Domain.Common.Classes;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static Efreshli.Application.Resources.SharedResourcesKeys;
using static Efreshli.Application.Resources.SharedResourcesKeys.Efreshli;
using ProductItem = Efreshli.Domain.Models.ProductItem;

namespace Efreshli.Application.Services.HomeServices
{
    public class HomeService : IHomeService
    {
        #region Ctor
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly ISharedService _sharedService;
        private readonly IProductItemService _productItemService;
        private readonly IFilterService _filterService;

        public HomeService(IUnitOfWork unitOfWork, IUserContext userContext, ISharedService sharedService, IProductItemService productItemService, IFilterService filterService)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _sharedService = sharedService;
            _productItemService = productItemService;
            _filterService = filterService;
        }
        #endregion

        public async Task<Response<PaginatedResult<FilteredProductsDto>>> SearchProducts(string keyword, int pageNumber, int pageSize)
        {
            var userId = _userContext.CurrentUserId;
            var lowerKeyword = keyword?.ToLower();

            //1: Use database-level pagination instead of loading all products
            var paginatedProducts = await _unitOfWork.ProductRepository.GetPagedAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,

                predicate: p => p.ProductItems.Any() &&
                p.ProductImages.Any() &&
               (string.IsNullOrEmpty(lowerKeyword) ||
                        //(p.Tags != null && p.Tags.Any(t => t != null && t.ToLower().Contains(lowerKeyword))) ||
                        (p.NameEn != null && p.NameEn.ToLower().Contains(lowerKeyword)) ||
                        (p.NameAr != null && p.NameAr.ToLower().Contains(lowerKeyword)) ||
                        (p.DescriptionEn != null && p.DescriptionEn.ToLower().Contains(lowerKeyword)) ||
                        (p.DescriptionAr != null && p.DescriptionAr.ToLower().Contains(lowerKeyword))),

                orderBy: query => query.OrderBy(p => p.NameEn), // Consistent ordering
                includes: new Expression<Func<Domain.Models.Product, object>>[]
                {
                    p => p.Category,
                    p => p.ProductItems,
                    p => p.ProductImages
                }
            );

            if (!paginatedProducts.Items.Any())
            {
                return ResponseHandler.Success(PaginatedResult<FilteredProductsDto>.Empty(pageNumber, pageSize));
            }
            //2: Get only the productIds for the current page
            var productIds = paginatedProducts.Items.Select(p => p.ProductId).ToList();

            // 3 - get colors and wishlist data in parallel
            var productColorsDict = await _productItemService.GetProductsColorsUrlsDictionaryAsync(productIds);
            var wishlistedProductIds = await GetWishlistDataAsync(userId);

            // 4: Process only the current page items
            var filteredProducts = new List<FilteredProductsDto>(paginatedProducts.Items.Count());


            foreach (var product in paginatedProducts.Items)
            {
                var productItems = product.ProductItems?.Where(pi => pi != null).ToList();
                if (!productItems?.Any() == true)
                    continue;

                var bestItem = SelectBestProductItem(productItems);
                if (bestItem == null)
                    continue;
                //5: Use TryGetValue more efficiently
                productColorsDict.TryGetValue(product.ProductId, out var colorUrls);
                var dto = new FilteredProductsDto
                {
                    ProductId = product.ProductId,
                    Name = product.GetLocalized(product.NameAr, product.NameEn) ?? string.Empty,
                    Description = product.GetLocalized(product.DescriptionAr, product.DescriptionEn) ?? string.Empty,
                    DimensionsOrSize = product.DimensionsOrSize,
                    Category = product.Category?.GetLocalized(product.Category.NameAr, product.Category.NameEn) ?? "Uncategorized",
                    CategoryId = product.CategoryId,
                    BrandId = product.BrandId,
                    Price = bestItem.Price,
                    FinalPrice = CalculateFinalPrice(bestItem),
                    ImageUrl = product.ProductImages?.FirstOrDefault()?.URL ?? string.Empty,
                    ProductItemColorsUrls = colorUrls ?? new List<string>(),
                    Discount = (decimal)(bestItem.Discount ?? 0),
                    IsWishlisted = !string.IsNullOrEmpty(userId) && wishlistedProductIds.Contains(product.ProductId)
                };
                filteredProducts.Add(dto);

            }
           
            return ResponseHandler.Success(new PaginatedResult<FilteredProductsDto>
            {
                Items = filteredProducts,
                TotalCount = paginatedProducts.TotalCount,
                PageNumber = paginatedProducts.PageNumber,
                PageSize = paginatedProducts.PageSize,
                TotalPages = paginatedProducts.TotalPages,
                HasNextPage = paginatedProducts.HasNextPage,
                HasPreviousPage = paginatedProducts.HasPreviousPage
            });
        }
        public async Task<Response<PaginatedResult<FilteredProductsDto>>> GetBrandProducts(int BrandId)
        {
            var brandIds = new List<int> { BrandId };
            return await _filterService.GetFilteredProductsAsync(new ProductFilterRequest
            {
                CategoryId = null,
                BrandIds = brandIds,
                FabricColorId = null,
                WoodColorId = null,
                MinPrice = null,
                MaxPrice = null,
                PageNumber = 1,
                PageSize = 30
            });
        }

        //public async Task<Response<PaginatedResult<FilteredProductsDto>>> FilterByColor(int pageNumber = 1, int pageSize = 30)
        //{
        //    var userId = _userContext.CurrentUserId;
        //    var paginatedProducts = await _unitOfWork.ProductRepository.GetPagedAsync(
        //        pageNumber: pageNumber,
        //        pageSize: pageSize,
        //        predicate: p => p.ProductItems.Any() && p.ProductImages.Any() && p.ProductItems.Any(pi => pi.ProductItemColors != null), // Filter at DB level
        //        orderBy: query => query.OrderBy(p => p.NameEn), // Consistent ordering
        //        includes: new Expression<Func<Domain.Models.Product, object>>[]
        //        {
        //            p => p.Category,
        //            p => p.ProductItems,
        //            p => p.ProductImages
        //        }
        //    );
        //    if (!paginatedProducts.Items.Any())
        //    {
        //        return ResponseHandler.Success(PaginatedResult<FilteredProductsDto>.Empty(pageNumber, pageSize));
        //    }
        //    //2: Get only the productIds for the current page
        //    var productIds = paginatedProducts.Items.Select(p => p.ProductId).ToList();
        //    // 3 - get colors and wishlist data in parallel
        //    var productColorsDict = await _productItemService.GetProductsColorsUrlsDictionaryAsync(productIds);
        //    var wishlistedProductIds = await GetWishlistDataAsync(userId);
        //    // 4: Process only the current page items
        //    var filteredProducts = new List<FilteredProductsDto>(paginatedProducts.Items.Count());
        //    foreach (var product in paginatedProducts.Items)
        //    {
        //        var productItems = product.ProductItems?.Where(pi => pi != null).ToList();
        //        if (!productItems?.Any() == true)
        //            continue;
        //        var bestItem = SelectBestProductItem(productItems);
        //        if (bestItem == null)
        //            continue;
        //        //5: Use TryGetValue more efficiently
        //        productColorsDict.TryGetValue(product.ProductId, out var colorUrls);
        //        var dto = new FilteredProductsDto
        //        {
        //            ProductId = product.ProductId,
        //            ProductName = product.NameEn ?? string.Empty,
        //            Description = product.DescriptionEn ?? string.Empty,
        //            DimensionsOrSize = product.DimensionsOrSize,
        //            CategoryName = product.Category?.NameEn ?? "Uncategorized",
        //            Price = bestItem.Price,
        //            FinalPrice = CalculateFinalPrice(bestItem),
        //            ImageUrl = product.ProductImages?.FirstOrDefault()?.URL ?? string.Empty,
        //            ProductItemColorsUrls = colorUrls ?? new List<string>(),
        //            Discount = (decimal)(bestItem.Discount ?? 0),
        //            IsWishlisted = !string.IsNullOrEmpty(userId) && wishlistedProductIds.Contains(product.ProductId)
        //            };
        //        filteredProducts.Add(dto);
        //    }
        //    return ResponseHandler.Success(new PaginatedResult<FilteredProductsDto>
        //    {
        //        Items = filteredProducts,
        //        TotalCount = paginatedProducts.TotalCount,
        //        PageNumber = paginatedProducts.PageNumber,
        //        PageSize = paginatedProducts.PageSize,
        //        TotalPages = paginatedProducts.TotalPages,
        //        HasNextPage = paginatedProducts.HasNextPage,
        //        HasPreviousPage = paginatedProducts.HasPreviousPage
        //    });
        //}

        #region HelperMethods
        public async Task<HashSet<int>> GetWishlistDataAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new HashSet<int>();

            var wishlistedIds = await _sharedService.GetWishlistedProductIdsAsync(userId);
            return wishlistedIds.ToHashSet();
        }
        public static ProductItem? SelectBestProductItem(IEnumerable<ProductItem> items)
        {
            if (items == null || !items.Any())
                return null;
            var itemsArray = items.ToArray();
            var discountedItems = itemsArray.Where(pi => pi.Discount.HasValue && pi.Discount.Value > 0).ToArray();

            if (discountedItems.Any())
            {
                return discountedItems
                    .OrderByDescending(pi => pi.IsPercentage == true ?
                        (pi.Price * pi.Discount!.Value / 100) :  // Actual discount amount for percentage
                        pi.Discount.Value)                        // Fixed discount amount
                    .ThenBy(pi => CalculateFinalPrice(pi))        // Then by final price
                    .FirstOrDefault();
            }

            // Fallback: No discounts, get cheapest
            return itemsArray.OrderBy(pi => pi.Price).FirstOrDefault();
        }
        public static decimal CalculateFinalPrice(ProductItem? productItem)
        {
            if (productItem?.Discount == null || productItem.Discount == 0)
                return productItem?.Price ?? 0;

            if (productItem.IsPercentage == true)
            {
                return productItem.Price - (productItem.Price * productItem.Discount.Value / 100);
            }

            return productItem.Price - productItem.Discount.Value;
        } 
        #endregion

    }
}
