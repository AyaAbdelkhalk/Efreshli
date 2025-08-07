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
    public class CouponRepository:GenericRepository<Coupon>,ICouponRepository
    {
        public CouponRepository(EfreshliDbContext context, IHttpContextAccessor httpContextAccessor) : base(context,httpContextAccessor) { }
        public async Task<Coupon> GetByCodeAsync(string code)
        {
            return await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
        }
       
    }
}
