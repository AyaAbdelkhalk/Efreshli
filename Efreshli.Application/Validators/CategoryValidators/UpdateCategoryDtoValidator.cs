using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Domain.Common.Interfaces;
using Efreshli.Domain.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.CategoryValidators
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.NameAr)
                .NotEmpty().WithMessage("Arabic name is required.")
                .MaximumLength(100).WithMessage("Arabic name must not exceed 100 characters.")
                .MustAsync(async (dto, name, cancellation) => await IsUniqueNameAr(dto.NameAr, dto.CategoryId))
                .WithMessage("Arabic name already exists");

            RuleFor(c => c.NameEn)
                .NotEmpty().WithMessage("English name is required.")
                .MaximumLength(100).WithMessage("English name must not exceed 100 characters.")
                .MustAsync(async (dto, name, cancellation) => await IsUniqueNameEn(dto.NameEn, dto.CategoryId))
                .WithMessage("English name already exists");
            RuleFor(c => c.ParentId)
                .MustAsync(async (dto, parentId, cancellation) => await IsValidParentId(dto.ParentId))
                .WithMessage("Invalid Parent Id");
        }

        private async Task<bool> IsUniqueNameAr(string name, int id)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.CountAsync(c => c.NameAr == name && c.CategoryId != id);
            return existingCategory == 0;
        }

        private async Task<bool> IsUniqueNameEn(string name, int id)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.CountAsync(c => c.NameEn == name && c.CategoryId != id);
            return existingCategory == 0;
        }

        private async Task<bool> IsValidParentId(int? parentId)
        {
            if (!parentId.HasValue) return true;
            var parentCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(parentId.Value);
            return parentCategory != null;
        }
    }
}