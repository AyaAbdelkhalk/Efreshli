using Efreshli.Application.DTOs.IdentityDTOs;
using Efreshli.Application.DTOs.Review;
using Efreshli.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Mapping
{
    public static class ReviewMapping
    {
        public static void Configure()
        {
            TypeAdapterConfig<AddReviewDto,Review>.NewConfig();
            TypeAdapterConfig<UpdateReviewDto, Review>.NewConfig();
            TypeAdapterConfig<Review, ReviewResponseDto>.NewConfig();
        }
    }
}
