using Efreshli.Domain.Common.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efreshli.Domain.Models
{
    public class Review : Auditable
    {
        [Key]
        public int Id { get; set; }
        public int Rate { get; set; }
        public string ReviewText { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<Image>? Images { get; set; }
    }
}
