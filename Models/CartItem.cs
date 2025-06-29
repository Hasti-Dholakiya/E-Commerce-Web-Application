﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Day6.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
    }
}