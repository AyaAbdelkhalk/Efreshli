using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.CategoryServices
{
    public interface ICategoryService
    {
        public Task<Response<GetCategoryDto>> AddCategoryAsync(AddCategoryDto category);
        public Task<Response<IEnumerable<GetCategoryDto>>> GetAllCategoriesAsync();
        public Task<Response<GetCategoryDto>> GetCategoryByIdAsync(int id);
        public Task<Response<GetCategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto category);
        public Task<Response<bool>> DeleteCategoryAsync(int id);
    }
}
