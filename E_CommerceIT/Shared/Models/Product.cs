using System;
using System.Collections.Generic;

#nullable disable

namespace E_CommerceIT.Shared.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderHistories = new HashSet<OrderHistory>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public decimal? ProductPrice { get; set; }
        public DateTime? ProductCreated { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<OrderHistory> OrderHistories { get; set; }
    }
}
