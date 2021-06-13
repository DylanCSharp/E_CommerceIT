using System;
using System.Collections.Generic;

#nullable disable

namespace E_CommerceIT.Shared.Models
{
    public partial class Users
    {
        public Users()
        {
            OrderHistories = new HashSet<OrderHistory>();
        }

        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public byte[] UserPassword { get; set; }
        public Guid UserSalt { get; set; }
        public DateTime UserCreatedOn { get; set; }
        public int IsAdmin { get; set; }
        public int IsCustomer { get; set; }

        public virtual ICollection<OrderHistory> OrderHistories { get; set; }
    }
}
