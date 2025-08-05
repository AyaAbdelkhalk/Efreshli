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
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly IValidator<AddCategoryDto> _addCategoryValidator;
        private readonly IValidator<UpdateCategoryDto> _updateCategoryValidator;
        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IImageService imageService, IValidator<AddCategoryDto> addCategoryValidator, IValidator<UpdateCategoryDto> updateCategoryValidator)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _addCategoryValidator = addCategoryValidator;
            _updateCategoryValidator = updateCategoryValidator;
        }
        public async Task<Response<GetCategoryDto>> AddCategoryAsync(AddCategoryDto category)
        {
            var validationResult = await _addCategoryValidator.ValidateAsync(category);
            if (!validationResult.IsValid)
            {
                return ResponseHandler.ValidationError<GetCategoryDto>(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var newCategory = new Category
            {
                NameAr = category.NameAr,
                NameEn = category.NameEn            
            };
            
            if (category.Image != null)
            {
                var id = await _imageService.UploadImageAsync(category.Image, ImageReferenceType.Category, newCategory.CategoryId);
                newCategory.ImageId = id.Id;
                newCategory.Image = await _unitOfWork.ImageRepository.GetByIdAsync(id.Id);
            }
            if (category.ParentId.HasValue)
            {
                var parentCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(category.ParentId.Value);
                if (parentCategory == null)
                {
                    return ResponseHandler.NotFound<GetCategoryDto>("Invalid Parent Id");
                }
                newCategory.ParentId = parentCategory.ParentId;
                newCategory.Parent = parentCategory;
            }
            await _categoryRepository.AddAsync(newCategory);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Created(newCategory.Adapt<GetCategoryDto>());
        }
        public async Task<Response<GetCategoryDto>> GetCategoryByIdAsync(int id)
        {
            var res= await _unitOfWork.CategoryRepository.GetByIdWithIncludeAsync(id,
                includes: new Expression<Func<Category, object>>[] { c => c.Parent, c => c.Image }
                );

            if (res == null)
            {
                return ResponseHandler.NotFound<GetCategoryDto>();
            }
            return ResponseHandler.Success(res.Adapt<GetCategoryDto>());
        }
        public async Task<Response<IEnumerable<GetCategoryDto>>> GetAllCategoriesAsync()
        {
            var res = await _unitOfWork.CategoryRepository.GetAllWithIncludeAsync(
                predicate: null,
                includes: new Expression<Func<Category, object>>[] { c => c.Parent, c => c.Image }
            );

            if (res == null || !res.Any())
            {
                return ResponseHandler.NotFound<IEnumerable<GetCategoryDto>>();
            }

            return ResponseHandler.Success(res.Select(category => category.Adapt<GetCategoryDto>()));
        }
        public async Task<Response<GetCategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto category)
        {
            category.CategoryId = id;
            var validationResult = await _updateCategoryValidator.ValidateAsync(category);
            if (!validationResult.IsValid)
            {
                return ResponseHandler.ValidationError<GetCategoryDto>(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }
            var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return ResponseHandler.NotFound<GetCategoryDto>();
            }
            existingCategory.NameAr = category.NameAr;
            existingCategory.NameEn = category.NameEn;
            if (category.Image != null)
            {
                var image = await _imageService.UploadImageAsync(category.Image, ImageReferenceType.Category, existingCategory.CategoryId);
                existingCategory.ImageId = image.Id;
            }
            if (category.ParentId.HasValue)
            {
                var parentCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(category.ParentId.Value);
                if (parentCategory == null)
                {
                    return ResponseHandler.NotFound<GetCategoryDto>("Invalid Parent Id");
                }
                existingCategory.ParentId = parentCategory.ParentId;
                existingCategory.Parent = parentCategory;
            }
            else
            {
                existingCategory.ParentId = null;
                existingCategory.Parent = null;
            }
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Success(existingCategory.Adapt<GetCategoryDto>());
        }
        public async Task<Response<bool>> DeleteCategoryAsync(int id)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                return ResponseHandler.NotFound<bool>();
            }
            var childCategories = await _unitOfWork.CategoryRepository.GetAllAsync(c => c.ParentId == id);
            if (childCategories.Any())
            {
                return ResponseHandler.ValidationError<bool>("Cannot delete category with child categories.");
            }
            await _unitOfWork.CategoryRepository.RemoveAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Success(true);
        }



    }
}
