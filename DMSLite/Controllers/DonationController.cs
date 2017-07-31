using DMSLite.DataContexts;
using DMSLite.Entities;
using DMSLite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Globalization;
using LinqKit;
using Newtonsoft.Json;
using NLog;

namespace DMSLite.Controllers
{
    using Helpers;
    using DateRange = Tuple<DateTime, DateTime>;

    [Authorize]
    public class DonationController : Controller
    {
        private OrganizationDb db;
        private static bool postedOnly = false;
        private static Logger logger = LogManager.GetLogger("serverlog");

        public DonationController()
        {
            db = new OrganizationDb();
        }

        public DonationController(OrganizationDb db)
        {
            this.db = db;
        }

        // Populate the viewbag with data from database for use in the view
        private void PopulateViewBag()
        {
            ViewBag.Donors = db.Donors.ToList();

            // Only populate open batches
            ViewBag.Batches = db.Batches.Where(x => x.CloseDate == null).ToList();

            ViewBag.Accounts = db.Accounts.ToList();
        }

        #region Fetch
        public ActionResult FetchByBatchId(Batch batch)
        {
            int batchId = batch.Id;
            List<Donation> donations = new List<Donation>();

            donations.AddRange(db.Donations
                .Include(x => x.DonationBatch)
                .Include(x => x.DonationDonor)
                .Where(x => x.DonationBatch_Id.Equals(batchId)));

            if (donations.Count > 0)
            {
                //Entity Framework needs related entities to be explicitly loaded to see their data.
                foreach(Donation donation in donations)
                    donation.DonationDonor = db.Donors.First(x => x.Id == donation.DonationDonor_Id);

                return PartialView("~/Views/Donation/_FetchIndexSolo.cshtml", donations);
            }
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "No donations in \"" + batch.Title + "\".");
        }

        public ActionResult FetchDonations(Dictionary<string, object> parameters)
        {
            if(parameters.Keys.Contains("value-range"))
            {
                //error responses related to range
                List<String> valueRangeList = JsonConvert.DeserializeObject<List<String>>(parameters["value-range"].ToString());
                if (valueRangeList.Any())
                {
                    if (valueRangeList.Count() != 2)
                        return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "that range isn't completed");
                    if (float.Parse(valueRangeList[0]) > float.Parse(valueRangeList[1]))
                        return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "that range is invalid");
                }

                //post-fetch responses
                List<Donation> filteredDonations = FindDonations(parameters);
                if (filteredDonations == null)
                    return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");

