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


        public ActionResult FetchDonor(Dictionary<string, object> parameters) //Main method to search for donors, parameters may or may not be used
        {
            List<Donor> currentDonors = db.Donors.ToList(); //Takes all donors from database (may not scale well, research)
            if (parameters.Count > 0) //Checks if searching for specific donor or all donors
            {                
                foreach (var parameter in parameters) //Iterates through each paremter (given name, last name, ect) to filter list iteratively
                {
                    //TODO: Search for more efficient way of cleaning this code smell -- pmiri

                    if (parameter.Value.ToString() != "") //Ignores empty parameters
                    { 
                        if(parameter.Key == "given-name")
                        {
                            currentDonors = currentDonors.Where(x => String.Equals(x.FirstName, parameter.Value.ToString(), StringComparison.InvariantCultureIgnoreCase)).ToList(); //Note that this is now case-insensitive, use this in all string comparisons
                        }
                        else if (parameter.Key == "last-name")
                        {
                            currentDonors = currentDonors.Where(x => String.Equals(x.LastName, parameter.Value.ToString(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }
                        else if (parameter.Key == "phone-number")
                        {
                            currentDonors = currentDonors.Where(x => x.PhoneNumber.Replace("-","") == parameter.Value.ToString()).ToList();
                        }
                        else if (parameter.Key == "email-address")
                        {
                            currentDonors = currentDonors.Where(x => x.Email == parameter.Value.ToString()).ToList();
                        }
                    }
                }
                if (currentDonors.Count == 0)
                {
                    return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no donors were found");
                }
            }
            return PartialView("~/Views/Donors/_FetchIndex.cshtml", currentDonors);       
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

        public ActionResult Add(Dictionary<string, object> parameters)
        {
            int id;
            String firstName, lastName, email, phoneNumber, type, receiptFrequency;
            foreach (var parameter in parameters) //Iterates through each paremter (given name, last name, ect) to filter list iteratively
            {
                //TODO: Search for more efficient way of cleaning this code smell -- pmiri

                if (parameter.Value.ToString() != "") //Ignores empty parameters
                {
                    if (parameter.Key == "id")
                    {
                        id= (int)parameter.Value;
                    }
                    else if (parameter.Key == "given-name")
                    {
                        firstName = parameter.Value.ToString();
                    }
                    else if (parameter.Key == "last-name")
                    {
                        lastName = parameter.Value.ToString();
                    }
                    else if (parameter.Key == "phone-number")
                    {
                        phoneNumber = parameter.Value.ToString();
                    }
                    else if (parameter.Key == "email-address")
                    {
                        email = parameter.Value.ToString();
                    }
                    else if (parameter.Key == "type")
                    {
                        type = parameter.Value.ToString();
                    }
                    else if (parameter.Key == "receipt-frequency")
                    {
                        receiptFrequency = parameter.Value.ToString();
                    }
                }
            }
            return View();
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
