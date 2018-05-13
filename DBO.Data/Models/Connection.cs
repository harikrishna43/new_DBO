using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public class Connection
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Company1))]
        public int CompanyId1 { get; set; }

        [ForeignKey(nameof(Company2))]
        public int CompanyId2 { get; set; }

        public ConnectionStatus Status { get; set; }

        public Company Company1 { get; set; }
        public Company Company2 { get; set; }
    }

    public enum ConnectionStatus
    {
        Requested,
        Approved,
        Rejected
    }
}
