using DBO.Data.Models;
using System;
using System.Collections.Generic;
using DBO.Common.Helpers;

namespace DBO.Data.ViewModels
{
    public class CompanyViewModel
    {
        public CompanyViewModel()
        {

        }

        public CompanyViewModel(Company company)
        {
            this.Id = company.Id;
            this.Name = company.Name;
            this.Connections = company.Connections;
            this.Image = company.Image;
            this.TextDescription = company.TextDescription;
            this.Address1 = company.Address;
            this.Address2 = company.PostCode + " " + company.City;
            this.Phone = company.Phone;
            this.Email = company.Email;
            this.PostCode = company.PostCode;
            this.Address = company.Address;
            this.City = company.City;
            this.Web = !string.IsNullOrEmpty(company.Web) ? LinkHelper.GetCorrectLink(company.Web) : company.Web;
        }

        public int Id { get; set; }
        public int Connections { get; set; }
        public string Image { get; set; }
        public string TextDescription { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        public List<Company> ApprovedConnections { get; set; }

        public List<Company> PendingConnections { get; set; }

        public IEnumerable<CompanySkill> Skills { get; set; }

        public IEnumerable<ApplicationUser> Employees { get; set; }

        public NotificationSettings NotificationSettings { get; set; }

        public int Followers { get; set; }
        public int CurrentCompany { get; set; }
        public string Web { get; set; }
    }
}