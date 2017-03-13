using DMSLite.DataContexts;
using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DMSLite.Controllers
{
    using Helpers;
    using Models;
    using Newtonsoft.Json;
    using DateRange = Tuple<DateTime, DateTime>;

    [Authorize]
    public class BatchController : Controller
    {
        private OrganizationDb db;

        private enum SEARCH_TYPE { BEFORE = -1, ON, AFTER };
        private enum COMPARE_TYPE { UNDER = -1, EQUAL, OVER };
        private enum BATCH_TYPE {OPEN, CLOSED };

        private static List<Batch> filteredBatches;

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
            filteredBatches = FindBatches(parameters);
            if (filteredBatches == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");

            if (filteredBatches.Count == 0)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no batches were found");

            return DisplayBatches(filteredBatches);
        }

        public ActionResult FetchClosedBatchesByDate(Dictionary<string, object> parameters)
        {
            if (filteredBatches == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");

            if (!String.IsNullOrEmpty(parameters["date-comparator"].ToString()) || !String.IsNullOrEmpty(parameters["date-period"].ToString()))
            {
                DateRange convertedDate = DateHelper.DateFromRange(null, parameters["date-period"].ToString(), parameters["date-comparator"].ToString());
                FilterByClosedDate(convertedDate, parameters["date-comparator"].ToString());
            }

            if (filteredBatches.Count == 0)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no batches were found");

            return DisplayBatches(filteredBatches);
        }

        public ActionResult FilterBatchesBySum(Dictionary<string, object> parameters)
        {
            if (filteredBatches == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");

            if (!String.IsNullOrEmpty(parameters["amount"].ToString()) && !String.IsNullOrEmpty(parameters["comparator"].ToString()))
            {
                FetchByTotal(parameters["amount"].ToString(), parameters["comparator"].ToString());

                if (filteredBatches.Count == 0)
                    return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no batches were found");

                return DisplayBatches(filteredBatches);
            }
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "not all parameters recognized");            
        }

        public ActionResult DisplayBatches(List<Batch> batches)
        {
            List<BatchViewModel> viewableBatches = new List<BatchViewModel>();
            for(int i = 0; i < batches.Count; i++)
            {
                BatchViewModel tempModel = new BatchViewModel();
                tempModel.batch = batches.ElementAt(i);
                tempModel.count = db.Donations.Where(x => x.DonationBatch_Id == tempModel.batch.Id).Count();
                if (tempModel.count != 0)
                    tempModel.sum = db.Donations.Where(x => x.DonationBatch_Id == tempModel.batch.Id).Sum(y => y.Value);
                else
                    tempModel.sum = 0;
                viewableBatches.Add(tempModel);
            }
            return PartialView("~/Views/Batch/_FetchIndex.cshtml", viewableBatches);
        }

        public List<Batch> FindBatches(Dictionary<string, object> parameters)
        {
            filteredBatches = new List<Batch>();

            //the paramsExist variable is used to check if the list of batches must be created or filtered.
            //every call of FindBatches must include both a type and title i nthe params, even if empty
            bool paramsExist =
                !String.IsNullOrEmpty(parameters["type"].ToString())
                || !String.IsNullOrEmpty(parameters["title"].ToString())
                || !String.IsNullOrEmpty(parameters["date"].ToString())
                || !String.IsNullOrEmpty(parameters["date-period"].ToString())
                || !String.IsNullOrEmpty(parameters["id"].ToString())
                || (!String.IsNullOrEmpty(parameters["amount"].ToString()) && !String.IsNullOrEmpty(parameters["number-comparator"].ToString()));

            if (!paramsExist)
                return FetchAllBatches();

            if (!String.IsNullOrEmpty(parameters["id"].ToString()))
            {
                int batchID = 0;
                if (int.TryParse(parameters["id"].ToString(), out batchID))
                {
                    FetchByID(ref filteredBatches, batchID);
                }
                return filteredBatches;
            }

            if (!String.IsNullOrEmpty(parameters["amount"].ToString()) && !String.IsNullOrEmpty(parameters["number-comparator"].ToString()))
            {
                FetchByTotal(parameters["amount"].ToString(), parameters["number-comparator"].ToString());
                if (filteredBatches.Count == 0) goto Finish;
            }

            if (!String.IsNullOrEmpty(parameters["title"].ToString()))
            {
                FetchByTitle(parameters["title"].ToString());
                if (filteredBatches.Count == 0) goto Finish;
            }

            if (!String.IsNullOrEmpty(parameters["amount"].ToString()) && !String.IsNullOrEmpty(parameters["number-comparator"].ToString()))
            {
                FetchByTotal(parameters["amount"].ToString(), parameters["number-comparator"].ToString());
                if (filteredBatches.Count == 0) goto Finish;
            }

            if (!String.IsNullOrEmpty(parameters["type"].ToString()))
            {
                FetchByType(parameters["type"].ToString());
                if (filteredBatches.Count == 0) goto Finish;
            }

            if (!String.IsNullOrEmpty(parameters["date"].ToString()) || !String.IsNullOrEmpty(parameters["date-period"].ToString()))
            {
                DateRange convertedDate = DateHelper.DateFromRange(parameters["date"].ToString(), parameters["date-period"].ToString(), parameters["date-comparator"].ToString());
                FetchByDate(convertedDate, parameters["date-comparator"].ToString(), parameters["type"].ToString());
                if (filteredBatches.Count == 0) goto Finish;
            }

            Finish:
            return filteredBatches;
        }

        // extract method
        private void FetchByDate(DateRange searchRange, string dateComparator, string type)
        {
            SEARCH_TYPE searchType;
            if (dateComparator == "<")                      //True when searching before a certain date
                searchType = SEARCH_TYPE.BEFORE;
            else if (dateComparator == ">")                  //True when searching after a certain date
                searchType = SEARCH_TYPE.AFTER;
            else                                           //True when searching on a specific date
                searchType = SEARCH_TYPE.ON;

            BATCH_TYPE batchType;
            if (type == "closed")
                batchType = BATCH_TYPE.CLOSED;
            else
                batchType = BATCH_TYPE.OPEN;

            bool emptyList = (filteredBatches.Count == 0); //True if the list is empty
            if (emptyList)
                AddByDate(searchRange, searchType, batchType);
            else
                FilterByDate(searchRange, searchType, batchType);
        }

        private void FetchByTotal(string amount, string comparator)
        {
            int total = int.Parse(amount);
            COMPARE_TYPE compareType;
            if (comparator == "<")                      //True when searching before a certain date
                compareType = COMPARE_TYPE.UNDER;
            else if (comparator == ">")                  //True when searching after a certain date
                compareType = COMPARE_TYPE.OVER;
            else                                           //True when searching on a specific date
                compareType = COMPARE_TYPE.EQUAL;
            bool emptyList = (filteredBatches.Count == 0); //True if the list is empty

            List<int> matchingDonations = FindBatchKeysBySum(total, compareType);

            if (emptyList)
                filteredBatches.AddRange(db.Batches.Where(x => matchingDonations.Any(y => y == x.Id)));
            else
                filteredBatches = filteredBatches.Where(x => matchingDonations.Any(y => y == x.Id)).ToList();
        }

        private List<int> FindBatchKeysBySum(int amount, COMPARE_TYPE compareType)
        {
            List<int> matchingBatches = new List<int>();
            switch (compareType)
            {
                case COMPARE_TYPE.UNDER: //Searching under a value
                    matchingBatches.AddRange(db.Donations.GroupBy(x => x.DonationBatch_Id).Where(x => x.Sum(y => y.Value) < amount).Select(x => x.Key)); //The sum is under the amount
                    break;
                case COMPARE_TYPE.OVER:  //Searching over a value
                    matchingBatches.AddRange(db.Donations.GroupBy(x => x.DonationBatch_Id).Where(x => x.Sum(y => y.Value) > amount).Select(x => x.Key)); //The sum is under the amount
                    break;
                case COMPARE_TYPE.EQUAL: //Searching equal to a value
                    matchingBatches.AddRange(db.Donations.GroupBy(x => x.DonationBatch_Id).Where(x => x.Sum(y => y.Value) == amount).Select(x => x.Key)); //The sum is under the amount
                    break;
            }
            return matchingBatches;
        }

        private void FilterByClosedDate(DateRange searchRange, string dateComparator = "==")
        {
            switch (dateComparator)
            {
                case "<": //Searching before a date
                    filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CloseDate??DateTime.MaxValue, searchRange.Item1) < 0).ToList(); //The closeDate is earlier than the searchDate
                    break;
                case ">": //Searching after a date
                    filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CloseDate??DateTime.MinValue, searchRange.Item2) > 0).ToList(); //The closeDate is later than the searchDate
                    break;
                case "==":default:    //Searching on a date
                    filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CloseDate??DateTime.MinValue, searchRange.Item1) >= 0 && DateTime.Compare(x.CloseDate??DateTime.MaxValue, searchRange.Item2) <= 0).ToList(); //The closeDate is the same as the searchDate
                    break;
            }
        }

        private void FetchByID(ref List<Batch> filteredBatches, int batchID)
        {
            if (filteredBatches.Count == 0)
            {
                filteredBatches.AddRange(db.Batches.Where(x => x.Id == batchID));
            }
            else
            {
                filteredBatches = filteredBatches.Where(x => x.Id == batchID).ToList();
            }
        }

        private void FilterByDate(DateRange searchRange, SEARCH_TYPE searchType, BATCH_TYPE batchType)
        {
            switch (searchType)
            {
                case SEARCH_TYPE.BEFORE: //Searching before a date
                    if (batchType.Equals(BATCH_TYPE.OPEN))
                        filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item1) < 0).ToList(); //The createDate is earlier than the searchDate
                    else
                        filteredBatches = filteredBatches.Where(x => x.CloseDate.HasValue && DateTime.Compare(x.CloseDate.Value, searchRange.Item1) < 0).ToList();
                    break;
                case SEARCH_TYPE.AFTER: //Searching after a date
                    if (batchType.Equals(BATCH_TYPE.OPEN))
                        filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item2) > 0).ToList(); //The createDate is later than the searchDate
                    else
                        filteredBatches = filteredBatches.Where(x => x.CloseDate.HasValue && DateTime.Compare(x.CloseDate.Value, searchRange.Item2) > 0).ToList();
                    break;
                case SEARCH_TYPE.ON:    //Searching on a date
                    if (batchType.Equals(BATCH_TYPE.OPEN))
                        filteredBatches = filteredBatches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item1) >= 0 && DateTime.Compare(x.CreateDate, searchRange.Item2) <= 0).ToList(); //The createDate is the same as the searchDate
                    else
                        filteredBatches = filteredBatches.Where(x => x.CloseDate.HasValue && (DateTime.Compare(x.CloseDate.Value, searchRange.Item1) >= 0 && DateTime.Compare(x.CloseDate.Value, searchRange.Item2) <= 0)).ToList(); //The createDate is the same as the searchDate
                    break;
            }
        }

        private void AddByDate(DateRange searchRange, SEARCH_TYPE searchType, BATCH_TYPE batchType)
        {
            switch (searchType)
            {
                case SEARCH_TYPE.BEFORE: //Searching before a date
                    if(batchType.Equals(BATCH_TYPE.OPEN))
                        filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item1) < 0)); //The createDate is earlier than the searchDate
                    else
                        filteredBatches.AddRange(db.Batches.Where(x => x.CloseDate.HasValue && DateTime.Compare(x.CloseDate.Value, searchRange.Item1) < 0)); //The closeDate is earlier than the searchDate
                    break;
                case SEARCH_TYPE.AFTER:  //Searching after a date
                    if(batchType.Equals(BATCH_TYPE.OPEN))
                        filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item2) > 0)); //The createDate is later than the searchDate
                    else
                        filteredBatches.AddRange(db.Batches.Where(x => x.CloseDate.HasValue && DateTime.Compare(x.CloseDate.Value, searchRange.Item1) > 0));

                    break;
                case SEARCH_TYPE.ON:     //Searching on a date
                    if (batchType.Equals(BATCH_TYPE.OPEN))
                        filteredBatches.AddRange(db.Batches.Where(x => DateTime.Compare(x.CreateDate, searchRange.Item1) >= 0 && DateTime.Compare(x.CreateDate, searchRange.Item2) <= 0)); //The createDate is the same as the searchDate
                    else
                        filteredBatches.AddRange(db.Batches.Where(x => x.CloseDate.HasValue &&( DateTime.Compare(x.CloseDate.Value, searchRange.Item1) >= 0 && DateTime.Compare(x.CloseDate.Value, searchRange.Item2) <= 0)));

                    break;
            }
        }

        private void FetchByTitle(string Title)
        {
            //searching through the db uses LINQ, which is picky about what variables can be passed.
            //For instance, LINQ does not accept ArrayIndex variables in queries,
            //so they are individual string variables in this query instead.
            if (filteredBatches.Count == 0)
            {
                //look for batches that contain the specified title (case insensitive)
                filteredBatches.AddRange(db.Batches.Where(x => x.Title.ToUpper().Contains(Title.ToUpper())));
            }
            else
            {
                filteredBatches = filteredBatches.Where(x => x.Title.ToUpper().Contains(Title.ToUpper())).ToList();
            }
        }

        private void FetchByType(string batchStatus)
        {
            bool openBatches = !batchStatus.Equals("closed");

            if (filteredBatches.Count == 0)
            {
                if (openBatches)
                    filteredBatches.AddRange(db.Batches.Where(x => x.CloseDate == null));
                else
                    filteredBatches.AddRange(db.Batches.Where(x => x.CloseDate != null));
            }
            else
            {
                if (openBatches)
                    filteredBatches = filteredBatches.Where(x => x.CloseDate == null).ToList();
                else
                    filteredBatches = filteredBatches.Where(x => x.CloseDate != null).ToList();
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
            int batchId = Int32.Parse(parameters["batchId"].ToString());
            Batch batchToClose = db.Batches.FirstOrDefault(x => x.Id == batchId);
            if (batchToClose != null)
                return PartialView("~/Views/Batch/_CloseBatch.cshtml", batchToClose);
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "There's no batch with id " + batchId);

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
                    Helpers.Log.WriteLog(Helpers.Log.LogType.ParamsSubmitted, JsonConvert.SerializeObject(batch));
                    return PartialView("~/Views/Batch/_AddSuccess.cshtml", batch);
                }
            }
            //an invalid submission shall return the form with some validation error messages.
            return PartialView("~/Views/Batch/_AddForm.cshtml", batch);
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