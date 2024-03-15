using PizzeriaEpicode.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PizzeriaEpicode.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private PizzeriaContext db = new PizzeriaContext();
        // GET: Admin
        public ActionResult Index()
        {
            var unfulfilledOrders = db.Orders.Where(o => o.Status == "Unfulfilled")
                .Include(o => o.OrderDetails).Include(o => o.OrderDetails.Select(od => od.Product)).ToList();
            return View(unfulfilledOrders);
        }

        public ActionResult FulfillOrder(int id)
        {
            var order = db.Orders.Find(id);
            if(order != null)
            {
                order.Status = "Fulfilled";
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> GetFulfilledOrdersCount()
        {
            var count = await db.Orders.CountAsync(o => o.Status == "Fulfilled");
            return Json(new { totalFulfilledOrders = count }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DailyRevenue(DateTime date)
        {
            var totalRevenue = await db.Orders.Where(o => DbFunctions.TruncateTime(o.DateTime) == date.Date && o.Status == "Fulfilled").SelectMany(o => o.OrderDetails).SumAsync(od => (decimal?)od.Quantity * od.PriceAtTheTimeOfOrder) ?? 0;

            return Json(new { Date = date.Date.Date, TotalRevenue = totalRevenue }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDailyRevenue()
        {
            return View();
        }
    }
}