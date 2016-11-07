using DMSLite.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMSLite.Controllers
{
    public class BatchController : Controller
    {
        private OrganizationDb db = new OrganizationDb();

        #region Fetch
        #endregion

        #region Modify
        #endregion

        #region Add
        public ActionResult AddMenu(Dictionary<string, object> parameters)
        {
            return PartialView("~/Views/Batches/_Add.cshtml", parameters); //view must be made
        }
        #endregion

        #region MadeByMs
        // GET: Batch
        public ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}