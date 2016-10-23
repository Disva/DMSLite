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

namespace DMSLite
{
    public class DonorsController : Controller
    {
        private OrganizationDb db = new OrganizationDb();

        public ActionResult FetchDonor(Dictionary<string, object> parameters) //Main method to search for donors, parameters may or may not be used
        {
            List<Donor> filteredDonors = new List<Donor>();
            List<Donor> allDonors = new List<Donor>();
            allDonors = db.Donors.ToList<Donor>();//Not an ideal solution, unless we find a way to make dv.Donors to only return the donors visible by the organization of the current user - dk
            bool paramsExist = false;
            int size = parameters.Count;
            if (parameters.Count > 0) //Checks if searching for specific donor or all donors
            {
                if (!String.IsNullOrEmpty(parameters["name"].ToString()))
                {
                    string name = parameters["name"].ToString();
                    if(name.Contains(" "))//split up by first and last
                    {
                        string[] names = name.Split(new char[] { ' ' });
                        filteredDonors.AddRange(allDonors.Where(x=> String.Equals(names[0], x.FirstName, StringComparison.InvariantCultureIgnoreCase) &&
                        String.Equals(names[1], x.LastName, StringComparison.InvariantCultureIgnoreCase)));
                        filteredDonors.AddRange(allDonors.Where(x => String.Equals(names[1], x.FirstName, StringComparison.InvariantCultureIgnoreCase) &&
                        String.Equals(names[0], x.LastName, StringComparison.InvariantCultureIgnoreCase)));
                    }
                    else
                    {
                        filteredDonors.AddRange(allDonors.Where(x => String.Equals(x.FirstName, name, StringComparison.InvariantCultureIgnoreCase) ||
                        String.Equals(x.LastName, name, StringComparison.InvariantCultureIgnoreCase)));
                    }
                    paramsExist = true;
                }
                if (!String.IsNullOrEmpty(parameters["email-address"].ToString()))
                {
                    if (filteredDonors.Count == 0 && !paramsExist)//to add new
                        filteredDonors.AddRange(allDonors.Where(x => x.Email == parameters["email-address"].ToString()));
                    else if(filteredDonors.Count == 0 && paramsExist)//to filter
                        filteredDonors = filteredDonors.Where(x => x.Email == parameters["email-address"].ToString()).ToList();
                    paramsExist = true;
                }
                if (!String.IsNullOrEmpty(parameters["phone-number"].ToString()))
                {
                    if (filteredDonors.Count == 0 && !paramsExist)//to add new
                        filteredDonors.AddRange(allDonors.Where(x => x.PhoneNumber.Replace("-", "") == parameters["phone-number"].ToString().Replace("-", "")));
                    else if (filteredDonors.Count == 0 && paramsExist)//to filter
                        filteredDonors = filteredDonors.Where(x => x.PhoneNumber.Replace("-", "") == parameters["phone-number"].ToString().Replace("-", "")).ToList();
                    paramsExist = true;
                }
            }
            if (filteredDonors.Count == 0 && paramsExist)
            {
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no donors were found");
            }
            if (paramsExist) //If at least one parameter's value was non-empty
                return PartialView("~/Views/Donors/_FetchIndex.cshtml", filteredDonors);
            else //if no parameters were recognized
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");
        }

        public ActionResult viewAllDonors()
        {
            List<Donor> allDonors = db.Donors.ToList();
            return PartialView("~/Views/Donors/_FetchIndex.cshtml", allDonors);
        }

        public ActionResult AddForm(Dictionary<string, object> parameters)
        {
            Donor newDonor = new Donor();
            if (parameters.ContainsKey("given-name"))
                newDonor.FirstName = parameters["given-name"].ToString();
            if (parameters.ContainsKey("last-name"))
                newDonor.LastName = parameters["last-name"].ToString();
            if (parameters.ContainsKey("phone-number"))
                newDonor.PhoneNumber = parameters["phone-number"].ToString();
            if (parameters.ContainsKey("email"))
                newDonor.Email = parameters["email"].ToString();
            return PartialView("~/Views/Donors/_Add.cshtml", newDonor);
        }

        public ActionResult Add(Donor donor)
        {
            if (ModelState.IsValid)
            {
                //check if db already contains this identical donor
                try
                {
                    foreach (Donor d in db.Donors)//TEMPORARY, will need to eventually have db.Donors only return those donors visible to the current user's organization
                    {
                        if (d.isEqualTo(donor))
                            throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    //TODO show the user an error message explaining that this user is a duplicate
                    return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "This donor is a duplicate! It has not been added.");
                }
                db.Donors.Add(donor);
                db.SaveChanges();
                return Content("Thanks","text/html");
            }
            return PartialView("~/Views/Donors/_Add.cshtml", donor);
            //TODO: make sure the name field is recognized as valid by api.ai
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
