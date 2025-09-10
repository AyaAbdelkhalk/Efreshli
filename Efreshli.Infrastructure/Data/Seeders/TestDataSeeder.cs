using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Efreshli.Infrastructure.Data.Seeders
{
    public static class TestDataSeeder
    {
        public static async Task SeedTestDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EfreshliDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = scope.ServiceProvider.GetService<ILogger>();
            if (logger == null)
            {
                // Fallback to console logging if ILogger is not available
                Console.WriteLine("Logger not available, using console output");
            }

            try
            {
                // Seed Roles
                await SeedRoles(roleManager, logger);

                // Seed Users
                var testUser = await SeedUsers(userManager, logger);

                // Seed Addresses
                await SeedAddresses(context, testUser.Id, logger);

                // Seed Brands
                await SeedBrands(context, logger);

                // Seed Categories
                await SeedCategories(context, logger);

                // Save changes before seeding products
                await context.SaveChangesAsync();

                // Seed Products and Product Items
                await SeedProducts(context, logger);

                // Seed Coupons
                await SeedCoupons(context, logger);

                // Seed Cart
                await SeedCart(context, testUser.Id, logger);

                await context.SaveChangesAsync();
                if (logger != null)
                    logger.LogInformation("Test data seeding completed successfully");
                else
                    Console.WriteLine("Test data seeding completed successfully");
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.LogError(ex, "Error occurred while seeding test data");
                else
                    Console.WriteLine($"Error occurred while seeding test data: {ex.Message}");
                throw;
            }
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager, ILogger? logger)
        {
            var roles = new[] { UserRoles.Admin, UserRoles.Customer };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        if (logger != null)
                            logger.LogInformation("Created role: {Role}", role);
                        else
                            Console.WriteLine($"Created role: {role}");
                    }
                }
            }
        }

        private static async Task<ApplicationUser> SeedUsers(UserManager<ApplicationUser> userManager, ILogger? logger)
        {
            var testUser = await userManager.FindByEmailAsync("testuser@example.com");
            if (testUser == null)
            {
                testUser = new ApplicationUser
                {
                    UserName = "testuser",
                    Email = "testuser@example.com",
                    PhoneNumber = "01234567890",
                    FirstName = "Test",
                    LastName = "User",
                    EmailConfirmed = true,
                    CreatedBy = "System",
                    IsDeleted = false,
                    Role = UserRoles.Customer
                };

                var result = await userManager.CreateAsync(testUser, "TestUser@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(testUser, UserRoles.Customer);
                    if (logger != null)
                        logger.LogInformation("Created test user: {Email}", testUser.Email);
                    else
                        Console.WriteLine($"Created test user: {testUser.Email}");
                }
            }
            return testUser;
        }

        private static async Task SeedAddresses(EfreshliDbContext context, string userId, ILogger? logger)
        {
            if (!context.Addresses.Any(a => a.ApplicationUserId == userId))
            {
                var addresses = new[]
                {
                    new Address
                    {
                        ApplicationUserId = userId,
                        FullAddress = "123 Main Street, Apt 4B",
                        PhoneNumber = "01234567890",
                        Area = "Giza",
                        FloorNumber = 6,
                        IsDefault = true,
                        CreatedBy = "System"
                    },
                    new Address
                    {
                        ApplicationUserId = userId,
                        FullAddress = "456 Secondary Avenue",
                        PhoneNumber = "01098765432",
                        Area = "Giza",
                        FloorNumber = 6,
                        IsDefault = false,
                        CreatedBy = "System"
                    }
                };

                context.Addresses.AddRange(addresses);
                if (logger != null)
                    logger.LogInformation("Added {Count} test addresses", addresses.Length);
                else
                    Console.WriteLine($"Added {addresses.Length} test addresses");
            }
        }

        private static async Task SeedBrands(EfreshliDbContext context, ILogger? logger)
        {
            // Check if we already have all required brands
            var existingBrands = context.Brands.Where(b => !b.IsDeleted).Select(b => b.NameEn).ToList();
            var requiredBrands = new[] { "Samsung", "Apple", "Nike", "Adidas" };
            var missingBrands = requiredBrands.Except(existingBrands).ToList();

            if (missingBrands.Any())
            {
                var brandsToAdd = new List<Brand>();
                
                if (missingBrands.Contains("Samsung"))
                    brandsToAdd.Add(new Brand { NameAr = "سامسونج", NameEn = "Samsung", CreatedBy = "System" });
                if (missingBrands.Contains("Apple"))
                    brandsToAdd.Add(new Brand { NameAr = "آبل", NameEn = "Apple", CreatedBy = "System" });
                if (missingBrands.Contains("Nike"))
                    brandsToAdd.Add(new Brand { NameAr = "نايكي", NameEn = "Nike", CreatedBy = "System" });
                if (missingBrands.Contains("Adidas"))
                    brandsToAdd.Add(new Brand { NameAr = "أديداس", NameEn = "Adidas", CreatedBy = "System" });

                context.Brands.AddRange(brandsToAdd);
                if (logger != null)
                    logger.LogInformation("Added {Count} test brands", brandsToAdd.Count);
                else
                    Console.WriteLine($"Added {brandsToAdd.Count} test brands");
            }
        }

        private static async Task SeedCategories(EfreshliDbContext context, ILogger? logger)
        {
            // Check if we already have all required categories
            var existingCategories = context.Categories.Where(c => !c.IsDeleted).Select(c => c.NameEn).ToList();
            var requiredCategories = new[] { "Electronics", "Clothing", "Shoes", "Phones" };
            var missingCategories = requiredCategories.Except(existingCategories).ToList();

            if (missingCategories.Any())
            {
                var categoriesToAdd = new List<Category>();
                
                if (missingCategories.Contains("Electronics"))
                    categoriesToAdd.Add(new Category { NameAr = "إلكترونيات", NameEn = "Electronics", CreatedBy = "System" });
                if (missingCategories.Contains("Clothing"))
                    categoriesToAdd.Add(new Category { NameAr = "ملابس", NameEn = "Clothing", CreatedBy = "System" });
                if (missingCategories.Contains("Shoes"))
                    categoriesToAdd.Add(new Category { NameAr = "أحذية", NameEn = "Shoes", CreatedBy = "System" });
                if (missingCategories.Contains("Phones"))
                    categoriesToAdd.Add(new Category { NameAr = "هواتف", NameEn = "Phones", CreatedBy = "System" });

                context.Categories.AddRange(categoriesToAdd);
                if (logger != null)
                    logger.LogInformation("Added {Count} test categories", categoriesToAdd.Count);
                else
                    Console.WriteLine($"Added {categoriesToAdd.Count} test categories");
            }
        }

        private static async Task SeedProducts(EfreshliDbContext context, ILogger? logger)
        {
            if (!context.Products.Any())
            {
                var samsungBrand = context.Brands.FirstOrDefault(b => b.NameEn == "Samsung");
                var appleBrand = context.Brands.FirstOrDefault(b => b.NameEn == "Apple");
                var nikeBrand = context.Brands.FirstOrDefault(b => b.NameEn == "Nike");
                var adidasBrand = context.Brands.FirstOrDefault(b => b.NameEn == "Adidas");

                var electronicsCategory = context.Categories.FirstOrDefault(c => c.NameEn == "Electronics");
                var clothingCategory = context.Categories.FirstOrDefault(c => c.NameEn == "Clothing");
                var shoesCategory = context.Categories.FirstOrDefault(c => c.NameEn == "Shoes");
                var phonesCategory = context.Categories.FirstOrDefault(c => c.NameEn == "Phones");

                // Verify we have the required brands and categories
                if (samsungBrand == null || appleBrand == null || nikeBrand == null || adidasBrand == null ||
                    electronicsCategory == null || clothingCategory == null || shoesCategory == null || phonesCategory == null)
                {
                    Console.WriteLine("Missing required brands or categories for product seeding");
                    return;
                }

                var products = new[]
                {
                    // Samsung Products
                    new Product
                    {
                        NameAr = "سامسونج جالاكسي اس 23",
                        NameEn = "Samsung Galaxy S23",
                        DescriptionAr = "هاتف ذكي متطور مع كاميرا احترافية وأداء عالي",
                        DescriptionEn = "Advanced smartphone with professional camera and high performance",
                        SKU = "SAM-S23-001",
                        DimensionsOrSize = "146.3 x 70.9 x 7.6 mm",
                        BrandId = samsungBrand.BrandId,
                        CategoryId = phonesCategory.CategoryId,
                        CreatedBy = "System"
                    },
                    new Product
                    {
                        NameAr = "سامسونج جالاكسي A54",
                        NameEn = "Samsung Galaxy A54",
                        DescriptionAr = "هاتف ذكي متوسط المدى بمواصفات ممتازة",
                        DescriptionEn = "Mid-range smartphone with excellent specifications",
                        SKU = "SAM-A54-001",
                        DimensionsOrSize = "158.2 x 76.7 x 8.2 mm",
                        BrandId = samsungBrand.BrandId,
                        CategoryId = phonesCategory.CategoryId,
                        CreatedBy = "System"
                    },
                    // Apple Products
                    new Product
                    {
                        NameAr = "آيفون 15 برو",
                        NameEn = "iPhone 15 Pro",
                        DescriptionAr = "أحدث هاتف من آبل مع شريحة A17 Pro المتطورة",
                        DescriptionEn = "Latest iPhone from Apple with advanced A17 Pro chip",
                        SKU = "APL-IP15P-001",
                        DimensionsOrSize = "146.6 x 70.6 x 8.25 mm",
                        BrandId = appleBrand.BrandId,
                        CategoryId = phonesCategory.CategoryId,
                        CreatedBy = "System"
                    },
                    new Product
                    {
                        NameAr = "آيفون 14",
                        NameEn = "iPhone 14",
                        DescriptionAr = "هاتف آيفون موثوق بأداء ممتاز",
                        DescriptionEn = "Reliable iPhone with excellent performance",
                        SKU = "APL-IP14-001",
                        DimensionsOrSize = "146.7 x 71.5 x 7.8 mm",
                        BrandId = appleBrand.BrandId,
                        CategoryId = phonesCategory.CategoryId,
                        CreatedBy = "System"
                    },
                    // Nike Products
                    new Product
                    {
                        NameAr = "حذاء نايكي للجري",
                        NameEn = "Nike Running Shoes",
                        DescriptionAr = "حذاء رياضي مريح للجري والأنشطة الرياضية",
                        DescriptionEn = "Comfortable sports shoes for running and athletic activities",
                        SKU = "NIK-RUN-001",
                        DimensionsOrSize = "Available in sizes 40-46",
                        BrandId = nikeBrand.BrandId,
                        CategoryId = shoesCategory.CategoryId,
                        CreatedBy = "System"
                    },
                    new Product
                    {
                        NameAr = "تيشيرت نايكي رياضي",
                        NameEn = "Nike Sports T-Shirt",
                        DescriptionAr = "تيشيرت رياضي من القطن المريح",
                        DescriptionEn = "Comfortable cotton sports t-shirt",
                        SKU = "NIK-TSH-001",
                        DimensionsOrSize = "Available in S, M, L, XL",
                        BrandId = nikeBrand.BrandId,
                        CategoryId = clothingCategory.CategoryId,
                        CreatedBy = "System"
                    },
                    // Adidas Products
                    new Product
                    {
                        NameAr = "حذاء أديداس كلاسيك",
                        NameEn = "Adidas Classic Shoes",
                        DescriptionAr = "حذاء كلاسيكي أنيق من أديداس",
                        DescriptionEn = "Elegant classic shoes from Adidas",
                        SKU = "ADI-CLS-001",
                        DimensionsOrSize = "Available in sizes 39-47",
                        BrandId = adidasBrand.BrandId,
                        CategoryId = shoesCategory.CategoryId,
                        CreatedBy = "System"
                    },
                    new Product
                    {
                        NameAr = "بنطلون أديداس رياضي",
                        NameEn = "Adidas Sports Pants",
                        DescriptionAr = "بنطلون رياضي مريح من أديداس",
                        DescriptionEn = "Comfortable sports pants from Adidas",
                        SKU = "ADI-PNT-001",
                        DimensionsOrSize = "Available in S, M, L, XL, XXL",
                        BrandId = adidasBrand.BrandId,
                        CategoryId = clothingCategory.CategoryId,
                        CreatedBy = "System"
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                // Add Product Items with multiple variants
                var productItems = new List<ProductItem>();

                // Samsung Galaxy S23 variants
                var galaxyS23 = context.Products.FirstOrDefault(p => p.SKU == "SAM-S23-001");
                if (galaxyS23 != null)
                {
                    productItems.AddRange(new[]
                    {
                        new ProductItem { ProductId = galaxyS23.ProductId, Price = 25000, Quantity = 10, CreatedBy = "System" },
                        new ProductItem { ProductId = galaxyS23.ProductId, Price = 27000, Quantity = 8, Discount = 5, IsPercentage = true, CreatedBy = "System" }
                    });
                }

                // Samsung Galaxy A54 variants
                var galaxyA54 = context.Products.FirstOrDefault(p => p.SKU == "SAM-A54-001");
                if (galaxyA54 != null)
                {
                    productItems.AddRange(new[]
                    {
                        new ProductItem { ProductId = galaxyA54.ProductId, Price = 15000, Quantity = 15, CreatedBy = "System" },
                        new ProductItem { ProductId = galaxyA54.ProductId, Price = 16000, Quantity = 12, Discount = 1000, IsPercentage = false, CreatedBy = "System" }
                    });
                }

                // iPhone 15 Pro variants
                var iPhone15 = context.Products.FirstOrDefault(p => p.SKU == "APL-IP15P-001");
                if (iPhone15 != null)
                {
                    productItems.AddRange(new[]
                    {
                        new ProductItem { ProductId = iPhone15.ProductId, Price = 45000, Quantity = 5, CreatedBy = "System" },
                        new ProductItem { ProductId = iPhone15.ProductId, Price = 48000, Quantity = 3, Discount = 10, IsPercentage = true, CreatedBy = "System" }
                    });
                }

                // iPhone 14 variants
                var iPhone14 = context.Products.FirstOrDefault(p => p.SKU == "APL-IP14-001");
                if (iPhone14 != null)
                {
                    productItems.AddRange(new[]
                    {
                        new ProductItem { ProductId = iPhone14.ProductId, Price = 35000, Quantity = 8, CreatedBy = "System" },
                        new ProductItem { ProductId = iPhone14.ProductId, Price = 37000, Quantity = 6, Discount = 2000, IsPercentage = false, CreatedBy = "System" }
                    });
                }

                // Nike Running Shoes variants
                var nikeShoes = context.Products.FirstOrDefault(p => p.SKU == "NIK-RUN-001");
                if (nikeShoes != null)
                {
                    productItems.AddRange(new[]
                    {
                        new ProductItem { ProductId = nikeShoes.ProductId, Price = 3500, Quantity = 20, CreatedBy = "System" },
                        new ProductItem { ProductId = nikeShoes.ProductId, Price = 4000, Quantity = 15, Discount = 15, IsPercentage = true, CreatedBy = "System" }
                    });
                }

                // Nike T-Shirt variants
                var nikeTShirt = context.Products.FirstOrDefault(p => p.SKU == "NIK-TSH-001");
                if (nikeTShirt != null)
                {
                    productItems.AddRange(new[]
                    {
                        new ProductItem { ProductId = nikeTShirt.ProductId, Price = 800, Quantity = 30, CreatedBy = "System" },
                        new ProductItem { ProductId = nikeTShirt.ProductId, Price = 900, Quantity = 25, Discount = 10, IsPercentage = true, CreatedBy = "System" }
                    });
                }

                // Adidas Classic Shoes variants
                var adidasShoes = context.Products.FirstOrDefault(p => p.SKU == "ADI-CLS-001");
                if (adidasShoes != null)
                {
                    productItems.AddRange(new[]
                    {
                        new ProductItem { ProductId = adidasShoes.ProductId, Price = 4200, Quantity = 18, CreatedBy = "System" },
                        new ProductItem { ProductId = adidasShoes.ProductId, Price = 4500, Quantity = 12, Discount = 200, IsPercentage = false, CreatedBy = "System" }
                    });
                }

                // Adidas Sports Pants variants
                var adidasPants = context.Products.FirstOrDefault(p => p.SKU == "ADI-PNT-001");
                if (adidasPants != null)
                {
                    productItems.AddRange(new[]
                    {
                        new ProductItem { ProductId = adidasPants.ProductId, Price = 1500, Quantity = 25, CreatedBy = "System" },
                        new ProductItem { ProductId = adidasPants.ProductId, Price = 1700, Quantity = 20, Discount = 100, IsPercentage = false, CreatedBy = "System" }
                    });
                }

                context.ProductItems.AddRange(productItems);
                if (logger != null)
                    logger.LogInformation("Added {ProductCount} test products with {ItemCount} product items", products.Length, productItems.Count);
                else
                    Console.WriteLine($"Added {products.Length} test products with {productItems.Count} product items");
            }
        }

        private static async Task SeedCoupons(EfreshliDbContext context, ILogger? logger)
        {
            if (!context.Coupons.Any())
            {
                var coupons = new[]
                {
                    new Coupon
                    {
                        Code = "SAVE10",
                        IsActive = true,
                        DiscountValue = 10,
                        IsPercentage = true,
                        UsageLimit = 100,
                        UsedCount = 0,
                        ExpireDate = DateTime.Now.AddDays(30),
                        MinOrderAmount = 1000,
                        CreatedBy = "System"
                    },
                    new Coupon
                    {
                        Code = "FLAT500",
                        IsActive = true,
                        DiscountValue = 500,
                        IsPercentage = false,
                        UsageLimit = 50,
                        UsedCount = 0,
                        ExpireDate = DateTime.Now.AddDays(15),
                        MinOrderAmount = 5000,
                        CreatedBy = "System"
                    },
                    new Coupon
                    {
                        Code = "EXPIRED",
                        IsActive = true,
                        DiscountValue = 20,
                        IsPercentage = true,
                        UsageLimit = 10,
                        UsedCount = 0,
                        ExpireDate = DateTime.Now.AddDays(-1), // Expired coupon for testing
                        MinOrderAmount = 2000,
                        CreatedBy = "System"
                    }
                };

                context.Coupons.AddRange(coupons);
                if (logger != null)
                    logger.LogInformation("Added {Count} test coupons", coupons.Length);
                else
                    Console.WriteLine($"Added {coupons.Length} test coupons");
            }
        }

        private static async Task SeedCart(EfreshliDbContext context, string userId, ILogger? logger)
        {
            if (!context.Carts.Any(c => c.ApplicationUserId == userId))
            {
                // First ensure we have product items
                await context.SaveChangesAsync();

                var productItems = context.ProductItems.Take(6).ToList();
                if (productItems.Any())
                {
                    var cart = new Cart
                    {
                        ApplicationUserId = userId,
                        CreatedBy = "System",
                        Items = new List<CartItem>
                        {
                            new CartItem
                            {
                                ProductItemId = productItems[0].ProductItemId, // Samsung Galaxy S23
                                RequiredQuantity = 1,
                                CreatedBy = "System"
                            },
                            new CartItem
                            {
                                ProductItemId = productItems[2].ProductItemId, // iPhone 15 Pro
                                RequiredQuantity = 1,
                                CreatedBy = "System"
                            },
                            new CartItem
                            {
                                ProductItemId = productItems[4].ProductItemId, // Nike Running Shoes
                                RequiredQuantity = 2,
                                CreatedBy = "System"
                            },
                            new CartItem
                            {
                                ProductItemId = productItems[5].ProductItemId, // Nike T-Shirt
                                RequiredQuantity = 3,
                                CreatedBy = "System"
                            }
                        }
                    };

                    context.Carts.Add(cart);
                    if (logger != null)
                        logger.LogInformation("Added test cart with {Count} items for user", cart.Items.Count);
                    else
                        Console.WriteLine($"Added test cart with {cart.Items.Count} items for user");
                }
            }
        }
    }
}