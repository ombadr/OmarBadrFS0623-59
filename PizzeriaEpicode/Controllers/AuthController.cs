using PizzeriaEpicode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PizzeriaEpicode.Controllers
{
    public class AuthController : Controller
    {
        PizzeriaContext db = new PizzeriaContext();
       

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user, bool? keepLogged)
        {
            bool keepUserLogged = keepLogged.HasValue && keepLogged.Value;
            var loggedUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
            if (loggedUser != null)
            {
                bool validPassword = BCrypt.Net.BCrypt.Verify(user.PasswordHash, loggedUser.PasswordHash);
                if (validPassword) {
                    FormsAuthentication.SetAuthCookie(loggedUser.Id.ToString(), keepUserLogged);
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["ErrorLogin"] = true;
            return RedirectToAction("Login");
        }

        public ActionResult Register()
        {

            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if(existingUser == null)
                {
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);

                    var newUser = new User
                    {
                        Name = model.Name,
                        Email = model.Email,
                        PasswordHash = hashedPassword,
                        Role = "User"
                    };
                    db.Users.Add(newUser);
                    db.SaveChanges();

                    FormsAuthentication.SetAuthCookie(newUser.Id.ToString(), false);
                    return RedirectToAction("Index", "Home");
                } else
                {
                    ModelState.AddModelError("", "An account with this email already exists.");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


    }
}