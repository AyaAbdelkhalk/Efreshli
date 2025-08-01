using System.ComponentModel.DataAnnotations;

namespace Efreshli.Domain.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int Rate { get; set; }
        public string TextReview { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int UserId { get; set; }
    }
}
