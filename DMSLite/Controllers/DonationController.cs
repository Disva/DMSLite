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

namespace DMSLite.Controllers
{
    using DateRange = Tuple<DateTime, DateTime>;

    [Authorize]
    public class DonationController : Controller
    {
        private OrganizationDb db;

        public DonationController()
        {
            db = new OrganizationDb();
        }

        public DonationController(OrganizationDb db)
        {
            this.db = db;
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
                    DateRange convertedDate = DateFromRange(parameters["date"].ToString(), parameters["date-period"].ToString());
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
            return returnedDonations;
        }

        private void FetchByDate(ref List<Donation> returnedDonations, DateRange dateRange, string dateComparator)
        {
            returnedDonations = returnedDonations.Where(x=> x.DonationBatch.CreateDate.Date == dateRange.Item1.Date).ToList();
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

        //originally from batch controller
        //refactor into a helper class
        private DateRange DateFromRange(string date, string datePeriod)
        {
            if (!String.IsNullOrWhiteSpace(date));
            {
                DateTime dateValue = ConvertDate(date);
                return Tuple.Create(StartOfDay(dateValue), EndOfDay(dateValue));
            }
            if (!String.IsNullOrWhiteSpace(datePeriod))
                return Tuple.Create(
                    StartOfDay(ConvertDate(datePeriod.Split('/')[0])),
                    EndOfDay(ConvertDate(datePeriod.Split('/')[1]))
                    );
            return null;
        }

        private DateTime ConvertDate(string date)
        {
            DateTime convertedDate;
            if (date.Length == 4)
                convertedDate = DateTime.ParseExact((date + "-01-01"), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            else
                convertedDate = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            //If date in the future, send it into the past (only year for year basis)
            if (convertedDate.CompareTo(DateTime.Today) > 0)
                convertedDate = DateTime.Today.Year - convertedDate.Year == 0 ? convertedDate.AddYears(-1) : convertedDate.AddYears(DateTime.Today.Year - convertedDate.Year);

            return convertedDate;
        }

        private DateTime StartOfDay(DateTime dateTime)
        {
            return dateTime.Date;
        }

        private DateTime EndOfDay(DateTime dateTime)
        {
            return StartOfDay(dateTime).AddDays(1).AddTicks(-1);
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
                return PartialView("~/Views/Donation/_Modify.cshtml", donation);
            }
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "This donation cannot be edited: batch \"" + donation.DonationBatch.Title + "\"is closed.");
        }

        public ActionResult Modify(Donation donation, int donationDonor, int donationBatch, int? donationAccount = null)
        {
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
                donation.DonationBatch_Id = donationBatch;
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
                return PartialView("~/Views/Donation/_ModifyForm.cshtml", donation);
            }
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "This donation cannot be edited: batch \"" + donation.DonationBatch.Title + "\"is closed.");
        }
        #endregion

        #region Add
        public ActionResult AddMenu(Dictionary<string, object> parameters)
        {
            return PartialView("~/Views/Donation/_Add.cshtml", parameters);
        }

        public ActionResult AddForm(Dictionary<string, object> parameters)
        {
            Donation newDonation = new Donation();
            AddDonationViewModel viewModel = new AddDonationViewModel { Donation = newDonation };

            if (parameters.ContainsKey("value"))
            {
                double donationValue;
                Double.TryParse(parameters["value"].ToString(), out donationValue);
                newDonation.Value = donationValue;
            }
            if (parameters.ContainsKey("batch"))
            {
                var batches = db.Batches.Where(x => x.Title.Equals(parameters["batch"].ToString(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                if(batches.Count == 1)
                {
                    newDonation.DonationBatch = batches[0];
                }
                else if(batches.Count > 1)
                {
                    viewModel.SimilarBatches = batches;
                }
            }
            if (parameters.ContainsKey("description"))
            {
                newDonation.ObjectDescription = parameters["description"].ToString();
            }
            if (parameters.ContainsKey("orgId"))
            {
                int orgId;
                Int32.TryParse(parameters["orgId"].ToString(), out orgId);
                newDonation.Value = orgId;
            }
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
            
            // When api.ai can parse out the donor and batch names, we can test a flow
            // for similar objects. We can store the viewModel in a session, and pass
            // the session through the flow, for now just show the new donation form
            if(viewModel.SimilarDonors != null && viewModel.SimilarDonors.Count > 1)
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
        public ActionResult Add(Donation donation, int donationDonor, int donationBatch, int? donationAccount = null)
        {
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
                    return PartialView("~/Views/Donation/_AddSuccess.cshtml", donation);
                }
            }
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "This donation cannot be added: batch \"" + donation.DonationBatch.Title + "\"is closed.");
            return PartialView("~/Views/Donation/_AddForm.cshtml", donation);
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