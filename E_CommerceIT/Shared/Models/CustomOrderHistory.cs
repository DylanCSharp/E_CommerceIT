using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceIT.Shared.Models
{
    public class CustomOrderHistory
    {
        public int OrderNum { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string CategoryType { get; set; }
        public int OrderDate { get; set; }
        public string Month { get; set; }
        public int OrderYear { get; set; }
    }
}
