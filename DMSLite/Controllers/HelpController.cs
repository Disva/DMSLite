using System.IO;
using System.Web.Mvc;

using NLog;

namespace DMSLite.Controllers
{
    public class HelpController : Controller
    {
        private static Logger logger = LogManager.GetLogger("serverlog");

        [Authorize]
        public ActionResult returnHelp(string msg)
        {
            return PartialView("_Help", msg);
        }

        public void Log()
        {
            logger.Error("User reported an error.");
        }

        [Authorize(Roles = "SPS")]
        public FileResult LogDownload()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.GetTempPath().ToString() + "DMSLitelog.txt");
            string fileName = "DMSLitelog-"+ System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".txt";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}