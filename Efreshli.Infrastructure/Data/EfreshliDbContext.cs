using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Data
{
    public class EfreshliDbContext : IdentityDbContext<ApplicationUser>
    {
        public EfreshliDbContext(DbContextOptions options) : base(options)
        {
        }

        // DbSets
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WebsiteInfo> WebsiteInfos { get; set; }
        public DbSet<VendorRequest> VendorRequests { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfreshliDbContext).Assembly);
            modelBuilder.ApplyGlobalFilters();
            modelBuilder.RestrictRelation();

            // ==========================
            // Categories
            // ==========================
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, NameAr = "كراسي", NameEn = "Chairs", ParentId = null, ImageId = null },
                new Category { CategoryId = 2, NameAr = "طاولات", NameEn = "Tables", ParentId = null, ImageId = null }
            );

            // ==========================
            // Brands
            // ==========================
            modelBuilder.Entity<Brand>().HasData(
                new Brand { BrandId = 1, NameAr = "ايكيا", NameEn = "IKEA", ImageId = null },
                new Brand { BrandId = 2, NameAr = "هوم سنتر", NameEn = "Home Center", ImageId = null }
            );

            // ==========================
            // Products
            // ==========================
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    NameAr = "كرسي خشب",
                    NameEn = "Wooden Chair",
                    DescriptionAr = "كرسي خشبي قوي",
                    DescriptionEn = "Strong wooden chair",
                    DimensionsOrSize = "40x40x90",
                    CategoryId = 1,
                    BrandId = 1
                },
                new Product
                {
                    ProductId = 2,
                    NameAr = "طاولة قهوة",
                    NameEn = "Coffee Table",
                    DescriptionAr = "طاولة قهوة عصرية",
                    DescriptionEn = "Modern coffee table",
                    DimensionsOrSize = "80x50x45",
                    CategoryId = 2,
                    BrandId = 2
                }
            );

            // ==========================
            // Product Attributes
            // ==========================
            modelBuilder.Entity<ProductAttribute>().HasData(
                new ProductAttribute { Id = 1, NameAr = "اللون", NameEn = "Color", CategoryId = null },
                new ProductAttribute { Id = 2, NameAr = "المادة", NameEn = "Material", CategoryId = null }
            );

            // ==========================
            // Product Attribute Values
            // ==========================
            modelBuilder.Entity<ProductAttributeValue>().HasData(
                new ProductAttributeValue { Id = 1, ProductId = 1, ProductAttributeId = 1, Value = "بني" },
                new ProductAttributeValue { Id = 2, ProductId = 1, ProductAttributeId = 2, Value = "خشب" },
                new ProductAttributeValue { Id = 3, ProductId = 2, ProductAttributeId = 1, Value = "أسود" },
                new ProductAttributeValue { Id = 4, ProductId = 2, ProductAttributeId = 2, Value = "زجاج" }
            );

            // ==========================
            // Product Items
            // ==========================
            modelBuilder.Entity<ProductItem>().HasData(
                new ProductItem
                {
                    ProductItemId = 1,
                    Price = 1500m,
                    FabricColorId = null,
                    WoodColorId = null,
                    Discount = 10m,
                    IsPercentage = true,
                    Quantity = 20,
                    SKU = "CHAIR001",
                    ProductId = 1
                },
                new ProductItem
                {
                    ProductItemId = 2,
                    Price = 2500m,
                    FabricColorId = null,
                    WoodColorId = null,
                    Discount = null,
                    IsPercentage = null,
                    Quantity = 15,
                    SKU = "TABLE001",
                    ProductId = 2
                }
            );
            

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