                if (filteredDonations.Count == 0)
                    return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no donations were found");
                return PartialView("~/Views/Donation/_FetchIndexSolo.cshtml", filteredDonations);
            }
            else
                return PartialView("~/Views/Donation/_FetchIndexSolo.cshtml", new List<Donation>(db.Donations.Include(x => x.DonationDonor).Include(y => y.DonationBatch).Include(z => z.DonationAccount).ToList<Donation>()));
        }

        public List<Donation> FindDonations(Dictionary<string, object> parameters)
        {
            List<Donation> returnedDonations = new List<Donation>(db.Donations.Include(x => x.DonationDonor).Include(y => y.DonationBatch).Include(z => z.DonationAccount).ToList<Donation>());
            bool paramsExist = ((((((
                !String.IsNullOrEmpty(parameters["donor-name"].ToString()) || !String.IsNullOrEmpty(parameters["value"].ToString())) ||
                        JsonConvert.DeserializeObject<List<String>>(parameters["value-range"].ToString()).Any()) ||
                                !String.IsNullOrEmpty(parameters["account-name"].ToString())) ||
                                    !String.IsNullOrEmpty(parameters["date"].ToString())) ||
                                        !String.IsNullOrEmpty(parameters["date-period"].ToString()))
            );
            if(paramsExist)
            {
                if (!String.IsNullOrEmpty(parameters["donor-name"].ToString())){
                    DonorsController dc = new DonorsController(db);
                    var donorPredicate = dc.FetchByName(parameters["donor-name"].ToString());
                    List<Donor> matchedDonors = db.Donors.AsExpandable().Where(donorPredicate).ToList();
                    FetchByDonor(ref returnedDonations, matchedDonors);
                }
                if (!String.IsNullOrEmpty(parameters["date"].ToString()) || !String.IsNullOrEmpty(parameters["date-period"].ToString()))
                {
                    DateRange convertedDate = DateHelper.DateFromRange(parameters["date"].ToString(), parameters["date-period"].ToString(), parameters["date-comparator"].ToString());
                    FetchByDate(ref returnedDonations, convertedDate, parameters["date-comparator"].ToString());
                }
                if (!String.IsNullOrEmpty(parameters["account-name"].ToString()))
                {
                    DonationAccountController dc = new DonationAccountController(db);
                    List<Account> possibleAccounts = dc.FindByTitle(parameters["account-name"].ToString());
                    FetchByAccount(ref returnedDonations, possibleAccounts);
                }
                if (!String.IsNullOrEmpty(parameters["value"].ToString()))
                {
                    FetchByValueOpenRange(ref returnedDonations, float.Parse(parameters["value"].ToString(), CultureInfo.InvariantCulture.NumberFormat), parameters["value-comparator"].ToString());
                }
                if (JsonConvert.DeserializeObject<List<String>>(parameters["value-range"].ToString()).Any())
                {
                    List<String> valueRangeList = JsonConvert.DeserializeObject<List<String>>(parameters["value-range"].ToString());
                    FetchByValueClosedRange(ref returnedDonations, float.Parse(valueRangeList[0]), float.Parse(valueRangeList[1]));
                }                
            }
            if (postedOnly)
                FetchPostedDonations(ref returnedDonations);
            return returnedDonations;
        }

        public ActionResult TogglePostedDisplay()
        {
            postedOnly = !postedOnly;
            return PartialView("~/Views/Donation/_ToggleDisplay.cshtml", postedOnly);
        }

        public void FetchPostedDonations(ref List<Donation> returnedDonations)
        {
            List<int> postedBatches = db.Batches.Where(x => x.CloseDate.HasValue).Select(x => x.Id).ToList();
            returnedDonations = returnedDonations.Where(x => postedBatches.Contains(x.DonationBatch_Id)).ToList();
        }

        public void FetchByDate(ref List<Donation> returnedDonations, DateRange dateRange, string dateComparator)
        {
            if (dateRange == null)
            {//if the date was converted to invalid, empty the list
                returnedDonations = new List<Donation>();
                return;                                                                                                                                                                                                                                                                                                                                                                                                                                   
            }
            if (dateRange.Item1.Equals(dateRange.Item2))
            {
                switch (dateComparator)
                {
                    case "<":
                        returnedDonations = returnedDonations.Where(x => x.DonationBatch.CreateDate.Date <= dateRange.Item1.Date).ToList();
                        break;
                    case ">":
                        returnedDonations = returnedDonations.Where(x => x.DonationBatch.CreateDate.Date >= dateRange.Item1.Date).ToList();
                        break;
                    default:
                        returnedDonations = returnedDonations.Where(x => x.DonationBatch.CreateDate.Date == dateRange.Item1.Date).ToList();
                        break;
                }
            }
            //for a date range
            else
            {
                switch (dateComparator)
                {
                    case "<":
                        returnedDonations = returnedDonations.Where(x => x.DonationBatch.CreateDate <= dateRange.Item1).ToList();
                        break;
                    case ">":
                        returnedDonations = returnedDonations.Where(x => x.DonationBatch.CreateDate >= dateRange.Item2).ToList();
                        break;
                    default:
                        returnedDonations = returnedDonations.Where(x => x.DonationBatch.CreateDate >= dateRange.Item1 && x.DonationBatch.CreateDate <= dateRange.Item2).ToList();
                        break;
                }
            }
        }

        public void FetchByAccount(ref List<Donation> returnedDonations, List<Account> accounts)
        {
            List<Donation> matchingDonations = new List<Donation>();
            foreach (Donation d in returnedDonations)
            {
                if((accounts.Where(x => x.Id == d.DonationAccount_Id)).Count() > 0)
                {
                    matchingDonations.Add(d);
                }
            }
            returnedDonations = returnedDonations.Intersect(matchingDonations).ToList();
        }

        // extract method
        public void FetchByDonor(ref List<Donation> filteredDonations, List<Donor> donors)
        {
            //NOTE: This filter needs to be applied FIRST if other filters are to be applied
            filteredDonations = new List<Donation>();
            List<Donation> allDonations = db.Donations.ToList<Donation>();
            foreach (Donor d in donors)
            {
                List<Donation> filteredDonationsPerDonor = allDonations.Where(x => x.DonationDonor_Id == d.Id).ToList();
                filteredDonations.AddRange(filteredDonationsPerDonor);
            }
        }

        public void FetchByValueOpenRange(ref List<Donation> filteredDonations, float value, string comparator)
        {
            switch (comparator)
            {
                case "<":
                    filteredDonations = filteredDonations.Where(x => x.Value < value).ToList();
                    break;
                case ">":
                    filteredDonations = filteredDonations.Where(x => x.Value > value).ToList();
                    break;
                default:
                    //search for donations exactly at that value
                    filteredDonations = filteredDonations.Where(x => x.Value == value).ToList();
                    break;
            }
        }

        public void FetchByValueClosedRange(ref List<Donation> filteredDonations, float valueMin, float valueMax)
        {
            filteredDonations = filteredDonations.Where(x => x.Value >= valueMin && x.Value <= valueMax).ToList();
        }

        public double SumNonReceiptedDonations()
        {
            double value = 0;
            List<Donation> donations = db.Donations.Where(x => x.Gift && x.DonationReceipt_Id == 0
                && (x.DonationDonor.Type == DonorType.Individual || x.DonationDonor.Type == DonorType.Business) ).ToList();
            foreach(var donation in donations)
                value += donation.Value;
            return value;
        }

        #endregion

        #region Modify
        public ActionResult ModifyFromDonation(Donation donation)
        {
            donation.DonationDonor = db.Donors.First(x => x.Id == donation.DonationDonor_Id);
            donation.DonationBatch = db.Batches.First(x => x.Id == donation.DonationBatch_Id);
            if(donation.DonationAccount_Id.HasValue)
                donation.DonationAccount = db.Accounts.First(x => x.Id == donation.DonationAccount_Id);

            if (donation.DonationBatch.CloseDate == null)
            {
                PopulateViewBag();
                return PartialView("~/Views/Donation/_Modify.cshtml", donation);
            }
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "This donation cannot be edited: batch \"" + donation.DonationBatch.Title + "\"is closed.");
        }

        public ActionResult Modify(Donation donation, int? donationDonor, int? donationBatch, int? donationAccount = null)
        {
            if(!donationDonor.HasValue || !donationBatch.HasValue)
            {
                if (!donationDonor.HasValue)
                    ModelState.AddModelError("DonationDonor", "A donor is required");

                if (!donationBatch.HasValue)
                    ModelState.AddModelError("DonationDonor", "A batch is required");

                PopulateViewBag();
                return PartialView("~/Views/Donation/_ModifyForm.cshtml", donation);
            }

            Donor actualDonor = db.Donors.First(x => x.Id == donationDonor);
            Batch actualBatch = db.Batches.First(x => x.Id == donationBatch);
            Account actualAccount = null;
            if (donationAccount.HasValue)
            {
                actualAccount = db.Accounts.First(x => x.Id == donationAccount);
            }
            if (actualBatch.CloseDate == null)
            {
                donation.DonationDonor = actualDonor;
                donation.DonationDonor_Id = actualDonor.Id;
                donation.DonationBatch = actualBatch;
                donation.DonationBatch_Id = actualBatch.Id;
                if (donationAccount.HasValue)
                {
                    donation.DonationAccount = actualAccount;
                    donation.DonationAccount_Id = donationAccount;
                }
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                    TryValidateModel(donation);
                }
                if (ModelState.IsValid)
                {
                    db.Modify(donation);
                    return PartialView("~/Views/Donation/_ModifySuccess.cshtml", donation);
                }

                PopulateViewBag();
                return PartialView("~/Views/Donation/_ModifyForm.cshtml", donation);
            }
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "This donation cannot be edited: batch \"" + donation.DonationBatch.Title + "\"is closed.");
        }
        #endregion

        #region Add
        public ActionResult AddMenu(Dictionary<string, object> parameters)
        {
            if(parameters.Keys.Contains("batchId"))
            {
                if (!String.IsNullOrEmpty(parameters["batchId"].ToString()))
                {
                    int batchId = Int32.Parse(parameters["batchId"].ToString());
                    Batch batch = db.Batches.FirstOrDefault(x => x.Id == batchId);
                    if (batch == null)
                        return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "There's no batch with id " + parameters["batchId"].ToString());
                    else if (batch.CloseDate != null)
                        return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "Batch " + batch.Id + " has already been posted. Can't add a new donation to it.");
                }
            }            
            return PartialView("~/Views/Donation/_Add.cshtml", parameters);
        }

        public ActionResult AddForm(Dictionary<string, object> parameters)
        {
            Donation newDonation = new Donation();
            AddDonationViewModel viewModel = new AddDonationViewModel { Donation = newDonation };

            //found this, its not used in api.ai - DK
            if (parameters.ContainsKey("value"))
            {
                double donationValue;
                Double.TryParse(parameters["value"].ToString(), out donationValue);
                newDonation.Value = donationValue;
            }

            //id here is batchId
            if (parameters.ContainsKey("id") && !String.IsNullOrEmpty(parameters["id"].ToString()))
            {
                //guaranteed to be valid at this point
                int batchId = Int32.Parse(parameters["id"].ToString());
                Batch batch = db.Batches.FirstOrDefault(x => x.Id == batchId);
                newDonation.DonationBatch = batch;
                newDonation.DonationBatch_Id = batch.Id;
            }
            //batchId here is batchId, same behavior as above, unsure as to why api.ai ir returning these keys
            if (parameters.ContainsKey("batchId") && !String.IsNullOrEmpty(parameters["batchId"].ToString()))
            {
                //guaranteed to be valid at this point
                int batchId = Int32.Parse(parameters["batchId"].ToString());
                Batch batch = db.Batches.FirstOrDefault(x => x.Id == batchId);
                newDonation.DonationBatch = batch;
                newDonation.DonationBatch_Id = batch.Id;
            }
            //found this, its not used in api.ai - DK
            if (parameters.ContainsKey("description"))
            {
                newDonation.ObjectDescription = parameters["description"].ToString();
            }
            //found this, its not used in api.ai - DK
            if (parameters.ContainsKey("orgId"))
            {
                int orgId;
                Int32.TryParse(parameters["orgId"].ToString(), out orgId);
                newDonation.Value = orgId;
            }
            //found this, its not used in api.ai - DK
            if (parameters.ContainsKey("donor"))
            {
                string donorName = parameters["donor"].ToString();
                List<Donor> donors = db.Donors.ToList();
                FetchByName(ref donors, donorName);
                if (donors.Count == 1)
                {
                    newDonation.DonationDonor = donors.First();
                }
                else if(donors.Count > 1)
                {
                    viewModel.SimilarDonors = donors;
                }
            }

            PopulateViewBag();

            // When api.ai can parse out the donor and batch names, we can test a flow
            // for similar objects. We can store the viewModel in a session, and pass
            // the session through the flow, for now just show the new donation form
            if (viewModel.SimilarDonors != null && viewModel.SimilarDonors.Count > 1)
            {
                //return PartialView("~/Views/Donation/_AddDonationSimilarDonor.cshtml", viewModel);
                return PartialView("~/Views/Donation/_AddForm.cshtml", newDonation);
            }
            else if (viewModel.SimilarBatches != null && viewModel.SimilarBatches.Count > 1)
            {
                //return PartialView("~/Views/Donation/_AddDonationSimilarBatch.cshtml", viewModel);
                return PartialView("~/Views/Donation/_AddForm.cshtml", newDonation);
            }
            else
            {
                return PartialView("~/Views/Donation/_AddForm.cshtml", newDonation);
            }            
        }

        // TODO: Anti-forgery
        public ActionResult Add(Donation donation, int? donationDonor, int? donationBatch, int? donationAccount = null, bool isGift = false)
        {
            if (!donationDonor.HasValue || !donationBatch.HasValue)
            {
                if (!donationDonor.HasValue)
                    ModelState.AddModelError("DonationDonor", "A donor is required");

                if (!donationBatch.HasValue)
                    ModelState.AddModelError("DonationDonor", "A batch is required");

                PopulateViewBag();
                return PartialView("~/Views/Donation/_AddForm.cshtml", donation);
            }

            donation.Gift = isGift;
            Donor actualDonor = db.Donors.First(x => x.Id == donationDonor);
            Batch actualBatch = db.Batches.First(x => x.Id == donationBatch);
            Account actualAccount = null;
            if (donationAccount.HasValue)
            {
                actualAccount = db.Accounts.First(x => x.Id == donationAccount);
            }
            if (actualBatch.CloseDate == null)
            {
                donation.DonationDonor = actualDonor;
                donation.DonationBatch = actualBatch;
                if (donationAccount.HasValue)
                {
                    donation.DonationAccount = actualAccount;
                }                    
                if (!ModelState.IsValid)
                {
                    ModelState.Clear();
                    TryValidateModel(donation);
                }
                if (ModelState.IsValid)
                {
                    db.Add(donation);
                    logger.Info(JsonConvert.SerializeObject(donation));
                    return PartialView("~/Views/Donation/_AddSuccess.cshtml", donation);
                }
            }
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "This donation cannot be added: batch \"" + donation.DonationBatch.Title + "\"is closed.");

            PopulateViewBag();
            return PartialView("~/Views/Donation/_AddForm.cshtml", donation);
        }

        public ActionResult AddFromBatch(Batch batch)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("batchId", batch.Id);
            parameters.Add("title", "");
            return AddMenu(parameters);
        }

        private void FetchByName(ref List<Donor> list, string name)
        {
            //searching through the db uses LINQ, which is picky about what variables can be passed.
            //For instance, LINQ does not accept ArrayIndex variables in queries,
            //so they are individual string variables in this query instead.
            string[] names = name.Split(' ');
            if (names.Count() == 2)
            {
                string name1 = names[0], name2 = names[1];
                list.AddRange(db.Donors.Where(x => x.FirstName.Equals(name1, StringComparison.InvariantCultureIgnoreCase) &&
                    x.LastName.Equals(name2, StringComparison.InvariantCultureIgnoreCase)));
                //reverse
                list.AddRange(db.Donors.Where(x => x.FirstName.Equals(name2, StringComparison.InvariantCultureIgnoreCase) &&
                    x.LastName.Equals(name1, StringComparison.InvariantCultureIgnoreCase)));
            }
            else
            {
                list.AddRange(db.Donors.Where(x => x.FirstName.Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
                    x.LastName.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
            }
        }
        #endregion

        #region Delete
        public ActionResult ShowDeleteFromDonation(Donation item)
        {
            return PartialView("~/Views/Donation/_Delete.cshtml", item);
        }

        public void DeleteFromDonation(int id)
        {
            db.Donations.Remove(db.Donations.First(x => x.Id == id));
            db.SaveChanges();
        }
        #endregion

        #region MadeByMs
        // GET: Batch
        public ActionResult Remove(Donation donation)
        {
            if (ModelState.IsValid)
            {
                db.Donations.Remove(donation);
                db.SaveChanges();
                return Content("Removed", "text/html");
            }
            return PartialView("~/Views/Donation/_Add.cshtml", donation);
            //TODO: make sure the name field is recognized as valid by api.ai
        }

        public ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}