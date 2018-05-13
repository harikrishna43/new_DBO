using DBO.Common;
using DBO.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace DBO.Data.ViewModels
{


    public class AdvertisementViewModel : IValidatableObject
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        [MaxLength(50)]
        public string Headline { get; set; }
        [MaxLength(200)]
        public string Text { get; set; }
        public string Link { get; set; }
        public bool LinkToProfile => string.IsNullOrEmpty(Link);

        [Required]
        public string FilePath { get; set; }
        public bool StartDateImmediately { get; set; } = true;
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? StartDate { get; set; }
        public bool EndDateBudgetDefined { get; set; } = true;
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }

        // Settings
        public IEnumerable<string> SelectedSkills { get; set; }
        public IEnumerable<SelectListItem> Skills { get; set; }
        public IEnumerable<string> SelectedIndustries { get; set; }
        public IEnumerable<SelectListItem> Industries { get; set; }

        [Required]
        public AdvertisementType Type { get; set; }
        public AdvertisementStatus Status { get; set; }
        public LocationType LocationType { get; set; }


        // Settings flags
        public bool ApearOnLogin { get; set; }
        public bool ApearOnLogout { get; set; }
        public bool ApearForPrivatePerson { get; set; }
        public bool ApearForCompany { get; set; }
        public bool IsFullWidth { get; set; }
        public decimal ClickPrice { get; set; }
        public decimal Budget { get; set; }
        public decimal BudgetSpent { get; set; }
        public int ClicksCount { get; set; }

        //Payment settings
        public bool IsAdmin { get; set; }
        public bool IsPaid { get; set; }

        public decimal BudgetLeft => Budget - BudgetSpent;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> res = new List<ValidationResult>();

            if (!LinkToProfile && string.IsNullOrEmpty(Link))
            {
                res.Add(new ValidationResult("Link is required", new string[] { nameof(Link) }));
            }
            if (!StartDateImmediately && StartDate == default(DateTime))
            {
                res.Add(new ValidationResult("Field is required", new string[] { nameof(EndDate) }));
            }
            if (!EndDateBudgetDefined && EndDate == default(DateTime))
            {
                res.Add(new ValidationResult("Field is required", new string[] { nameof(EndDate) }));
            }

            if (StartDateImmediately) StartDate = DateTime.Now;
            //if (EndDateBudgetDefined) EndDate = DateTime.Now.AddDays(7);
            //if (LinkToProfile && HttpContext.Current.User.IsInRole(Constants.CompanyRole))
            //{
            //    Link = $"{Constants.BussinessDetailsLink}{CompanyId}";
            //}


            if (StartDate > EndDate)
            {
                res.Add(new ValidationResult("End date must be greater than or equal to start date.", new string[] { nameof(EndDate) }));
            }
            if (Budget < 500 && !IsAdmin)
            {
                res.Add(new ValidationResult("Budget must be greater than 500 Kr.", new string[] { nameof(Budget) }));
            }
            if (ClickPrice < 1 && !IsAdmin)
            {
                res.Add(new ValidationResult("Click price must be greater than 1 Kr.", new string[] { nameof(ClickPrice) }));
            }

            if (LocationType == LocationType.Cities && string.IsNullOrEmpty(Location))
            {
                res.Add(new ValidationResult("Enter name of city. If more than one - separate by comma.", new string[] { nameof(Location) }));
            }

            return res;
        }
    }
}
