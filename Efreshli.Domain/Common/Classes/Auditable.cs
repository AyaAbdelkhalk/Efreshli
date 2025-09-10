using Efreshli.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Domain.Common.Classes
{
    public class Auditable : IAuditable
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
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            if (cultureInfo.TwoLetterISOLanguageName.ToLower().Equals("ar"))
                return textAr;
            else return textEn;
        }
    }
}
