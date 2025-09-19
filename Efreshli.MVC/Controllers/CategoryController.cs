using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.CategoryServices;
using Efreshli.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Efreshli.MVC.Controllers
{
    [Authorize(UserRoles.Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // عرض الصفحة الرئيسية
        [HttpGet]
        public IActionResult Index()
        {
            return View("Main");
        }

        // API للحصول على جميع الكاتيجوريز
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetAllCategoriesAsync();

            if (response.Succeeded)
            {
                return Json(new { success = true, data = response.Data });
            }

            return Json(new { success = false, message = response.Message, errors = response.Errors });
        }

        // API للحصول على كاتيجوري واحد
        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);

            if (response.Succeeded)
            {
                return Json(new { success = true, data = response.Data });
            }

            return Json(new { success = false, message = response.Message });
        }

        // API لإضافة كاتيجوري جديد
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromForm] AddCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "البيانات غير صحيحة", errors = ModelState });
            }

            var response = await _categoryService.AddCategoryAsync(categoryDto);

            if (response.Succeeded)
            {
                return Json(new { success = true, data = response.Data, message = "تم إضافة الكاتيجوري بنجاح" });
            }

            return Json(new { success = false, message = response.Message, errors = response.Errors });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] UpdateCategoryDto categoryDto)
        {
            try
            {
                var result = await _categoryService.UpdateCategoryAsync(id, categoryDto);

                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Category updated successfully" });
                }

                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the category" });
            }
        }

        // API لحذف كاتيجوري
        [HttpDelete]
        [Route("Category/DeleteCategory/{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);

            if (response.Succeeded)
            {
                return Json(new { success = true, message = "تم حذف الكاتيجوري بنجاح" });
            }

            return Json(new { success = false, message = response.Message, errors = response.Errors });
        }
    }
}