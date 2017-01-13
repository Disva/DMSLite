using DMSLite.DataContexts;
using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DMSLite.Controllers
{
    [Authorize]
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
        public ActionResult FetchBatches(Dictionary<string, object> parameters)
        {
            List<Batch> filteredBatches = FindBatches(parameters);
            if (filteredBatches == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");

            if (filteredBatches.Count == 0)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no batches were found");

            return PartialView("~/Views/Batch/_FetchIndex.cshtml", filteredBatches);
        }

        public List<Batch> FindBatches(Dictionary<string, object> parameters)
        {
            List<Batch> filteredBatches = new List<Batch>();

            //the paramsExist variable is used to check if the list of batches must be created or filtered.
            //every call of FindBatches must include both a type and title i nthe params, even if empty
            bool paramsExist =
                !String.IsNullOrEmpty(parameters["type"].ToString())
                || !String.IsNullOrEmpty(parameters["title"].ToString())
                || ((!String.IsNullOrEmpty(parameters["date"].ToString()) && !String.IsNullOrEmpty(parameters["posttype"].ToString())));

            if(paramsExist)
            {
                if (!String.IsNullOrEmpty(parameters["date"].ToString()) && !String.IsNullOrEmpty(parameters["posttype"].ToString()))
                {
                    DateTime convertedDate = convertDate(parameters["date"].ToString());
                    if(!String.IsNullOrEmpty(parameters["datetype"].ToString()))
                    {
                        FetchByDate(ref filteredBatches, convertedDate, parameters["datetype"].ToString(), parameters["posttype"].ToString());
                    }
                    else
                    {
                        FetchByDate(ref filteredBatches, convertedDate, "on", parameters["posttype"].ToString());
                    }
                }
                if (!String.IsNullOrEmpty(parameters["type"].ToString()))
                    FetchByType(ref filteredBatches, parameters["type"].ToString());
                if (!String.IsNullOrEmpty(parameters["title"].ToString()))
                    FetchByTitle(ref filteredBatches, parameters["title"].ToString());
            }
            else
            {
                filteredBatches = FetchAllBatches();
            }
            return filteredBatches;
        }

        private DateTime convertDate(string date)
        {
            DateTime convertedDate;
            if (date.Length == 4)
                convertedDate = DateTime.ParseExact((date + "-01-01"), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            else
                convertedDate = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            bool result = date.All(Char.IsLetter);            
            return convertedDate;
        }

        private void FetchByDate(ref List<Batch> filteredBatches, DateTime searchDate, string datetype, string postType)
        {
            bool searchCreate;
            if (postType == "made")
                searchCreate = true;
            else
                searchCreate = false;
            if (filteredBatches.Count == 0)
            {
                if (datetype == "before")
                {
                    if (searchCreate)
                    {
                        filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CreateDate, searchDate) < 0)); //The createDate is earlier than the searchDate
                    }
                    else
                    {
                        filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CloseDate.Value, searchDate) < 0)); //The closeDate is earlier than the searchDate
                    }
                }
                else if (datetype == "after")
                {
                    if (searchCreate)
                    {
                        filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CreateDate, searchDate) > 0)); //The createDate is later than the searchDate
                    }
                    else
                    {
                        filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CloseDate.Value, searchDate) > 0)); //The closeDate is later than the searchDate
                    }
                }
                else
                {
                    if (searchCreate)
                    {
                        filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CreateDate, searchDate) == 0)); //The createDate is the same as the searchDate
                    }
                    else
                    {
                        filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CloseDate.Value, searchDate) == 0)); //The closeDate is the same as the searchDate
                    }
                }
            }
            else
            {
                if (datetype == "before")
                {
                    if (searchCreate)
                    {
                        filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CreateDate, searchDate) < 0).ToList(); //The createDate is earlier than the searchDate
                    }
                    else
                    {
                        filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CloseDate.Value, searchDate) < 0).ToList(); //The closeDate is earlier than the searchDate
                    }
                }
                else if (datetype == "after")
                {
                    if (searchCreate)
                    {
                        filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CreateDate, searchDate) > 0).ToList(); //The createDate is later than the searchDate
                    }
                    else
                    {
                        filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CloseDate.Value, searchDate) > 0).ToList(); //The closeDate is later than the searchDate
                    }
                }
                else
                {
                    if (searchCreate)
                    {
                        filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CreateDate, searchDate) == 0).ToList(); //The createDate is the same as the searchDate
                    }
                    else
                    {
                        filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CloseDate.Value, searchDate) == 0).ToList(); //The closeDate is the same as the searchDate
                    }
                }
            }
        }

        private void FetchByTitle(ref List<Batch> list, string Title)
        {
            //searching through the db uses LINQ, which is picky about what variables can be passed.
            //For instance, LINQ does not accept ArrayIndex variables in queries,
            //so they are individual string variables in this query instead.
            if (list.Count == 0)
            {
                //look for batches that contain the specified title (case insensitive)
                list.AddRange(db.Batches.Where(x => x.Title.ToUpper().Contains(Title.ToUpper())));
            }
            else
            {
                list = list.Where(x => x.Title.ToUpper().Contains(Title.ToUpper())).ToList();
            }
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

        public List<Batch> FetchAllBatches()
        {
            List<Batch> allBatches = db.Batches.ToList();
            return allBatches;
        }

        #endregion

        #region Modify
        public ActionResult CloseBatch(Dictionary<string, object> parameters)
        {
            String title = parameters["title"].ToString();
            Batch batchToClose = db.Batches.First(x => x.Title == title);
            return PartialView("~/Views/Batch/_CloseBatch.cshtml", batchToClose);
        }

        public ActionResult CloseBatchFromList(int id)
        {
            Batch batchToClose = db.Batches.First(x => x.Id == id);
            return PartialView("~/Views/Batch/_CloseBatch.cshtml", batchToClose);
        }

        public ActionResult PostBatch(int id)
        {
            Batch batchToClose = db.Batches.First(x => x.Id == id);
            if (batchToClose.CloseDate == null)
            {
                batchToClose.CloseDate = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.Modify(batchToClose);
                    return PartialView("~/Views/Batch/_CloseSuccess.cshtml", batchToClose);
                }
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "batch was invalid");
            }
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "batch is already closed");
        }
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