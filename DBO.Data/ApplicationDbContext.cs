using System.Data.Entity;
using DBO.Data.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DBO.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CompanySkill> CompanySkills { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<ClaimRequest> ClaimRequests { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<RegistrationCode> RegistrationCodes { get; set; }
        public DbSet<ScheduledEmail> ScheduledEmails { get; set; }
        public DbSet<LogItem> Logs { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Advertisement> Ads { get; set; }
        public DbSet<AdsSkills> AdsSkills { get; set; }
        public DbSet<AdsIndustries> AdsIndustries { get; set; }
        public DbSet<AdClick> AdClicks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationSettings> NotificationSettings { get; set; }
        public DbSet<UserFile> UserFiles { get; set; }


        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}