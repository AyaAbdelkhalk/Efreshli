using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Application.Helper.ResultPattern;
using Efreshli.Application.Interfaces;
using Efreshli.Application.Services.File;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Enums;
using Efreshli.Domain.Models;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        #region Props&Ctor
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        public CategoryService(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }
        #endregion


        public async Task<Response<GetCategoryDto>> AddCategoryAsync(AddCategoryDto category)
        {
            var newCategory = new Category
            {
                NameAr = category.NameAr,
                NameEn = category.NameEn
            };

            // Handle image upload
            if (category.Image != null)
            {
                var imageResult = await _imageService.UploadImageAsync(category.Image, ImageReferenceType.Category, newCategory.CategoryId);
                
                    newCategory.ImageId = imageResult.Id;
                    newCategory.Image = await _unitOfWork.ImageRepository.GetByIdAsync(imageResult.Id);
                
            }

            // Handle parent category
            
            if (category.ParentId.HasValue)
            {
                var parentCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(category.ParentId.Value);
                if (parentCategory == null)
                {
                    return ResponseHandler.NotFound<GetCategoryDto>("Invalid Parent Id");
                }
                newCategory.ParentId = category.ParentId.Value;
                newCategory.Parent = parentCategory;
            }

            await _unitOfWork.CategoryRepository.AddAsync(newCategory);
            await _unitOfWork.SaveChangesAsync();

            // Return with includes
            var result = await _unitOfWork.CategoryRepository.GetByIdWithIncludeAsync(newCategory.CategoryId,
                includes: new Expression<Func<Category, object>>[]
                { c => c.Parent, c => c.Image }
            );

            return ResponseHandler.Created(result.Adapt<GetCategoryDto>());
        }

        public async Task<Response<GetCategoryDto>> GetCategoryByIdAsync(int id)
        {
            var result = await _unitOfWork.CategoryRepository.GetByIdWithIncludeAsync(id,
                includes: new Expression<Func<Category, object>>[]
                { c => c.Parent, c => c.Image }
            );

            if (result == null)
            {
                return ResponseHandler.NotFound<GetCategoryDto>("Category not found");
            }

            return ResponseHandler.Success(result.Adapt<GetCategoryDto>());
        }

        public async Task<Response<IEnumerable<GetCategoryDto>>> GetAllCategoriesAsync()
        {
            var result = await _unitOfWork.CategoryRepository.GetAllWithIncludeAsync(
                predicate: null,
                includes: new Expression<Func<Category, object>>[] { c => c.Parent, c => c.Image }
            );

            if (result == null || !result.Any())
            {
                return ResponseHandler.Success<IEnumerable<GetCategoryDto>>(new List<GetCategoryDto>());
            }

            var categoryDtos = result.Select(category => category.Adapt<GetCategoryDto>()).ToList();
            return ResponseHandler.Success<IEnumerable<GetCategoryDto>>(categoryDtos);
        }

        public async Task<Response<IEnumerable<GetCategoryDto>>> GetMainCategoriesAsync()
        {
            var result = await _unitOfWork.CategoryRepository.GetAllWithIncludeAsync(
                predicate: c => c.ParentId == null,
                includes: new Expression<Func<Category, object>>[] { c => c.Image }
            );

            var categoryDtos = result.Select(category => category.Adapt<GetCategoryDto>()).ToList();
            return ResponseHandler.Success<IEnumerable<GetCategoryDto>>(categoryDtos);
        }

        public async Task<Response<IEnumerable<GetCategoryDto>>> GetSubCategoriesAsync(int parentId)
        {
            var result = await _unitOfWork.CategoryRepository.GetAllWithIncludeAsync(
                predicate: c => c.ParentId == parentId,
                includes: new Expression<Func<Category, object>>[] { c => c.Parent, c => c.Image }
            );

            var categoryDtos = result.Select(category => category.Adapt<GetCategoryDto>()).ToList();
            return ResponseHandler.Success<IEnumerable<GetCategoryDto>>(categoryDtos);
        }

        public async Task<Response<GetCategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto category)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return ResponseHandler.NotFound<GetCategoryDto>("Category not found");
            }

            // Update basic properties
            existingCategory.NameAr = category.NameAr;
            existingCategory.NameEn = category.NameEn;

            // Handle image update
            if (category.Image != null)
            {
                // Delete old image if exists
                if (existingCategory.ImageId.HasValue)
                {
                    await _imageService.DeleteImageAsync(existingCategory.ImageId.Value);
                }

                var imageResult = await _imageService.UploadImageAsync(category.Image, ImageReferenceType.Category, existingCategory.CategoryId);
                if (imageResult !=null) 
                {
                    existingCategory.ImageId = imageResult.Id;
                }
            }

            // Handle parent category update
            if (category.ParentId.HasValue)
            {
                // Check if trying to set parent to itself or its descendant
                if (category.ParentId.Value == id || await IsDescendant(id, category.ParentId.Value))
                {
                    return ResponseHandler.ValidationError<GetCategoryDto>("Cannot set parent to self or descendant");
                }

                var parentCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(category.ParentId.Value);
                if (parentCategory == null)
                {
                    return ResponseHandler.NotFound<GetCategoryDto>("Invalid Parent Id");
                }
                existingCategory.ParentId = category.ParentId.Value;
            }
            else
            {
                existingCategory.ParentId = null;
            }

            await _unitOfWork.CategoryRepository.UpdateAsync(existingCategory);
            await _unitOfWork.SaveChangesAsync();

            // Return with includes
            var result = await _unitOfWork.CategoryRepository.GetByIdWithIncludeAsync(id,
                includes: new Expression<Func<Category, object>>[]
                { c => c.Parent, c => c.Image }
            );

            return ResponseHandler.Success(result.Adapt<GetCategoryDto>());
        }

        public async Task<Response<bool>> DeleteCategoryAsync(int id)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return ResponseHandler.NotFound<bool>("Category not found");
            }

            // Check for child categories
            var childCategories = await _unitOfWork.CategoryRepository.GetAllAsync(c => c.ParentId == id);
            if (childCategories.Any())
            {
                return ResponseHandler.ValidationError<bool>("Cannot delete category with subcategories. Please delete or move subcategories first.");
            }

            // Delete associated image
            if (existingCategory.ImageId.HasValue)
            {
                await _imageService.DeleteImageAsync(existingCategory.ImageId.Value);
            }

            await _unitOfWork.CategoryRepository.RemoveAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ResponseHandler.Success(true);
        }

        // Helper method to check if a category is a descendant of another
        private async Task<bool> IsDescendant(int ancestorId, int descendantId)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(descendantId);

            while (category?.ParentId != null)
            {
                if (category.ParentId == ancestorId)
                {
                    return true;
                }
                category = await _unitOfWork.CategoryRepository.GetByIdAsync(category.ParentId.Value);
            }

            return false;
        }

        // Additional method for getting category hierarchy
        public async Task<Response<IEnumerable<GetCategoryDto>>> GetCategoryHierarchyAsync()
        {
            var allCategories = await _unitOfWork.CategoryRepository.GetAllWithIncludeAsync(
                predicate: null,
                includes: new Expression<Func<Category, object>>[] { c => c.Parent, c => c.Image }
            );

            var categoryDtos = allCategories.Select(category => category.Adapt<GetCategoryDto>()).ToList();

            // Build hierarchy (you can implement tree structure if needed)
            return ResponseHandler.Success<IEnumerable<GetCategoryDto>>(categoryDtos);
        }

        #region Helper Methods

        private async Task<bool> WouldCreateCircularReference(int categoryId, int potentialParentId)
        {
            int? currentParent = potentialParentId;
            while (currentParent != null)
            {
                if (currentParent == categoryId)
                {
                    return true;
                }
                var parent = await _unitOfWork.CategoryRepository.GetByIdAsync(currentParent.Value);
                currentParent = parent?.ParentId;
            }
            return false;
        }

        private async Task<bool> HasProducts(int categoryId)
        {
            // Check if category has products directly associated
            var products = await _unitOfWork.ProductRepository.GetAllAsync(p => p.CategoryId == categoryId);
            return products.Any();
        }

        private int GetProductCount(int categoryId)
        {
            // This is a simplified version - you might want to make this async and more sophisticated
            try
            {
                var products = _unitOfWork.ProductRepository.GetAll().Where(p => p.CategoryId == categoryId);
                return products.Count();
            }
            catch
            {
                return 0;
            }
        }

        private IEnumerable<GetCategoryDto> BuildCategoryHierarchy(List<Category> rootCategories, List<Category> allCategories)
        {
            return rootCategories.Select(root => BuildCategoryDto(root, allCategories));
        }

        private GetCategoryDto BuildCategoryDto(Category category, List<Category> allCategories)
        {
            var dto = category.Adapt<GetCategoryDto>();
            dto.ProductCount = GetProductCount(category.CategoryId);
            
            // Build children recursively
            var children = allCategories.Where(c => c.ParentId == category.CategoryId).ToList();
            if (children.Any())
            {
                dto.Children = children.Select(child => BuildCategoryDto(child, allCategories)).ToList();
            }

            return dto;
        }

        #endregion
    }
}
