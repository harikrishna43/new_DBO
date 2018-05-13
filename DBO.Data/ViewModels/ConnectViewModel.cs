using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.Data.ViewModels
{
    public class ConnectViewModel
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public int ConnectedCompanyId { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }
    }
}
