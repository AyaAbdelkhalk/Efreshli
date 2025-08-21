using Efreshli.Application.Interfaces;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EfreshliDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IGenericRepository<Category> CategoryRepository { get; private set; }
        public IGenericRepository<Image> ImageRepository { get; private set; }
        public IGenericRepository<Brand> BrandRepository { get; private set; }
        public IGenericRepository<Coupon> CouponRepository { get; private set; }
        public IGenericRepository<ApplicationUser> UserRepository { get; private set; }
        public IGenericRepository<Product> ProductRepository { get; private set; }
        public IGenericRepository<ProductAttribute> ProductAttributeRepository { get; private set; }
        public IGenericRepository<ProductAttributeValue> ProductAttributeValueRepository { get; private set; }

        public IGenericRepository<ProductItem> ProductItemRepository { get; private set; }

        public IGenericRepository<Color> ColorRepository { get; private set; }

        public IGenericRepository<Cart> CartRepository { get; private set; }

        public IGenericRepository<CartItem> CartItemRepository { get; private set; }

        public IGenericRepository<Order> OrderRepository { get; private set; }

        public IGenericRepository<OrderItem> OrderItemRepository { get; private set; }

        public IGenericRepository<Address> AddressRepository { get; private set; }

        public IGenericRepository<Wishlist> WishlistRepository { get; private set; }

        public IGenericRepository<WishlistItem> WishlistItemRepository { get; private set; }

        public IGenericRepository<Payment> PaymentRepository { get; private set; }

        public IGenericRepository<ContactUs> ContactUsRepository { get; private set; }              

        public IGenericRepository<Review> ReviewRepository { get; private set; }    

        public IGenericRepository<WebsiteInfo> WebsiteInfoRepository { get; private set; }

        public IGenericRepository<VendorRequest> VendorRequestRepository { get; private set; }

        public UnitOfWork(EfreshliDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            CategoryRepository = new GenericRepository<Category>(_context, _httpContextAccessor);
            ImageRepository = new GenericRepository<Image>(_context, _httpContextAccessor);
            BrandRepository = new GenericRepository<Brand>(_context, _httpContextAccessor);
            CouponRepository = new GenericRepository<Coupon>(_context, _httpContextAccessor);
            UserRepository = new GenericRepository<ApplicationUser>(_context, _httpContextAccessor);
            ProductRepository = new GenericRepository<Product>(_context, _httpContextAccessor);
            ProductAttributeRepository = new GenericRepository<ProductAttribute>(_context, _httpContextAccessor);
            ProductAttributeValueRepository = new GenericRepository<ProductAttributeValue>(_context, _httpContextAccessor);
            ProductItemRepository = new GenericRepository<ProductItem>(_context, _httpContextAccessor);
            ColorRepository = new GenericRepository<Color>(_context, _httpContextAccessor);
            CartRepository = new GenericRepository<Cart>(_context, _httpContextAccessor);
            CartItemRepository = new GenericRepository<CartItem>(_context, _httpContextAccessor);
            OrderRepository = new GenericRepository<Order>(_context, _httpContextAccessor);
            OrderItemRepository = new GenericRepository<OrderItem>(_context, _httpContextAccessor);
            AddressRepository = new GenericRepository<Address>(_context, _httpContextAccessor);
            WishlistRepository = new GenericRepository<Wishlist>(_context, _httpContextAccessor);
            WishlistItemRepository = new GenericRepository<WishlistItem>(_context, _httpContextAccessor);
            PaymentRepository = new GenericRepository<Payment>(_context, _httpContextAccessor);
            ContactUsRepository = new GenericRepository<ContactUs>(_context, _httpContextAccessor);
            ReviewRepository = new GenericRepository<Review>(_context, _httpContextAccessor);
            WebsiteInfoRepository = new GenericRepository<WebsiteInfo>(_context, _httpContextAccessor);
            VendorRequestRepository = new GenericRepository<VendorRequest>(_context, _httpContextAccessor);


        }








        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
        public void Dispose() => _context.Dispose();

       
    }
}