using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Helper
{
    public class CheckCreditdebitAttribute : ValidationAttribute
    {
        private readonly string _otherProperties;
        private readonly object[] _desiredValue;

        public CheckCreditdebitAttribute(string otherProperties, params object[] desiredvalue)
        {
            _otherProperties = otherProperties;
            _desiredValue = desiredvalue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(_otherProperties);

            var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (_desiredValue.Contains(otherPropertyValue))
            {
                if (value == null)
                {
                    return new ValidationResult("This field is required if the payment type is Credit/Debit");
                }
            }

            return ValidationResult.Success;
        }
    }
}
