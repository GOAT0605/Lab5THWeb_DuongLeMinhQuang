using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace Lab3_DuongLeMinhQuang.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public string ShippingAddress { get; set; }
        public string Notes { get; set; }

        public IdentityUser User { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }

}
