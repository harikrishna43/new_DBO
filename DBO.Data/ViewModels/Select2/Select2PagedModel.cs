using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.Data.ViewModels
{
    public class Select2PagedModel
    {
        public int TotalCount { get; set; }
        public List<Select2Model> Items { get; set; }
    }
}
