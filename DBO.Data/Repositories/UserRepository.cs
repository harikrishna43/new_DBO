using DBO.Data.Models;
using DBO.Data.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DBO.Data.Repositories
{
    public class UserRepository
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private UserManager<ApplicationUser> _userManager;

        public UserRepository()
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
        }

        public IQueryable<ApplicationUser> Query()
        {
            return _db.Users;
        }

        public IEnumerable<ApplicationUser> GetUsersByRole(string roleName)
        {
            var query = _userManager.Users.AsQueryable().Include("Company");
            return query.ToList().Where(x => _userManager.IsInRole(x.Id, roleName));
        }

        public async Task<ApplicationUser> FindAsync(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public ICollection<ApplicationUser> GetEmployees(int companyId)
        {
            return _db.Users.Where(x => x.CompanyId == companyId).ToList();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationUser user, string imagePath = null, string directoryPath = null)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var userFiles = _db.UserFiles.FirstOrDefault(uf => uf.UserId == user.Id && uf.Type == FileType.Image);
                var path = UploadFile(imagePath, directoryPath);
                userFiles.FilePath = path;
            }

            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public void CreateUsersFile(string userId, string base64path, FileType fileType, string directoryPath)
        {
            var filePath = UploadFile(base64path, directoryPath);
            if (!string.IsNullOrEmpty(filePath))
            {
                _db.UserFiles.Add(new UserFile
                {
                    UserId = userId,
                    FilePath = filePath,
                    Type = fileType,
                    DateOfCreate = DateTime.Now
                });
                _db.SaveChanges();
            }
        }

        public ApplicationUser GetById(string userId)
        {
            return _db.Users.FirstOrDefault(u => u.Id.Contains(userId));
        }

        public string GetUserFilePath(string userId, FileType type)
        {
            return _db.UserFiles.FirstOrDefault(uf => uf.UserId == userId && uf.Type == type)?.FilePath;
        }

        private string UploadFile(string base64Path, string directoryPath)
        {
            try
            {
                if (base64Path.Contains(DBO.Common.Constants.UserImagePath))
                    return base64Path;

                var path = string.Empty;
                var base64String = base64Path.Split(';')[1].Split(',')[1];
                var extension = base64Path.Split(';')[0].Split(':')[1].Split('/')[1];
                var fileBytes = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
                {
                    if (!Directory.Exists(DBO.Common.Constants.UserImagePath))
                    {
                        Directory.CreateDirectory(DBO.Common.Constants.UserImagePath);
                    }

                    //upload file
                    var filePathGuid = $"{Guid.NewGuid().ToString()}.{extension}";
                    var fullPath = Path.Combine(directoryPath, filePathGuid);

                    FileUploader.UploadFile(ms, fullPath);

                    path = $"{DBO.Common.Constants.UserImagePath}{filePathGuid}";
                }
                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
