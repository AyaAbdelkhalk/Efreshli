using Efreshli.Application.DTOs.ProductDTOs;
using Efreshli.Application.DTOs.ProductDTOs.ProductItemDto;
using Efreshli.Application.Services.BrandsServices;
using Efreshli.Application.Services.CategoryServices;
using Efreshli.Application.Services.ProductItemServices;
using Efreshli.Application.Services.ProductServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Efreshli.Application.Services.ProductAttributeServices;
using System.Diagnostics;

namespace Efreshli.MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductItemService _productItemService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandsService _brandsService;
        private readonly IProductAttributeService _productAttributeService;

        public ProductController(IProductService productService, IProductItemService productItemService, IBrandsService brandsService, ICategoryService categoryService, IProductAttributeService productAttributeService)
        {
            _productService = productService;
            _productItemService = productItemService;
            _brandsService = brandsService;
            _categoryService = categoryService;
            _productAttributeService = productAttributeService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductItem(int id, [FromForm] CreateProductItemDto createProductDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _productItemService.CreateProductItemAsync(id, createProductDto);
                if (response.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", response.Message);
            }
            return View(createProductDto);
        }
        public IActionResult CreateProductItem()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            var response = await _productService.CreateProductAsync(createProductDto);
            if (response.Succeeded)
            {
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction("GetMainProducts");
            }
            ModelState.AddModelError("", response.Errors.ToString());
            return View(createProductDto);
        }
        public async Task<IActionResult> CreateProduct()
        {
            await PopulateDropdownData();
            return View();
        }
        public async Task<IActionResult> GetMainProducts(int? categoryId)
        {
            var response = await _productService.GetMainProductsAsync(categoryId);
            if (response.Succeeded)
            {
                return View("Index",response.Data);
            }
            ModelState.AddModelError("", response.Message);
            return View("Index");

           

        }

        [HttpGet]
        public async Task<IActionResult> GetProductDetailsForAdmin(int id)
        {
            var response = await _productService.GetProductDetailsForAdminAsync(id);
            if (response.Succeeded)
            {
                Console.WriteLine(response.Data);
                Debug.WriteLine(response.Data);
                //return View(response.Data);
                return View("GetProductDetails", response.Data);
            }
            ModelState.AddModelError("", response.Message);
            //return View("Error");
            Console.WriteLine(response.Data);
            Debug.WriteLine(response.Data);
            return NotFound();

        }

        [HttpGet]
        public async Task<IActionResult> GetProductDetailsById(int id)
        {
            var response = await _productService.GetAllProductsAsync();
            if (response.Succeeded)
            {
                var product = response.Data.FirstOrDefault(p => p.ProductId == id);
                if (product != null)
                {
                    return View(product);
                }
                return NotFound();
            }
            ModelState.AddModelError("", response.Message);
            return View();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            if (response)
            {
                return Ok(new { success = true });
            }
            return BadRequest(new { success = false, message = "Failed to delete product" });
        }

        private async Task PopulateDropdownData()
        {
            // Populate ViewBag with categories and brands for dropdowns
            var categories = await _categoryService.GetAllCategoriesAsync();
            var brands = await _brandsService.GetAllBrandsAsync();
            var productAttributes = _productAttributeService.GetAllAttributesAsync().Result.Data;

            ViewBag.Categories = categories.Data.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(), // Assuming Id is the value you want to submit
                Text = c.NameEn
            }).ToList();

            ViewBag.Brands = brands.Select(b => new SelectListItem
            {
                Value = b.BrandId.ToString(), // Assuming Id is the value you want to submit
                Text = b.NameEn
            }).ToList();
            ViewBag.ProductAttributes = productAttributes.Select(pa => new SelectListItem
            {
                Value = pa.ProductAttributeId.ToString(), 
                Text = pa.ProductAttributeNameEn
            }).ToList();
        }
    }
}
