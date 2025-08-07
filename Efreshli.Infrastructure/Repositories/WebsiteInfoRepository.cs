using Efreshli.Application.Interfaces;
using Efreshli.Domain.Models;
using Efreshli.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Infrastructure.Repositories
{
    public class WebsiteInfoRepository : GenericRepository<WebsiteInfo>, IWebsiteInfoRepository
    {
        public WebsiteInfoRepository(EfreshliDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }
    }
}
