using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Day6.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddToCart(CartItem item)
        {
            var existing = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public decimal CartTotal => Items.Sum(i => i.TotalPrice);
    }

}