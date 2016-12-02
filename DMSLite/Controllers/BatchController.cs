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
        private OrganizationDb db;

        public BatchController()
        {
            db = new OrganizationDb();
        }

        public BatchController(OrganizationDb db)
        {
            this.db = db;
        }

        #region Fetch

        public ActionResult FetchBatch(Dictionary<string, object> parameters) //Main method to search for batches, parameters may or may not be used
        {
            List<Batch> matchingBatches = FindBatch(parameters);
            if (matchingBatches == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");
            else if (matchingBatches.Count == 0)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no batch was found");
            else //if (matchingBatches.Count > 1)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "more than one batch was found");
        }
        public ActionResult FetchBatches(Dictionary<string, object> parameters)
        {
            List<Batch> filteredBatches = FindBatches(parameters);
            if (filteredBatches == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");

            if (filteredBatches.Count == 0)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no batchess were found");

            return PartialView("~/Views/Batch/_FetchIndex.cshtml", filteredBatches);
        }

        public List<Batch> FindBatch(Dictionary<string, object> parameters)
        {
            List<Batch> filteredBatches = new List<Batch>();

            //the paramsExist variable is used to check if the list of filtered donors must be created or filtered.
            bool paramsExist =
                !String.IsNullOrEmpty(parameters["title"].ToString());

            //FetchByName creates, but never Filters, so far place first
            if (paramsExist)
            {
                if (!String.IsNullOrEmpty(parameters["title"].ToString()))
                    FetchByTitle(ref filteredBatches, parameters["title"].ToString());
            }
            else
            {
                filteredBatches = FetchAllBatches();
            }
            return filteredBatches;
        }

        public List<Batch> FindBatches(Dictionary<string, object> parameters)
        {
            List<Batch> filteredBatches = new List<Batch>();

            //the paramsExist variable is used to check if the list of filtered donors must be created or filtered.
            bool paramsExist =
                !String.IsNullOrEmpty(parameters["type"].ToString());

            //FetchByName creates, but never Filters, so far place first
            if(paramsExist)
            {
                if (!String.IsNullOrEmpty(parameters["type"].ToString()))
                    FetchByType(ref filteredBatches, parameters["type"].ToString());
            }
            else
            {
                filteredBatches = FetchAllBatches();
            }
            return filteredBatches;
        }

        private void FetchByTitle(ref List<Batch> list, string name)
        {
            //searching through the db uses LINQ, which is picky about what variables can be passed.
            //For instance, LINQ does not accept ArrayIndex variables in queries,
            //so they are individual string variables in this query instead.
            list.AddRange(db.Batches.Where(x => x.Title.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
        }

        public List<Batch> FetchAllBatches()
        {
            List<Batch> allBatches = db.Batches.ToList();
            return allBatches;
        }

        private void FetchByType(ref List<Batch> list, string v)
        {
            bool openBatches = true;
            if (v == "open")
                openBatches = true;
            else if (v == "closed")
                openBatches = false;
            if (list.Count == 0)
            {
                if(openBatches)
                    list.AddRange(db.Batches.Where(x => x.CloseDate == null));
                else
                    list.AddRange(db.Batches.Where(x => x.CloseDate != null));
            }                
            else
            {
                if (openBatches)
                    list.Where(x => x.CloseDate == null);
                else
                    list.Where(x => x.CloseDate != null);
            }                
        }
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
            if (ModelState.IsValid)
            {
                db.Add(batch);
                return PartialView("~/Views/Batch/_AddSuccess.cshtml", batch);
            }

            //an invalid submission shall return the form with some validation error messages.
            return PartialView("~/Views/Batch/_AddForm.cshtml", batch);
        }

        // Action to search for donors by name and obtain a json result
        public ActionResult SearchBatches(string searchKey)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                return new JsonResult { Data = new { results = new List<Batch>() }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            var batches = db.Batches.Where(x => x.Title.ToLower().StartsWith(searchKey.ToLower()));
            return new JsonResult { Data = new { results = batches.Select(x => new { title = x.Title, id = x.Id }) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region MadeByMs
        public ActionResult Remove(Batch batch)
        {
            if (ModelState.IsValid)
            {
                db.Batches.Remove(batch);
                db.SaveChanges();
                return Content("Removed", "text/html");
            }
            return PartialView("~/Views/Batches/_Add.cshtml", batch);
            //TODO: make sure the name field is recognized as valid by api.ai
        }
        // GET: Batch
        public ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}