using DMSLite.DataContexts;
using DMSLite.Entities;
using DMSLite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMSLite.Controllers
{
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
            donations.AddRange(db.Donations.Where(x => x.DonationBatch_Id.Equals(batchId)));
            if (donations.Count > 0)
                return PartialView("~/Views/Donation/_FetchIndex.cshtml", donations);
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "No donations in \"" + batch.Title + "\".");
        }
        #endregion

        #region Modify
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
        public ActionResult Add(Donation donation, int donationDonor, int donationBatch)
        {
            Donor actualDonor = db.Donors.First(x => x.Id == donationDonor);
            Batch actualBatch = db.Batches.First(x => x.Id == donationBatch);
            donation.DonationDonor = actualDonor;
            donation.DonationBatch = actualBatch;
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