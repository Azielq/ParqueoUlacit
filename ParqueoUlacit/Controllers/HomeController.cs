using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ParqueoUlacit.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["RolID"] == null || (int)Session["RolID"] != 1)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}