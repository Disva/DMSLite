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

namespace DMSLite
{
    public class DonorsController : Controller
    {
        private OrganizationDb db = new OrganizationDb();

        #region Fetch
        public void Validate(Donor donor)
        {
            string phoneNumberCheck = "";
            if (!String.IsNullOrWhiteSpace(donor.PhoneNumber))
                phoneNumberCheck = Regex.Replace(donor.PhoneNumber, "[^\\d]", "");

            //Custom validation error messages are added.

            if (!String.IsNullOrWhiteSpace(donor.Email)
                && !Regex.IsMatch(donor.Email, "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"))
                ModelState.AddModelError("Email", "Email is invalid.");

            if (!String.IsNullOrWhiteSpace(donor.PhoneNumber) && !Regex.IsMatch(phoneNumberCheck, "\\d{10}"))
                ModelState.AddModelError("PhoneNumber", "Phone number is invalid.");
        }

        public string FormatValidPhoneNumber(string phoneNumber)
        {
            phoneNumber = Regex.Replace(phoneNumber, "[^\\d]", "");

            //This method currently assumes the phone number is at least ten digits and does not feature a "+1".
            string s1 = phoneNumber.Substring(0, 3);
            string s2 = phoneNumber.Substring(3, 3);
            string s3 = phoneNumber.Substring(6, 4);
            string s4 = "";
            if (phoneNumber.Length > 10)
                s4 = phoneNumber.Substring(10);

            return s1 + "-" + s2 + "-" + s3 + ((s4 == "") ? "" : " " + s4);
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

        private void FetchByEmail(ref List<Donor> list, string email)
        {
            if (list.Count == 0)//to add new
                list.AddRange(db.Donors.Where(x => x.Email.Equals(email)));
            else//to filter
                list = list.Where(x => x.Email.Equals(email)).ToList();
        }

        private void FetchByPhoneNumber(ref List<Donor> list, string phone)
        {
            if (list.Count == 0)//to add new
                list.AddRange(db.Donors.Where(x => x.PhoneNumber.Equals(phone)));
            else//to filter
                list = list.Where(x => x.PhoneNumber.Equals(phone)).ToList();
        }

        public ActionResult FetchDonor(Dictionary<string, object> parameters) //Main method to search for donors, parameters may or may not be used
        {
            List<Donor> filteredDonors = FindDonors(parameters);

            if (filteredDonors == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");

            if (filteredDonors.Count == 0)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no donors were found");

            return PartialView("~/Views/Donors/_FetchIndex.cshtml", filteredDonors);
        }

        public List<Donor> FindDonors(Dictionary<string, object> parameters)
        {
            List<Donor> filteredDonors = new List<Donor>();

            //the paramsExist variable is used to check if the list of filtered donors must be created or filtered.
            bool paramsExist =
                !String.IsNullOrEmpty(parameters["name"].ToString())
                || !String.IsNullOrEmpty(parameters["email-address"].ToString())
                || !String.IsNullOrEmpty(parameters["phone-number"].ToString());

            //FetchByName creates, but never Filters, so far place first
            if (!String.IsNullOrEmpty(parameters["name"].ToString()))
                FetchByName(ref filteredDonors, parameters["name"].ToString());

            if (!String.IsNullOrEmpty(parameters["email-address"].ToString()))
                FetchByEmail(ref filteredDonors, parameters["email-address"].ToString());

            if (!String.IsNullOrEmpty(parameters["phone-number"].ToString()))
                FetchByPhoneNumber(ref filteredDonors, parameters["phone-number"].ToString());

            if (paramsExist)
                return filteredDonors;
            else
                return null;
        }

        public ActionResult ViewAllDonors()
        {
            List<Donor> allDonors = db.Donors.ToList();
            return PartialView("~/Views/Donors/_FetchIndex.cshtml", allDonors);
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

        public ActionResult Modify(Donor donor)
        {
            Validate(donor);

            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(donor.PhoneNumber))
                    donor.PhoneNumber = FormatValidPhoneNumber(donor.PhoneNumber);

                db.Entry(donor).State = EntityState.Modified;
                db.SaveChanges();
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

        // TODO: Anti-forgery
        public ActionResult Add(Donor donor)
        {
            Validate(donor);

            if (ModelState.IsValid && donor.isValid())
            {
                if (!String.IsNullOrWhiteSpace(donor.PhoneNumber))
                    donor.PhoneNumber = FormatValidPhoneNumber(donor.PhoneNumber);
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
                    db.Donors.Add(donor);
                    db.SaveChanges();
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
            db.Donors.Add(donor);
            db.SaveChanges();
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
