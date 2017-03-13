using System.IO;
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
            Helpers.Log.WriteLog(Helpers.Log.LogType.Bug, "User reported an error.");
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