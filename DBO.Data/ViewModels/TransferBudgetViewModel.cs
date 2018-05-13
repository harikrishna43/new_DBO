using DBO.Data.Utilities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBO.Data.ViewModels
{
    public class TransferBudgetViewModel : IValidatableObject
    {
        public int CurrentAdId { get; set; }
        public bool TransferToAnother { get; set; }
        public decimal? Amount { get; set; }
        public int? SelectedAdvertisementId { get; set; }
        public string ErrorMessage { get; set; }
        public decimal? RemainingBudget { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> res = new List<ValidationResult>();

            if (Amount == decimal.Zero || Amount < decimal.Zero || !Amount.HasValue)
            {
                res.Add(new ValidationResult((string)ResourceString.Instance.TransferBudgetError, new [] { nameof(Amount) }));
            }

            if (!SelectedAdvertisementId.HasValue)
            {
                res.Add(new ValidationResult((string)ResourceString.Instance.SelectAdvertisementBeforeTransfer, new [] { nameof(SelectedAdvertisementId) }));
            }

            return res;
        }
    }
}
