using DBO.Data.Repositories;
using DBO.Data.Utilities;
using DBO.Data.ValidationAttributes;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DBO.Data.ViewModels
{
    public class ClaimingViewModel
    {
        [MaxLength(255)]
        [LocalizedRequired("ClaimCompanyName")]
        [LocalizedDisplayName("Name")]
        public string Name { get; set; }

        [EmailAddress]
        [LocalizedRequired("ClaimCompanyEmail")]
        [LocalizedDisplayName("Email")]
        [ClaimedCompany("ClaimCompany_UserHasCompany")]
        public string Email { get; set; }

        [Display(Name="Agree with terms and conditions")]
        [Range(typeof(bool), "true", "true")]
        public bool IsAgreedWithTerms { get; set; }

        public CompanyViewModel Company { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ClaimedCompanyAttribute : ValidationAttribute
    {
        private readonly string _errorMessage;

        public ClaimedCompanyAttribute(string errorMessageName)
        {
            _errorMessage = errorMessageName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext != null)
            {
                var userRepository = new UserRepository();
                var companyRepository = new CompanyRepository();

                if (value is string strValue && (!userRepository.Query().Any(x => x.Email == strValue && x.CompanyId != null) ||
                    companyRepository.Query().Any(x => x.Email == strValue)))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessage);
        }

        public override string FormatErrorMessage(string name)
        {
            return ResourceString.Get(_errorMessage) ?? string.Empty;
        }
    }
}
