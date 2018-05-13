using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.Data.Models
{
    public class Follower
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public Guid UserId { get; set; }
    }
}
