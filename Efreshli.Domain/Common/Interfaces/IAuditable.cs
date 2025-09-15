using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Domain.Common.Interfaces
{
    public interface IAuditable
    {
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } 
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string GetLocalized(string textAr, string textEn)
        {
            return string.IsNullOrEmpty(textAr) ? textEn : textAr;
        }

    }
}
