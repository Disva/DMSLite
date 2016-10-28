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

namespace DMSLite
{
    public class DonorsController : Controller
    {
        private OrganizationDb db = new OrganizationDb();

        public ActionResult FetchDonor(Dictionary<string, object> parameters) //Main method to search for donors, parameters may or may not be used
        {
            List<Donor> filteredDonors = new List<Donor>();

            //the paramsExist variable is used to check if the list of filtered donors must be created or filtered.
            bool paramsExist = false;

            if (parameters.ContainsKey("name") && !String.IsNullOrEmpty(parameters["name"].ToString()))
            {
                string name = parameters["name"].ToString();

                //for now, a name parameter containing a space is presumed to be a first name and last name
                if(name.Contains(" "))
                {
                    string[] names = name.Split(new char[] { ' ' });
                    //searching through the db uses LINQ, which is picky about what variables can be passed.
                    //For instance, LINQ does not accept ArrayIndex variables in queries,
                    //so they are individual string variables in this query instead.
                    string name1 = names[0];
                    string name2 = names[1];
                    filteredDonors.AddRange(db.Donors.Where(x=> x.FirstName.Equals(name1, StringComparison.InvariantCultureIgnoreCase) &&
                    x.LastName.Equals(name2, StringComparison.InvariantCultureIgnoreCase)));

                    filteredDonors.AddRange(db.Donors.Where(x => x.FirstName.Equals(name2, StringComparison.InvariantCultureIgnoreCase) &&
                    x.LastName.Equals(name1, StringComparison.InvariantCultureIgnoreCase)));
                }
                //a name without a space is presumed to either be a first or last name
                else
                {
                    filteredDonors.AddRange(db.Donors.Where(x => x.FirstName.Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
                    x.LastName.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
                }
                //confirm that a parameter was used to create a list for later filtering
                paramsExist = true;
            }

            if (parameters.ContainsKey("email-address") && !String.IsNullOrEmpty(parameters["email-address"].ToString()))
            {
                string email = parameters["email-address"].ToString();
                if (filteredDonors.Count == 0 && !paramsExist)//to add new
                    filteredDonors.AddRange(db.Donors.Where(x => x.Email.Equals(email)));
                else if(filteredDonors.Count != 0 && paramsExist)//to filter
                    filteredDonors = filteredDonors.Where(x => x.Email.Equals(email)).ToList();
                paramsExist = true;
            }

            if (parameters.ContainsKey("phone-number") && !String.IsNullOrEmpty(parameters["phone-number"].ToString()))
            {
                string phone = parameters["phone-number"].ToString();
                if (filteredDonors.Count == 0 && !paramsExist)//to add new
                    filteredDonors.AddRange(db.Donors.Where(x => x.PhoneNumber.Equals(phone)));
                else if (filteredDonors.Count != 0 && paramsExist)//to filter
                    filteredDonors = filteredDonors.Where(x => x.PhoneNumber.Equals(phone)).ToList();
                paramsExist = true;
            }

            if (filteredDonors.Count == 0 && paramsExist)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no donors were found");

            if (paramsExist) //If at least one parameter's value was non-empty
                return PartialView("~/Views/Donors/_FetchIndex.cshtml", filteredDonors);
            else //if no parameters were recognized
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");
        }
        
        public List<Donor> findDonors(Dictionary<string, object> parameters)
        {
            List<Donor> filteredDonors = new List<Donor>();

            //the paramsExist variable is used to check if the list of filtered donors must be created or filtered.
            bool paramsExist = false;

            if (parameters.ContainsKey("name") && !String.IsNullOrEmpty(parameters["name"].ToString()))
            {
                string name = parameters["name"].ToString();

                //for now, a name parameter containing a space is presumed to be a first name and last name
                if (name.Contains(" "))
                {
                    string[] names = name.Split(new char[] { ' ' });
                    //searching through the db uses LINQ, which is picky about what variables can be passed.
                    //For instance, LINQ does not accept ArrayIndex variables in queries,
                    //so they are individual string variables in this query instead.
                    string name1 = names[0];
                    string name2 = names[1];
                    filteredDonors.AddRange(db.Donors.Where(x => x.FirstName.Equals(name1, StringComparison.InvariantCultureIgnoreCase) &&
                    x.LastName.Equals(name2, StringComparison.InvariantCultureIgnoreCase)));

                    filteredDonors.AddRange(db.Donors.Where(x => x.FirstName.Equals(name2, StringComparison.InvariantCultureIgnoreCase) &&
                    x.LastName.Equals(name1, StringComparison.InvariantCultureIgnoreCase)));
                }
                //a name without a space is presumed to either be a first or last name
                else
                {
                    filteredDonors.AddRange(db.Donors.Where(x => x.FirstName.Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
                    x.LastName.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
                }
                //confirm that a parameter was used to create a list for later filtering
                paramsExist = true;
            }

            if (parameters.ContainsKey("email-address") && !String.IsNullOrEmpty(parameters["email-address"].ToString()))
            {
                string email = parameters["email-address"].ToString();
                if (filteredDonors.Count == 0 && !paramsExist)//to add new
                    filteredDonors.AddRange(db.Donors.Where(x => x.Email.Equals(email)));
                else if (filteredDonors.Count != 0 && paramsExist)//to filter
                    filteredDonors = filteredDonors.Where(x => x.Email.Equals(email)).ToList();
                paramsExist = true;
            }

            if (parameters.ContainsKey("phone-number") && !String.IsNullOrEmpty(parameters["phone-number"].ToString()))
            {
                string phone = parameters["phone-number"].ToString();
                if (filteredDonors.Count == 0 && !paramsExist)//to add new
                    filteredDonors.AddRange(db.Donors.Where(x => x.PhoneNumber.Equals(phone)));
                else if (filteredDonors.Count != 0 && paramsExist)//to filter
                    filteredDonors = filteredDonors.Where(x => x.PhoneNumber.Equals(phone)).ToList();
                paramsExist = true;
            }
            if (paramsExist)
                return filteredDonors;
            else
                return null;
        }

        public ActionResult ModifyForm(Dictionary<string, object> parameters)
        {
            List<Donor> matchingDonors = findDonors(parameters);
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
            if (ModelState.IsValid)
            {
                db.Entry(donor).State = EntityState.Modified;
                db.SaveChanges();
                return Content("Thanks", "text/html");
            }

            //an invalid submission should just return the form
            return PartialView("~/Views/Donors/_Modify.cshtml", donor);
        }

        public ActionResult ViewAllDonors()
        {
            List<Donor> allDonors = db.Donors.ToList();
            return PartialView("~/Views/Donors/_FetchIndex.cshtml", allDonors);
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

                if(lastSpace < 0 || lastSpace + 1 > name.Length)
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
            return PartialView("~/Views/Donors/_Add.cshtml", newDonor);
        }

        // TODO: Anti-forgery
        public ActionResult Add(Donor donor)
        {
            if (ModelState.IsValid && donor.isValid())
            {
                //confirm with the person submitting the form whether a similar donor already exists

                //fetch a list of similar donors
                List<Donor> sd = db.Donors.Where(x =>
                (x.FirstName == donor.FirstName && x.LastName == donor.LastName) ||
                (x.Email == donor.Email) ||
                (x.PhoneNumber == donor.PhoneNumber)).ToList();

                if(sd.Count() > 0)
                {
                    //return a view showing those similar donors if they exist
                    SimilarDonorModel sdm = new SimilarDonorModel() { newDonor = donor, similarDonors = sd };
                    return PartialView("~/Views/Donors/_Similar.cshtml", sdm);
                }
                else
                { 
                    db.Donors.Add(donor);
                    db.SaveChanges();
                    return PartialView("~/Views/Donors/_AddSuccess.cshtml", donor);
                }
            }

            //an invalid submission should just return the form
            return PartialView("~/Views/Donors/_AddForm.cshtml", donor);
        }

        //this method should only fire after the user has confirmed thy want to add a similar donor
        //(same phone number, email, or first name/ last name combo)
        public ActionResult AddSimilar(SimilarDonorModel sdm)
        {
            db.Donors.Add(sdm.newDonor);
            db.SaveChanges();
            return Content("Thanks", "text/html");
        }

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
    }
}
