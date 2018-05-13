using System.Collections.Generic;
using DBO.Common;
using DBO.Data.Models;

namespace DBO.Data.ViewModels
{
    public class AdminSearchViewModel
    {
        public List<Company> Companies { get; set; }
        public SearchParams SearchParams { get; set; }
    }
}