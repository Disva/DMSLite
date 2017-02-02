﻿using System.Web.Mvc;

namespace DMSLite.Controllers
{
    public class HelpController : Controller
    {
        [Authorize]
        public ActionResult returnHelp(string msg)
        {
            return PartialView("_Help", msg);
        }
    }
}