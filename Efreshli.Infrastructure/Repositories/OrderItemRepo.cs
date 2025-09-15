using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Repositories
{
    public class OrderItemRepo : GenericRepository<OrderItem>, IOrderItemRepo
    {
        public OrderItemRepo(EfreshliDbContext context, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {
        }
        //public async Task<List<OrderItem>> GetFrequentlyBoughtTogether(int productId)
        //{
            
        //}
    }
    public interface IOrderItemRepo
    {
        //Task<List<OrderItem>> GetFrequentlyBoughtTogether(int productId);

    }
}
