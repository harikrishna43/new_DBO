using DBO.Data.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DBO.Data.Repositories
{
    public class NotificationSettingsRepository
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public async Task<NotificationSettings> GetByUserId(string userId)
        {
            return await _context.NotificationSettings.FirstOrDefaultAsync(ns => ns.UserId.Equals(userId));
        }

        public async Task<NotificationSettings> GetById(int id)
        {
            return await _context.NotificationSettings.FindAsync(id);
        }

        public async Task<NotificationSettings> Add(NotificationSettings notificationSettings)
        {
            _context.NotificationSettings.Add(notificationSettings);
            await _context.SaveChangesAsync();
            return await GetByUserId(notificationSettings.UserId);
        }

        public async Task Update(NotificationSettings settings)
        {
            var savedSettings = await GetById(settings.Id);
            if (savedSettings != null)
            {
                _context.Entry(savedSettings).CurrentValues.SetValues(settings);
                await _context.SaveChangesAsync();
            }
        }
    }
}
