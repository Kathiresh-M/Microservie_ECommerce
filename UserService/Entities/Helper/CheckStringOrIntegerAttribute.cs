using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Entities.Helper
{
    public class CheckStringOrIntegerAttribute : ValidationAttribute
    {
        private readonly string _givenpaymentType;

        public CheckStringOrIntegerAttribute(string paymentType)
        {
            _givenpaymentType = paymentType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var paymentTypeProperty = validationContext.ObjectType.GetProperty(_givenpaymentType);

            var paymentTypePropertyValue = paymentTypeProperty.GetValue(validationContext.ObjectInstance);

            if (paymentTypePropertyValue == null)
                return ValidationResult.Success;

            if (paymentTypePropertyValue.Equals("Credit") || paymentTypePropertyValue.Equals("Debit"))
            {
                if (Regex.IsMatch(value.ToString(), @"[a-zA-Z]"))
                    return new ValidationResult("Payment value can contain only numbers if the type is Credit/Debit");
            }

            return ValidationResult.Success;
        }
    }
}
