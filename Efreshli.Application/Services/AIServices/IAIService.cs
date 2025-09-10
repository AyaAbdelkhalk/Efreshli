using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.AIServices
{

    public interface IAIService
    {
        Task<List<string>> SummarizeReviewsAsync(List<string> reviews);
    }
}
