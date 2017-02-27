using DMSLite.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMSLite.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public Dispatcher dispatcher;
        public HomeController()
        {
            Helpers.Log.WriteLog("User connected to DMSLite", Helpers.Log.LogType.Task);
            dispatcher = Dispatcher.getDispatcher();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendInput(FormCollection fc)
        {
            string inputText = fc["mainInput"];
            Helpers.Log.WriteLog(inputText, Helpers.Log.LogType.UserIn);

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