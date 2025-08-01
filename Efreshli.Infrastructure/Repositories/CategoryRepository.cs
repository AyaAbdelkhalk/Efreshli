using Efreshli.Application.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(EfreshliDbContext context) : base(context)
        {
        }
    }
}
