using System;
using DBO.Data.Models;

namespace DBO.Data.Repositories
{
    public class DatabaseLogging
    {
        ApplicationDbContext _db = new ApplicationDbContext();

        public void Add(string message)
        {
            _db.Logs.Add(new LogItem { Time = DateTime.Now, Value = message });
            _db.SaveChanges();
        }
    }
}
