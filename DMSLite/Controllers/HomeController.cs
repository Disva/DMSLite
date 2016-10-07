using DMSLite.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMSLite.Controllers
{
    public class HomeController : Controller
    {
        public Dispatcher dispatcher;
        public HomeController()
        {
            dispatcher = new Dispatcher();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendInput(FormCollection fc)
        {
            string inputText = fc["mainInput"];
            string returnedText = dispatcher.Dispatch(inputText);
            return Json(returnedText, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestInput()
        {
            return Json("Thanks", JsonRequestBehavior.AllowGet);
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