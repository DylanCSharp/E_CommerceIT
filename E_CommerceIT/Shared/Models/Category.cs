using System;
using System.Collections.Generic;

namespace E_CommerceIT.Shared.Models
{
    public partial class Category
    {
        public Category()
        {
            OrderHistories = new HashSet<OrderHistory>();
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryType { get; set; }
        public string CategoryIcon { get; set; }
        public string CategoryUrl { get; set; }

        public virtual ICollection<OrderHistory> OrderHistories { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
