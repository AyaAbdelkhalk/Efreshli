using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Services.CategoryServices;
using Efreshli.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return this.CreateResponse(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return this.CreateResponse(category);
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromForm] AddCategoryDto category)
        {
            var result = await _categoryService.AddCategoryAsync(category);
            return this.CreateResponse(result);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] UpdateCategoryDto category)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, category);
            return this.CreateResponse(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            return this.CreateResponse(result);
        }
     
    
    
    
    
    }
}
