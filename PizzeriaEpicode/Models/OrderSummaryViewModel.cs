using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PizzeriaEpicode.Models
{
    public class OrderSummaryViewModel
    {
        public int OrderId { get; set; }
        public string ShippingAddress { get; set; }
        public string Notes { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }

        public decimal TotalOrderPrice { get; set; }
    }
}