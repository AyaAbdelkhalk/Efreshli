using Efreshli.Application.DTOs.CartDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Efreshli.Application.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<CartDto>> GetCartByUserIdAsync(string userId)
        {
            try
            {
               // userId ??= _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                var carts = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
                    predicate: c => c.ApplicationUserId == userId,
                    includes: new Expression<Func<Cart, object>>[]
                    {
                        c => c.Items // ✅ بس Items
                    }
                );
                var userCart = carts.FirstOrDefault();

                if (userCart == null)
                {
                    userCart = new Cart
                    {
                        ApplicationUserId = userId,
                        Items = new List<CartItem>()
                    };

                    await _unitOfWork.CartRepository.AddAsync(userCart, CancellationToken.None);
                    await _unitOfWork.SaveChangesAsync();

                    var saved = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
                        predicate: c => c.ApplicationUserId == userId,
                        includes: new Expression<Func<Cart, object>>[]
                        {
                            c => c.Items
                        }
                    );
                    userCart = saved.FirstOrDefault();
                }

                var dto = await MapToCartDto(userCart);
                return ResponseHandler.Success(dto);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<CartDto>($"Error retrieving cart: {ex.Message}");
            }
        }

        public async Task<Response<CartDto>> AddToCartAsync(string userId, AddToCartRequestDto request)
        {
            try
            {
                if (request.Quantity <= 0)
                    return ResponseHandler.BadRequest<CartDto>("Quantity must be greater than zero");

                var productItem = await _unitOfWork.ProductItemRepository.GetByIdWithIncludeAsync(
                    request.ProductItemId,
                    includes: new Expression<Func<ProductItem, object>>[] { pi => pi.Product }
                );

                if (productItem == null)
                    return ResponseHandler.NotFound<CartDto>("Product item not found");

                if (productItem.Quantity < request.Quantity)
                    return ResponseHandler.BadRequest<CartDto>("Insufficient product quantity available");

                var userCart = await GetOrCreateUserCartAsync(userId);
                if (userCart == null)
                    return ResponseHandler.BadRequest<CartDto>("Unable to create or retrieve user cart");

                var existing = userCart.Items?.FirstOrDefault(i => i.ProductItemId == request.ProductItemId);

                if (existing != null)
                {
                    var total = existing.RequiredQuantity + request.Quantity;
                    if (total > productItem.Quantity)
                        return ResponseHandler.BadRequest<CartDto>("Total quantity exceeds available stock");

                    existing.RequiredQuantity = total;
                    await _unitOfWork.CartItemRepository.UpdateAsync(existing, CancellationToken.None);
                }
                else
                {
                    var newItem = new CartItem
                    {
                        CartId = userCart.CartId,
                        ProductItemId = request.ProductItemId,
                        RequiredQuantity = request.Quantity
                    };
                    await _unitOfWork.CartItemRepository.AddAsync(newItem, CancellationToken.None);
                }

                await _unitOfWork.SaveChangesAsync();
                return await GetCartByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<CartDto>($"Error adding item to cart: {ex.Message}");
            }
        }

        public async Task<Response<CartDto>> UpdateCartItemQuantityAsync(string userId, UpdateCartItemQuantityRequestDto request)
        {
            try
            {
                if (request.Quantity <= 0)
                    return ResponseHandler.BadRequest<CartDto>("Quantity must be greater than zero");

                var cartItem = await _unitOfWork.CartItemRepository.GetByIdWithIncludeAsync(
                    request.CartItemId,
                    includes: new Expression<Func<CartItem, object>>[]
                    {
                        ci => ci.Cart,
                        ci => ci.ProductItem
                    }
                );

                if (cartItem == null)
                    return ResponseHandler.NotFound<CartDto>("Cart item not found");

                if (cartItem.Cart?.ApplicationUserId != userId)
                    return ResponseHandler.Unauthorized<CartDto>("Unauthorized access to cart item");

                if (cartItem.ProductItem != null && request.Quantity > cartItem.ProductItem.Quantity)
                    return ResponseHandler.BadRequest<CartDto>("Requested quantity exceeds available stock");

                cartItem.RequiredQuantity = request.Quantity;
                await _unitOfWork.CartItemRepository.UpdateAsync(cartItem, CancellationToken.None);
                await _unitOfWork.SaveChangesAsync();

                return await GetCartByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<CartDto>($"Error updating cart item: {ex.Message}");
            }
        }

        public async Task<Response<bool>> RemoveCartItemAsync(string userId, int cartItemId)
        {
            try
            {
                var cartItem = await _unitOfWork.CartItemRepository.GetByIdWithIncludeAsync(
                    cartItemId,
                    includes: new Expression<Func<CartItem, object>>[] { ci => ci.Cart }
                );

                if (cartItem == null)
                    return ResponseHandler.NotFound<bool>("Cart item not found");

                if (cartItem.Cart?.ApplicationUserId != userId)
                    return ResponseHandler.Unauthorized<bool>("Unauthorized access to cart item");

                await _unitOfWork.CartItemRepository.RemoveAsync(cartItemId, CancellationToken.None);
                await _unitOfWork.SaveChangesAsync();

                return ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<bool>($"Error removing cart item: {ex.Message}");
            }
        }

        public async Task<Response<bool>> ClearCartAsync(string userId)
        {
            try
            {
                var carts = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
                    predicate: c => c.ApplicationUserId == userId,
                    includes: new Expression<Func<Cart, object>>[] { c => c.Items }
                );

                var userCart = carts.FirstOrDefault();
                if (userCart == null)
                    return ResponseHandler.NotFound<bool>("Cart not found");

                if (userCart.Items != null && userCart.Items.Any())
                {
                    foreach (var item in userCart.Items.ToList())
                        await _unitOfWork.CartItemRepository.RemoveAsync(item.CartItemId, CancellationToken.None);
                }

                await _unitOfWork.SaveChangesAsync();
                return ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<bool>($"Error clearing cart: {ex.Message}");
            }
        }

        public async Task<Response<int>> GetCartItemsCountAsync(string userId)
        {
            try
            {
                var carts = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
                    predicate: c => c.ApplicationUserId == userId,
                    includes: new Expression<Func<Cart, object>>[] { c => c.Items }
                );

                var userCart = carts.FirstOrDefault();
                var count = userCart?.Items?.Sum(i => i.RequiredQuantity) ?? 0;

                return ResponseHandler.Success(count);
            }
            catch (Exception ex)
            {
                return ResponseHandler.BadRequest<int>($"Error getting cart items count: {ex.Message}");
            }
        }

        private async Task<Cart?> GetOrCreateUserCartAsync(string userId)
        {
            try
            {
                var carts = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
                    predicate: c => c.ApplicationUserId == userId,
                    includes: new Expression<Func<Cart, object>>[] { c => c.Items }
                );

                var userCart = carts.FirstOrDefault();

                if (userCart == null)
                {
                    userCart = new Cart
                    {
                        ApplicationUserId = userId,
                        Items = new List<CartItem>()
                    };

                    await _unitOfWork.CartRepository.AddAsync(userCart, CancellationToken.None);
                    await _unitOfWork.SaveChangesAsync();

                    var updated = await _unitOfWork.CartRepository.GetAllWithIncludeAsync(
                        predicate: c => c.ApplicationUserId == userId,
                        includes: new Expression<Func<Cart, object>>[] { c => c.Items }
                    );
                    userCart = updated.FirstOrDefault();
                }

                return userCart;
            }
            catch
            {
                return null;
            }
        }

        private async Task<CartDto> MapToCartDto(Cart cart)
        {
            var cartDto = cart.Adapt<CartDto>();

            if (cart.Items != null && cart.Items.Any())
            {
                var itemDtos = new List<CartItemDto>();

                foreach (var item in cart.Items)
                {
                    var productItem = item.ProductItem ?? await _unitOfWork.ProductItemRepository.GetByIdWithIncludeAsync(
                        item.ProductItemId,
                        includes: new Expression<Func<ProductItem, object>>[] { pi => pi.Product }
                    );

                    if (productItem != null)
                    {
                        itemDtos.Add(new CartItemDto
                        {
                            CartItemId = item.CartItemId,
                            ProductItemId = item.ProductItemId,
                            ProductName = productItem.Product?.NameEn ?? "Unknown Product",
                            Price = productItem.Price,
                            Quantity = item.RequiredQuantity,
                            TotalPrice = productItem.Price * item.RequiredQuantity
                        });
                    }
                }

                cartDto.Items = itemDtos;
                cartDto.GrandTotal = itemDtos.Sum(i => i.TotalPrice);
            }
            else
            {
                cartDto.Items = new List<CartItemDto>();
                cartDto.GrandTotal = 0;
            }

            return cartDto;
        }
    }
}
