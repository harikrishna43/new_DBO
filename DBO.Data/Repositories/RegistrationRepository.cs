using System;
using DBO.Data.Models;
using System.Threading.Tasks;

namespace DBO.Data.Repositories
{
    public class RegistrationRepository
    {
        ApplicationDbContext _db = new ApplicationDbContext();

        public async Task AddRegistrationRequest(ClaimRequest claimRequest)
        {
            _db.ClaimRequests.Add(claimRequest);
            await _db.SaveChangesAsync();
        }

        public Guid GetRegistrationCode(int companyId)
        {
            var token = Guid.NewGuid();
            var registrationCode = new RegistrationCode
            {
                Id = token,
                CompanyId = companyId,
                Generated = DateTime.Now,
            };

            _db.RegistrationCodes.Add(registrationCode);
            _db.SaveChanges();
            return token;
        }

        public int? GetCompanyByToken(Guid token)
        {
            var item = _db.RegistrationCodes.Find(token);
            return item?.CompanyId;
        }

        public void AddCompanyRegistration(Guid? token, int cvr, string name, string email)
        {
            var item = _db.RegistrationCodes.Find(token);
            if (item != null)
            {
                item.Cvr = cvr;
                item.Name = name;
                item.Email = email;
                item.Registered = DateTime.Now;
                item.IsCompany = true;
            }
            else
            {
                item = new RegistrationCode
                {
                    Id = Guid.NewGuid(),
                    Cvr = cvr,
                    Name = name,
                    Email = email,
                    Registered = DateTime.Now,
                    IsCompany = true
                };

                _db.RegistrationCodes.Add(item);
            }

            _db.SaveChanges();
        }

        public void AddPersonRegistration(string vmName, string vmEmail)
        {
            var person = new RegistrationCode
            {
                Id = Guid.NewGuid(),
                Name = vmName,
                Email = vmEmail,
                Registered = DateTime.Now,
                IsCompany = false
            };

            _db.RegistrationCodes.Add(person);
            _db.SaveChanges();
        }
    }
}
