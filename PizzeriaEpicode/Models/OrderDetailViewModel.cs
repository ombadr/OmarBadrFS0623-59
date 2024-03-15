using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PizzeriaEpicode.Models
{
    public class OrderDetailViewModel
    {
        public string ProductName { get; set; }
        public decimal PriceAtTheTimeOfOrder { get; set; }
        public int Quantity { get; set; }
        public decimal TotalePrice { get; set; }
    }
}