using Efreshli.Application.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(EfreshliDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.AttributeValues)
                    .ThenInclude(av => av.ProductAttribute)
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.ProductItemColors)
                        .ThenInclude(pic => pic.Image)
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.FabricColor)
                        .ThenInclude(fc => fc.Image)
                .Include(p => p.ProductItems)
                .ThenInclude(pi => pi.WoodColor)
                    .ThenInclude(wc => wc.Image)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

        }
    }
}
