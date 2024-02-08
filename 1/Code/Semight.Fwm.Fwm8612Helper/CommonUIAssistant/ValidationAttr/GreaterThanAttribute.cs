using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semight.Fwm.Fwm8612Helper.CommonUIAssistant.ValidationAttr
{
    public sealed class GreaterThanAttribute : ValidationAttribute
    {
        public GreaterThanAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;

            var property = instance.GetType().GetProperty(PropertyName);
            var otherValue = property?.GetValue(instance);

            if (value is not null && ((IComparable)value).CompareTo(otherValue) > 0)
                return ValidationResult.Success;

            return new("The current value is smaller than the other one");
        }
    }
}