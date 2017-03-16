using DMSLite.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using NLog;

namespace DMSLite.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public Dispatcher dispatcher;

        private static Logger logger = LogManager.GetLogger("serverlog");

        public HomeController()
        {
            dispatcher = Dispatcher.getDispatcher();
        }

        public ActionResult Index()
        {
            logger.Info("User connected to DMSLite.");
            return View();
        }

        public ActionResult SendInput(FormCollection fc)
        {
            string inputText = fc["mainInput"];
            logger.Info("User: \"{0}\"", inputText);

            var responseModel = dispatcher.Dispatch(inputText);

            return PartialView("~/Views/Home/_Response.cshtml", responseModel);
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