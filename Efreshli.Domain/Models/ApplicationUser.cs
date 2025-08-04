using Efreshli.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Efreshli.Domain.Models
{
    public class ApplicationUser : IdentityUser , IAuditable
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? FullName => $"{FirstName} {LastName}";

        public int? ImageId { get; set; }

        public virtual Image? Image { get; set; }

        public virtual ICollection<Address>? Addresses { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }

        public virtual ICollection<Wishlist>? Wishlists { get; set; }

        public virtual ICollection<Cart>? Carts { get; set; }











        //Auditable properties
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; } 
        public DateTime? DeletedDate { get; set; }
    }
}
