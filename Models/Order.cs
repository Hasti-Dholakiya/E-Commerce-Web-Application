using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Day6.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}