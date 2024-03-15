using PizzeriaEpicode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace PizzeriaEpicode.Controllers
{
    public class OrdersController : Controller
    {
        PizzeriaContext db = new PizzeriaContext();
       
        public ActionResult Summary(int id)
        {
            int userId;
            if (!int.TryParse(User.Identity.Name, out userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            int currentUserId = Convert.ToInt32(User.Identity.Name);
            var order = db.Orders.Include(o => o.OrderDetails.Select(od => od.Product))
                        .SingleOrDefault(o => o.Id == id);

            if (order == null || order.UserId != currentUserId)
            {
                return HttpNotFound();
            }

            var orderSummaryViewModel = new OrderSummaryViewModel
            {
                OrderId = order.Id,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailViewModel
                {
                    ProductName = od.Product.Name,
                    PriceAtTheTimeOfOrder = od.PriceAtTheTimeOfOrder,
                    Quantity = od.Quantity,
                    TotalePrice = od.Quantity * od.PriceAtTheTimeOfOrder
                }).ToList(),
                TotalOrderPrice = order.OrderDetails.Sum(od => od.Quantity * od.PriceAtTheTimeOfOrder)
            };

            return View(orderSummaryViewModel);
        }

        public ActionResult MyOrders()
        {
            int userId;
            if (!int.TryParse(User.Identity.Name, out userId))
            {
                return RedirectToAction("Login", "Auth");
            }
            var myOrders = db.Orders.Where(o => o.UserId == userId).ToList();
            return View(myOrders);
        }


    }
}