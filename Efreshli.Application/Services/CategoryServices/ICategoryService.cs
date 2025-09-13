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
        //Admin
        public Task<Response<GetCategoryDto>> AddCategoryAsync(AddCategoryDto category);
        public Task<Response<GetCategoryDto>> GetCategoryByIdAsync(int id);
        public Task<Response<IEnumerable<GetCategoryDto>>> GetAllCategoriesAsync();
        public Task<Response<IEnumerable<GetCategoryDto>>> GetMainCategoriesAsync();
        public Task<Response<IEnumerable<GetCategoryDto>>> GetSubCategoriesAsync(int pid);
        public Task<Response<GetCategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto category);
        public Task<Response<bool>> DeleteCategoryAsync(int id);
        public Task<Response<IEnumerable<GetCategoryDto>>> GetCategoryHierarchyAsync();

        //User
        Task<Response<List<LocalizedCategoryDto>>> GetCategoryChildrenAsync(int? categoryId);



        //Task<Response<List<CategoryHeaderDto>>> GetMainCategoriesForHeaderAsync();
        //Task<Response<List<CategoryDropdownDto>>> GetSubCategoriesForDropdownAsync(int parentId);
        //Task<Response<List<CategoryHierarchyDto>>> GetFullCategoryTreeAsync();
    }
}
