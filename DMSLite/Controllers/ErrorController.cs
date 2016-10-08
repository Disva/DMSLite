using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMSLite.DataContexts;
using DMSLite.Entities;

namespace DMSLite.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult ErrorMessage(string msg)
        {
            return PartialView("_ErrorMessage", msg);
        }
    }
}