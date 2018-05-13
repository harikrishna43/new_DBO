using DBO.Data.Models;

namespace DBO.Data.ViewModels
{
    public class DisplayAdViewModel
    {
        public int Id { get; set; }

        public string FilePath { get; set; }

        public string Link { get; set; }

        public string Headline { get; set; }

        public string Text { get; set; }

        public AdvertisementType Type { get; set; }

        public bool CreatedByAdmin { get; set; }

        public bool IsFullWidth { get; set; }
    }
}
