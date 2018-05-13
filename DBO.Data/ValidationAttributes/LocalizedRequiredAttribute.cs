using DBO.Data.Utilities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBO.Data.ValidationAttributes
{
    public class LocalizedRequiredAttribute : RequiredAttribute
    {
        private readonly string _errorMessageName;

        public LocalizedRequiredAttribute(string errorMessageName)
        {
            _errorMessageName = errorMessageName;
        }

        public override string FormatErrorMessage(string name)
        {
            return ResourceString.Get(_errorMessageName) ?? string.Empty;
        }
    }
}