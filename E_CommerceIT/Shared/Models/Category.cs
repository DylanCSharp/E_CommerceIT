using System;
using System.Collections.Generic;

#nullable disable

namespace E_CommerceIT.Shared.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryUrl { get; set; }
        public string CategoryIcon { get; set; }
        public string CategoryType { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
