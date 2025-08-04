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
    public class BrandsRepository : GenericRepository<Brand>, IBrandsRepository
    {
        public BrandsRepository(EfreshliDbContext context) : base(context)
        {
        }
    }
}
