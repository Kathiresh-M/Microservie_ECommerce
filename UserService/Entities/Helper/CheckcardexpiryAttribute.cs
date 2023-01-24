using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Helper
{
    public class CheckcardexpiryAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is string str)
            {
                if (DateTime.TryParseExact(str, "MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult("This field should be in the format of MM-YYYY");
        }
    }
}
