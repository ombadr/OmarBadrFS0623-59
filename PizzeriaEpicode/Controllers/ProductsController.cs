using PizzeriaEpicode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PizzeriaEpicode.Controllers
{
    public class ProductsController : Controller
    {

        PizzeriaContext db = new PizzeriaContext();

        // GET: Products
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }


        public ActionResult AddToCart(int productId, int quantity)
        {

            int userId;
            if (!int.TryParse(User.Identity.Name, out userId))
            {
                return RedirectToAction("Login", "Auth");
            }
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            CartItem existingItem = cart.Find(item => item.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem { ProductId = productId, Quantity = quantity });
            }

            Session["Cart"] = cart;
            return RedirectToAction("Index", "Products");
        }

        public ActionResult Details(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        public ActionResult Cart()
        {

            int userId;
            if (!int.TryParse(User.Identity.Name, out userId))
            {
                return RedirectToAction("Login", "Auth");
            }
            var cartItems = Session["Cart"] as List<CartItem>;
            var viewModel = new List<CartViewModel>();

            if (cartItems != null)
            {
                foreach (var item in cartItems)
                {
                    var product = db.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        viewModel.Add(new CartViewModel
                        {
                            ProductId = item.ProductId,
                            Name = product.Name,
                            Price = product.Price,
                            Quantity = item.Quantity
                        });
                    }
                }
            }
            return View(viewModel);
        }

        public ActionResult RemoveFromCart(int productId)
        {
            var cartItems = Session["Cart"] as List<CartItem>;

            if(cartItems != null)
            {
                var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);

                if(itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);
                    Session["Cart"] = cartItems;
                }
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string shippingAddress, string notes)
        {
            var cartItems = Session["Cart"] as List<CartItem>;
            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Cart");
            }

            var order = new Order
            {
                UserId = Convert.ToInt32(User.Identity.Name),
                ShippingAddress = shippingAddress,
                Notes = notes,
                Status = "Unfulfilled",
                DateTime = DateTime.Now,
            };

            db.Orders.Add(order);
            db.SaveChanges();

            decimal totalOrderPrice = 0;

            foreach (var item in cartItems)
            {
                var product = db.Products.Find(item.ProductId);
                if (product != null)
                {

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        PriceAtTheTimeOfOrder = product.Price,
                    };

                    totalOrderPrice += item.Quantity * product.Price;

                    db.OrderDetails.Add(orderDetail);
                }
            }

            db.SaveChanges();

            Session.Remove("Cart");

            return RedirectToAction("Summary","Orders", new { id = order.Id });
        }

    }
}