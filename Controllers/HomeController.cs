using CNF.Business.BusinessConstant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNF.API.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // BusinessCont.SendNotification(14193, 11711419300000004.ToString(), "Hiii", "Hello");
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
