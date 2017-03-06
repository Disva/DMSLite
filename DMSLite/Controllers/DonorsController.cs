using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DMSLite.DataContexts;
using DMSLite.Entities;
using DMSLite.Controllers;
using DMSLite.Models;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using LinqKit;
using Newtonsoft.Json;

namespace DMSLite
{
    [Authorize]
    public class DonorsController : Controller
    {
        private OrganizationDb db;

        private static List<Donor> filteredDonors;

        public DonorsController()
        {
            db = new OrganizationDb();
        }

        public DonorsController(OrganizationDb db)
        {
            this.db = db;
        }

        #region Fetch

        public Expression<Func<Donor, bool>> FetchByName(string name)
        {
            //searching through the db uses LINQ, which is picky about what variables can be passed.
            //For instance, LINQ does not accept ArrayIndex variables in queries,
            //so they are individual string variables in this query instead.
            string[] names = name.Split(' ');

            var predicate = PredicateBuilder.New<Donor>();


            if (names.Count() == 2)
            {
                string name1 = names[0], name2 = names[1];

                predicate.Or(x => x.FirstName != null && x.LastName != null && x.FirstName.Equals(name1, StringComparison.InvariantCultureIgnoreCase) &&
                    x.LastName.Equals(name2, StringComparison.InvariantCultureIgnoreCase));

                predicate.Or(x => x.FirstName != null && x.LastName != null && x.FirstName.Equals(name2, StringComparison.InvariantCultureIgnoreCase) &&
                    x.LastName.Equals(name1, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                predicate = PredicateBuilder.New<Donor>(x => (x.FirstName != null && x.FirstName.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ||
                    (x.LastName != null && x.LastName.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
            }

            return predicate;
        }

        private Expression<Func<Donor, bool>> FetchByEmail(string email)
        {
            return PredicateBuilder.New<Donor>(x => x.Email != null && x.Email.Equals(email));
        }

        private Expression<Func<Donor, bool>> FetchByPhoneNumber(string phone)
        {
            return PredicateBuilder.New<Donor>(x => x.PhoneNumber != null && x.PhoneNumber.Replace("-", "").Equals(phone));
        }

        public ActionResult FetchDonor(Dictionary<string, object> parameters) //Main method to search for donors, parameters may or may not be used
        {
            filteredDonors = FindDonors(parameters);

            if (filteredDonors == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");

            if (filteredDonors.Count == 0)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no donors were found");

            return PartialView("~/Views/Donors/_FetchIndex.cshtml", filteredDonors);
        }

        public List<Donor> FindDonors(Dictionary<string, object> parameters)
        {
            var donorSearchPredicate = PredicateBuilder.New<Donor>();

            //the paramsExist variable is used to check if the list of filtered donors must be created or filtered.
            bool paramsExist =
                !String.IsNullOrEmpty(parameters["name"].ToString())
                || !String.IsNullOrEmpty(parameters["email-address"].ToString())
                || !String.IsNullOrEmpty(parameters["phone-number"].ToString());

            if (!String.IsNullOrEmpty(parameters["name"].ToString()))
                donorSearchPredicate.And(FetchByName(parameters["name"].ToString()));

            if (!String.IsNullOrEmpty(parameters["email-address"].ToString()))
                donorSearchPredicate.And(FetchByEmail(parameters["email-address"].ToString()));

            if (!String.IsNullOrEmpty(parameters["phone-number"].ToString()))
                donorSearchPredicate.And(FetchByPhoneNumber(parameters["phone-number"].ToString()));

            if (paramsExist)
                return db.Donors.AsExpandable().Where(donorSearchPredicate).ToList();
            else
                return null;
        }

        public ActionResult FilterDonors(Dictionary<string, object> parameters)
        {
            if (filteredDonors == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");

            if (filteredDonors.Count == 0)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "cannot filter an empty list");

            bool paramsExist =
                !String.IsNullOrEmpty(parameters["name"].ToString())
                || !String.IsNullOrEmpty(parameters["email-address"].ToString())
                || !String.IsNullOrEmpty(parameters["phone-number"].ToString());
            if (paramsExist)
                FilterDonorList(parameters);
            else
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");

            return PartialView("~/Views/Donors/_FetchIndex.cshtml", filteredDonors);
        }

        public void FilterDonorList(Dictionary<string, object> parameters)
        {
            var donorSearchPredicate = PredicateBuilder.New<Donor>();

            if (!String.IsNullOrEmpty(parameters["name"].ToString()))
                donorSearchPredicate.And(FetchByName(parameters["name"].ToString()));

            if (!String.IsNullOrEmpty(parameters["email-address"].ToString()))
                donorSearchPredicate.And(FetchByEmail(parameters["email-address"].ToString()));

            if (!String.IsNullOrEmpty(parameters["phone-number"].ToString()))
                donorSearchPredicate.And(FetchByPhoneNumber(parameters["phone-number"].ToString()));

            filteredDonors = filteredDonors.Where(donorSearchPredicate).ToList();
        }

        public ActionResult ViewAllDonors()
        {
            filteredDonors = db.Donors.ToList();
            return PartialView("~/Views/Donors/_FetchIndex.cshtml", filteredDonors);
        }

        // Action to search for donors by name and obtain a json result
        public ActionResult SearchDonors(string searchKey)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                return new JsonResult { Data = new { results = new List<Donor>() }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            var donors = db.Donors.Where(x => x.FirstName.ToLower().StartsWith(searchKey.ToLower()) || x.LastName.ToLower().StartsWith(searchKey.ToLower()));
            return new JsonResult { Data = new { results = donors.Select(x => new { firstName = x.FirstName, lastName = x.LastName, id = x.Id }) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region Modify

        public ActionResult ModifyForm(Dictionary<string, object> parameters)
        {
            List<Donor> matchingDonors = FindDonors(parameters);
            if (matchingDonors == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");
            else if (matchingDonors.Count == 0)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no donors were found");
            else if (matchingDonors.Count > 1)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "more than one donor was found");
            else
                return PartialView("~/Views/Donors/_Modify.cshtml", matchingDonors.First());
        }

        public ActionResult ModifyFromDonor(Donor donor)
        {
            return PartialView("~/Views/Donors/_Modify.cshtml", donor);
        }

        public ActionResult Modify(Donor donor, [Bind(Prefix = "Type.Type")] DonorType type = DonorType.Individual)
        {
            donor.Type = type;

            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                TryValidateModel(donor);
            }

            if (ModelState.IsValid)
            {
                db.Modify(donor);
                return PartialView("~/Views/Donors/_ModifySuccess.cshtml", donor);
            }

            //an invalid submission should just return the form.

            return PartialView("~/Views/Donors/_ModifyForm.cshtml", donor);
        }

        #endregion

        #region Add

        public ActionResult AddMenu(Dictionary<string, object> parameters)
        {
            return PartialView("~/Views/Donors/_Add.cshtml", parameters);
        }

        public ActionResult AddForm(Dictionary<string, object> parameters)
        {
            Donor newDonor = new Donor();

            // Split off the last word in the full name as the last name
            // If only one word is present, set the first name
            if (parameters.ContainsKey("name"))
            {
                string name = parameters["name"].ToString();

                int lastSpace = name.Trim().LastIndexOf(" ");

                if (lastSpace < 0 || lastSpace + 1 > name.Length)
                {
                    newDonor.FirstName = name;
                }
                else
                {
                    newDonor.FirstName = name.Substring(0, lastSpace).Trim();
                    newDonor.LastName = name.Substring(lastSpace + 1);
                }
            }
            if (parameters.ContainsKey("phone-number"))
                newDonor.PhoneNumber = parameters["phone-number"].ToString();
            if (parameters.ContainsKey("email"))
                newDonor.Email = parameters["email"].ToString();
            return PartialView("~/Views/Donors/_AddForm.cshtml", newDonor);
        }

        // Show an add form for a donor
        public ActionResult ShowAddForm(Donor donor)
        {
            return PartialView("~/Views/Donors/_AddForm.cshtml", donor);
        }

        // TODO: Anti-forgery
        public ActionResult Add(Donor donor, [Bind(Prefix = "Type.Type")] DonorType type = DonorType.Individual)
        {
            donor.Type = type;

            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                TryValidateModel(donor);
            }

            if (ModelState.IsValid)
            {
                //confirm with the person submitting the form whether a similar donor already exists

                //fetch a list of similar donors
                List<Donor> sd = db.Donors.Where(x =>
                (x.FirstName == donor.FirstName && x.LastName == donor.LastName) ||
                (x.Email != null && x.Email == donor.Email) ||
                (x.PhoneNumber != null && x.PhoneNumber == donor.PhoneNumber)).ToList();

                if (sd.Count() > 0)
                {
                    //return a view showing those similar donors if they exist
                    SimilarDonorModel sdm = new SimilarDonorModel() { newDonor = donor, similarDonors = sd };
                    return PartialView("~/Views/Donors/_AddSimilar.cshtml", sdm);
                }
                else
                {
                    db.Add(donor);
                    Helpers.Log.WriteLog(Helpers.Log.LogType.ParamsSubmitted, JsonConvert.SerializeObject(donor));
                    return PartialView("~/Views/Donors/_AddSuccess.cshtml", donor);
                }
            }

            //an invalid submission shall return the form with some validation error messages.
            return PartialView("~/Views/Donors/_AddForm.cshtml", donor);
        }

        //this method should only fire after the user has confirmed thy want to add a similar donor
        //(same phone number, email, or first name/ last name combo)
        public ActionResult AddSimilar(Donor donor)
        {
            db.Add(donor);
            return PartialView("~/Views/Donors/_AddSuccess.cshtml", donor);
        }

        #endregion

        #region MadeByMS
        public ActionResult Remove(Donor donor)
        {
            if (ModelState.IsValid)
            {
                db.Donors.Remove(donor);
                db.SaveChanges();
                return Content("Removed", "text/html");
            }
            return PartialView("~/Views/Donors/_Add.cshtml", donor);
            //TODO: make sure the name field is recognized as valid by api.ai
        }

        // GET: Donors
        public ActionResult Index()
        {
            return View(db.Donors.ToList());
        }

        // GET: Donors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donor donor = db.Donors.Find(id);
            if (donor == null)
            {
                return HttpNotFound();
            }
            return View(donor);
        }

        // GET: Donors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Donors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Firstame,LastName,Email,PhoneNumber,Type,ReceiptFrequency")] Donor donor)
        {
            if (ModelState.IsValid)
            {
                db.Donors.Add(donor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(donor);
        }

        // GET: Donors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donor donor = db.Donors.Find(id);
            if (donor == null)
            {
                return HttpNotFound();
            }
            return View(donor);
        }

        // POST: Donors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,PhoneNumber,Type,ReceiptFrequency")] Donor donor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(donor);
        }

        // GET: Donors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donor donor = db.Donors.Find(id);
            if (donor == null)
            {
                return HttpNotFound();
            }
            return View(donor);
        }

        // POST: Donors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Donor donor = db.Donors.Find(id);
            db.Donors.Remove(donor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
