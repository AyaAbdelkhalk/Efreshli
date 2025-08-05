using Efreshli.Application.DTOs.CategoryDTOs;
using Efreshli.Domain.Common.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.CategoryValidators
{
    public class AddCategoryDtoValidator : AbstractValidator<AddCategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddCategoryDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.NameAr)
                .NotEmpty().WithMessage("Arabic name is required.")
                .MaximumLength(100).WithMessage("Arabic name must not exceed 100 characters.")
                .MustAsync(async (name, cancellation) => await IsUniqueNameAr(name)).WithMessage("Arabic name already exist");
            RuleFor(c => c.NameEn)
                .NotEmpty().WithMessage("English name is required.")
                .MaximumLength(100).WithMessage("English name must not exceed 100 characters.")
                .MustAsync(async (name, cancellation) => await IsUniqueNameEn(name)).WithMessage("English name already exist");

            RuleFor(c => c.ParentId)
                .MustAsync(async (parentId, cancellation) => await IsValidParentId(parentId))
                .WithMessage("Invalid Parent Id");
        }
        private async Task<bool> IsUniqueNameAr(string name)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.CountAsync(c => c.NameAr == name);
            return existingCategory == 0;

        }
        private async Task<bool> IsUniqueNameEn(string name)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.CountAsync(c => c.NameEn == name);
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
