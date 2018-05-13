using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBO.Data.Models
{
    public enum FileType
    {
        Image
    }

    public class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public FileType Type { get; set; }
        public string FilePath { get; set; }
        public DateTime DateOfCreate { get; set; }
        public bool IsRemoved { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
