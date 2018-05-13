using System.Collections.Generic;

namespace DBO.Common
{
    public class SearchParams
    {
        public string SearchName { get; set; }
        public int? SearchIndustryCode { get; set; }
        public string SearchZipFrom { get; set; }
        public string SearchZipTo { get; set; }

        public bool OnlyUnprocessed { get; set; }
        public bool ActiveMembers { get; set; }
        public bool UnactiveMembers { get; set; }
        public bool FromIndex { get; set; }
    }
}
