using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AFS_Rekrutacja.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "AFS translation application";

            return View("About");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "My contact information.";

            return View("Contact");
        }
    }
}