using DMSLite.DataContexts;
using DMSLite.Entities;
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
            return PartialView("~/Views/Batch/_Add.cshtml", parameters);
        }

        public ActionResult AddForm(Dictionary<string, object> parameters)
        {
            Batch newBatch = new Batch();
            if (parameters.ContainsKey("title"))
                newBatch.Title = parameters["title"].ToString();
            return PartialView("~/Views/Batch/_AddForm.cshtml", newBatch);
        }

        // TODO: Anti-forgery
        public ActionResult Add(Batch batch)
        {
            batch.CreateDate = DateTime.Now;
            batch.BatchOrganization = db.Organizations.First();
            //ModelState["batch.CreateDate"].Errors.Clear();
            ModelState["BatchOrganization"].Errors.Clear();
            //UpdateModel(batch);
            if (ModelState.IsValid)
            {                
                db.Batches.Add(batch);
                db.SaveChanges();
                return PartialView("~/Views/Batch/_AddSuccess.cshtml", batch);
            }

            //an invalid submission shall return the form with some validation error messages.
            return PartialView("~/Views/Batch/_AddForm.cshtml", batch);
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