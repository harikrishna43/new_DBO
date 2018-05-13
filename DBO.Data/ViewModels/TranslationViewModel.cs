using DBO.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.Data.ViewModels
{
    public class TranslationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IDictionary<string, string> Values { get; set; }
    }
}
