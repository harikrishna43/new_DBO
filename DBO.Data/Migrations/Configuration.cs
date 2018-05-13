using System;
using DBO.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Constants = DBO.Common.Constants;

namespace DBO.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<DBO.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            AddRoles(roleManager);

            Console.WriteLine("Test*******");
            InitUsers(context);
        }

        public void InitUsers(ApplicationDbContext context, UserManager<ApplicationUser> userManager = null)
        {
            AddUser(userManager ?? new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)),
                Constants.AdminEmail, "adminPass", Constants.AdminRole);
        }

        private static void AddUser(UserManager<ApplicationUser> userManager, string userName, string password, string role)
        {
            if (userManager.FindByName(userName) == null)
            {
                ApplicationUser user = new ApplicationUser {Email = userName, UserName = userName };
                IdentityResult result = userManager.Create(user, password);
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, role);
                }
            }
        }

        private static void AddRoles(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.FindByName(Constants.AdminRole) == null)
            {
                roleManager.Create(new IdentityRole(Constants.AdminRole));
            }
            if (roleManager.FindByName(Constants.CompanyRole) == null)
            {
                roleManager.Create(new IdentityRole(Constants.CompanyRole));
            }
            if (roleManager.FindByName(Constants.EmployeeRole) == null)
            {
                roleManager.Create(new IdentityRole(Constants.EmployeeRole));
            }
        }

        private static void AddFakeCompanies(ApplicationDbContext context)
        {
            foreach (var company in context.Companies)
            {
                context.Companies.Remove(company);
            }

            context.Companies.AddOrUpdate(
                new Company
                {
                    Id = 1,
                    Connections = 23,
                    Image = "img/logo-1.png",
                    Name = "Esbjerg Erhvervsudvikling",
                    Address = "Forsknings- og udviklingsparken Niels Bohrs Vej 6",
                    City = "Esbjerg",
                    PostCode = "6700",
                    Phone = "+45 75 12 37 44"
                },
                new Company
                {
                    Id = 2,
                    Connections = 20,
                    Image = "img/logo-2.png",
                    Name = "Skybrudskompaniet",
                    Address = "Vestkraftkaj 4",
                    City = "Esbjerg",
                    PostCode = "6700",
                    Phone = "72 10 90 50"
                },
                new Company
                {
                    Id = 3,
                    Connections = 20,
                    TextDescription = "Seneste nyheder fra Sædding Revision",
                    Name = "Sædding Revision",
                    Address = "Snedkervej 17",
                    City = "Esbjerg V",
                    PostCode = "6710",
                    Phone = "75 15 22 22"
                },
                new Company
                {
                    Id = 4,
                    Connections = 4,
                    TextDescription = "Seneste nyheder fra Hans Kjellerup",
                    Name = "Hans Kjellerup",
                    Address = "Tømmervej 7",
                    City = "Esbjerg V",
                    PostCode = "6710",
                    Phone = "75 15 67 45"
                }
            );
        }
    }
}
