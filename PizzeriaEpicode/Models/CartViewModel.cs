using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PizzeriaEpicode.Models
{
    public class CartViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}