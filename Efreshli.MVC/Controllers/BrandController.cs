using Microsoft.AspNetCore.Mvc;
using Efreshli.MVC.Models;
using Efreshli.Application.Services.BrandsServices;
using Efreshli.Application.DTOs.BrandDTOs;

namespace Efreshli.MVC.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandsService _brandsService;
        private readonly ILogger<BrandController> _logger;

        public BrandController(IBrandsService brandsService, ILogger<BrandController> logger)
        {
            _brandsService = brandsService;
            _logger = logger;
        }

        // GET: Brand
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
        {
            try
            {
                var brands = await _brandsService.GetAllBrandsAsync(search, page, pageSize);

                var viewModel = new BrandListViewModel
                {
                    Brands = brands.Select(b => new BrandDisplayViewModel
                    {
                        BrandId = b.BrandId,
                        NameAr = b.NameAr,
                        NameEn = b.NameEn,
                        ImageId = b.ImageId
                    }),
                    SearchTerm = search,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = brands.Count() // This should be improved with proper pagination
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching brands");
                TempData["Error"] = "An error occurred while loading brands";
                return View(new BrandListViewModel());
            }
        }

        // GET: Brand/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var brand = await _brandsService.GetBrandByIdAsync(id.Value);
                if (brand == null)
                {
                    return NotFound();
                }

                var viewModel = new BrandDisplayViewModel
                {
                    BrandId = brand.BrandId,
                    NameAr = brand.NameAr,
                    NameEn = brand.NameEn,
                    ImageId = brand.ImageId
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching brand details for ID: {BrandId}", id);
                TempData["Error"] = "An error occurred while loading brand details";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Brand/Create
        public IActionResult Create()
        {
            return View(new BrandViewModel());
        }

        // POST: Brand/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var createDto = new CreateBrandDto
                {
                    NameAr = model.NameAr,
                    NameEn = model.NameEn,
                    BrandImage = model.BrandImage
                };

                var result = await _brandsService.CreateBrandAsync(createDto);

                TempData["Success"] = "Brand created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating brand");
                TempData["Error"] = "An error occurred while creating the brand";
                return View(model);
            }
        }

        // GET: Brand/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var brand = await _brandsService.GetBrandByIdAsync(id.Value);
                if (brand == null)
                {
                    return NotFound();
                }

                var viewModel = new BrandViewModel
                {
                    BrandId = brand.BrandId,
                    NameAr = brand.NameAr,
                    NameEn = brand.NameEn,
                    ImageId = brand.ImageId
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching brand for edit. ID: {BrandId}", id);
                TempData["Error"] = "An error occurred while loading the brand for editing";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Brand/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandViewModel model)
        {
            if (id != model.BrandId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var updateDto = new UpdateBrandDto
                {
                    BrandId = model.BrandId,
                    NameAr = model.NameAr,
                    NameEn = model.NameEn,
                    OldImageId = model.ImageId,
                    NewImage = model.BrandImage
                };

                var result = await _brandsService.UpdateBrandAsync(id, updateDto);
                if (result == null)
                {
                    TempData["Error"] = "Brand not found";
                    return View(model);
                }

                TempData["Success"] = "Brand updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating brand. ID: {BrandId}", id);
                TempData["Error"] = "An error occurred while updating the brand";
                return View(model);
            }
        }

        // GET: Brand/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var brand = await _brandsService.GetBrandByIdAsync(id.Value);
                if (brand == null)
                {
                    return NotFound();
                }

                var viewModel = new BrandDisplayViewModel
                {
                    BrandId = brand.BrandId,
                    NameAr = brand.NameAr,
                    NameEn = brand.NameEn,
                    ImageId = brand.ImageId
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching brand for delete. ID: {BrandId}", id);
                TempData["Error"] = "An error occurred while loading the brand for deletion";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Brand/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _brandsService.DeleteBrandAsync(id);
                if (!result)
                {
                    TempData["Error"] = "Brand not found";
                }
                else
                {
                    TempData["Success"] = "Brand deleted successfully!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting brand. ID: {BrandId}", id);
                TempData["Error"] = "An error occurred while deleting the brand";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}