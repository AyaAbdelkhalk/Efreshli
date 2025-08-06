using Efreshli.Domain.Models;


namespace Efreshli.Domain.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Image> ImageRepository { get; }

        IGenericRepository<Brand> BrandRepository { get; }
        IGenericRepository<Coupon> CouponRepository { get; }
        IGenericRepository<ApplicationUser> UserRepository { get; }
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<ProductAttribute> ProductAttributeRepository { get; }
        IGenericRepository<ProductAttributeValue> ProductAttributeValueRepository { get; }
        IGenericRepository<ProductItem> ProductItemRepository { get; }
        IGenericRepository<Color> ColorRepository { get; }
        IGenericRepository<Cart> CartRepository { get; }
        IGenericRepository<CartItem> CartItemRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<OrderItem> OrderItemRepository { get; }
        IGenericRepository<Address> AddressRepository { get; }
        IGenericRepository<Wishlist> WishlistRepository { get; }
        IGenericRepository<WishlistItem> WishlistItemRepository { get; }
        IGenericRepository<Payment> PaymentRepository { get; }
        IGenericRepository<ContactUs> ContactUsRepository { get; }
        IGenericRepository<Review> ReviewRepository { get; }
        IGenericRepository<WebsiteInfo> WebsiteInfoRepository { get; }
        IGenericRepository<VendorRequest> VendorRequestRepository { get; }




        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
