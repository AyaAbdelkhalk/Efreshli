using Efreshli.Application.DTOs.CategoryDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Validators.CategoryValidators
{
    public class AddCategoryValidator : AbstractValidator<AddCategoryDto>
    {
        public AddCategoryValidator() 
        { 
            RuleFor(c => c.NameAr)
                .NotEmpty().WithMessage("Arabic name is required.")
                .MaximumLength(100).WithMessage("Arabic name must not exceed 100 characters.");
            RuleFor(c => c.NameEn)
                .NotEmpty().WithMessage("English name is required.")
                .MaximumLength(100).WithMessage("English name must not exceed 100 characters.");
        }
    }
}
