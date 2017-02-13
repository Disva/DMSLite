using System.Web.Mvc;

namespace DMSLite.Controllers
{
    public class HelpController : Controller
    {
        [Authorize]
        public ActionResult returnHelp(string msg)
        {
            return PartialView("_Help", msg);
        }

        public ActionResult Log()
        {
            Helpers.Log.writeLog("User reported an error.", " ! ");
            return Content("<script language='javascript' type='text/javascript'>alert('Thanks for Feedback!');</script>");
        }
    }
}