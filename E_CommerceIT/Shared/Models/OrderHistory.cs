using System;
using System.Collections.Generic;

#nullable disable

namespace E_CommerceIT.Shared.Models
{
    public partial class OrderHistory
    {
        public int OrderNum { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? OrderDate { get; set; }
        public int? OrderYear { get; set; }

        public virtual Category Category { get; set; }
        public virtual Product Product { get; set; }
        public virtual Users User { get; set; }
    }
}
