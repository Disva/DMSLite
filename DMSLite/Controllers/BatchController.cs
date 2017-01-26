using DMSLite.DataContexts;
using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DMSLite.Controllers
{
    using DateRange = Tuple<DateTime, DateTime>;

    [Authorize]
    public class BatchController : Controller
    {
        private OrganizationDb db;

        private enum SEARCH_TYPE { BEFORE = -1, ON, AFTER };

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
                || !String.IsNullOrEmpty(parameters["datetype"].ToString()) && (
                ((!String.IsNullOrEmpty(parameters["date"].ToString()))) || !String.IsNullOrEmpty(parameters["date-period"].ToString())
                );

            if (!paramsExist)
                return FetchAllBatches();

            if (!String.IsNullOrEmpty(parameters["date"].ToString()) || !String.IsNullOrEmpty(parameters["date-period"].ToString()))
            {
                DateRange convertedDate = dateFromRange(ref parameters);
                FetchByDate(ref filteredBatches, convertedDate, parameters["datetype"].ToString());
                if (filteredBatches.Count == 0) goto Finish;
            }

            if (!String.IsNullOrEmpty(parameters["type"].ToString()))
            {
                FetchByType(ref filteredBatches, parameters["type"].ToString());
                if (filteredBatches.Count == 0) goto Finish;
            }

            if (!String.IsNullOrEmpty(parameters["title"].ToString()))
                FetchByTitle(ref filteredBatches, parameters["title"].ToString());

            Finish:
            return filteredBatches;
        }

        private DateRange dateFromRange(ref Dictionary<string, object> parameters)
        {

            if (!String.IsNullOrWhiteSpace(parameters["date"].ToString()))
            {
                DateTime dateValue = convertDate(parameters["date"].ToString());
                return Tuple.Create<DateTime, DateTime>(dateValue, dateValue);
            }

            if (!String.IsNullOrWhiteSpace(parameters["date-period"].ToString()))
                return Tuple.Create<DateTime, DateTime>(
                    convertDate(parameters["date-period"].ToString().Split('/')[0]),
                    convertDate(parameters["date-period"].ToString().Split('/')[1])
                    );

            return null;
        }

        // TODO: rebuff
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

        // extract method
        private void FetchByDate(ref List<Batch> filteredBatches, DateRange searchRange, string datetype = "on")
        {
            SEARCH_TYPE searchType;
            if (datetype == "before")                      //True when searching before a certain date
                searchType = SEARCH_TYPE.BEFORE;
            else if (datetype == "after")                  //True when searching after a certain date
                searchType = SEARCH_TYPE.AFTER;
            else                                           //True when searching on a specific date
                searchType = SEARCH_TYPE.ON;
            bool emptyList = (filteredBatches.Count == 0); //True if the list is empty

            if (emptyList)
                addByDate(ref filteredBatches, searchRange, searchType);
            else
                filterByDate(ref filteredBatches, searchRange, searchType);
        }

        private void filterByDate(ref List<Batch> filteredBatches, DateRange searchRange, SEARCH_TYPE searchType)
        {
            switch (searchType)
            {
                case SEARCH_TYPE.BEFORE: //Searching before a date
                    filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item1) < 0).ToList(); //The createDate is earlier than the searchDate
                    break;
                case SEARCH_TYPE.AFTER: //Searching after a date
                    filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item2) > 0).ToList(); //The createDate is later than the searchDate
                    break;
                case SEARCH_TYPE.ON:    //Searching on a date
                    filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item1) >= 0 && DateTime.Compare(x.CreateDate, searchRange.Item2) <= 0).ToList(); //The createDate is the same as the searchDate
                    break;
            }
        }

        private void addByDate(ref List<Batch> filteredBatches, DateRange searchRange, SEARCH_TYPE searchType)
        {
            switch (searchType)
            {
                case SEARCH_TYPE.BEFORE: //Searching before a date
                    filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item1) < 0)); //The createDate is earlier than the searchDate
                    break;
                case SEARCH_TYPE.AFTER:  //Searching after a date
                    filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item2) > 0)); //The createDate is later than the searchDate
                    break;
                case SEARCH_TYPE.ON:     //Searching on a date
                    filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item1) >= 0 && DateTime.Compare(x.CreateDate, searchRange.Item2) <= 0)); //The createDate is the same as the searchDate
                    break;
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

        private void FetchByType(ref List<Batch> list, string batchStatus)
        {
            bool openBatches = !batchStatus.Equals("closed");

            if (list.Count == 0)
            {
                if (openBatches)
                    list.AddRange(db.Batches.Where(x => x.CloseDate == null));
                else
                    list.AddRange(db.Batches.Where(x => x.CloseDate != null));
            }
            else
            {
                if (openBatches)
                    list = list.Where(x => x.CloseDate == null).ToList();
                else
                    list = list.Where(x => x.CloseDate != null).ToList();
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
            if (batchToClose.CloseDate != null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "batch is already closed");

            batchToClose.CloseDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Modify(batchToClose);
                return PartialView("~/Views/Batch/_CloseSuccess.cshtml", batchToClose);
            }
            return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "batch was invalid");

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
            List<Batch> similarBatches = db.Batches.Where(x => x.Title == batch.Title).ToList();
            if (similarBatches.Count() > 0)
            {
                //return an error
                ModelState.AddModelError("Title", "An existing batch has this title.");
            }
            else
            {
                batch.CreateDate = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.Add(batch);
                    return PartialView("~/Views/Batch/_AddSuccess.cshtml", batch);
                }
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