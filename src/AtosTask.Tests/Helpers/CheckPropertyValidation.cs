using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtosTask.Tests.Helpers
{
    public class CheckPropertyValidation
    {
        public IList<ValidationResult> ValidateObject(object model)
        {
            var result = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, result);
            if (model is IValidatableObject m)
            {
                m.Validate(validationContext);
            }
            return result;
        }
    }
}    
