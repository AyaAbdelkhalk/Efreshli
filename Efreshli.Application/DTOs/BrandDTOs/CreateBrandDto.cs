using System.ComponentModel.DataAnnotations;

namespace Efreshli.Application.DTOs.BrandDTOs
{
    public class CreateBrandDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Url]
        public string? Website { get; set; }

        [Url]
        public string? Logo { get; set; }
    }
}
