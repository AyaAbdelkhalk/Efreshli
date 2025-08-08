using Efreshli.Domain.Models;

namespace Efreshli.MVC.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int BrandId { get; set; }  // مفتاح أجنبي
        public Brand Brand { get; set; }  // Navigation property
    }
}
