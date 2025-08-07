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
                newCategory.CreatedBy = parentCategory.CreatedBy;
            }
            await _unitOfWork.CategoryRepository.AddAsync(newCategory);
            await _unitOfWork.SaveChangesAsync();
            return ResponseHandler.Created(newCategory.Adapt<GetCategoryDto>());
        }
        public async Task<Response<GetCategoryDto>> GetCategoryByIdAsync(int id)
        {
            var res= await _unitOfWork.CategoryRepository.GetByIdWithIncludeAsync(id,
                includes: new Expression<Func<Category, object>>[] 
                { c => c.Parent, c => c.Image }
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
            await _unitOfWork.CategoryRepository.UpdateAsync(existingCategory);
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
