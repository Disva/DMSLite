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

        public void Log()
        {
            Helpers.Log.WriteLog("User reported an error.", " ! ");
        }
    }
}