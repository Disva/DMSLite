using DMSLite.DataContexts;
using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMSLite.Controllers
{
    public class DonationController : Controller
    {
        private OrganizationDb db = new OrganizationDb();

        #region Fetch
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
           /* if (parameters.ContainsKey("title"))
                newDonation.Title = parameters["title"].ToString();*/
            return PartialView("~/Views/Donation/_AddForm.cshtml", newDonation);
        }

        // TODO: Anti-forgery
        public ActionResult Add(Donation donation)
        {
            //TODO: add to the db
            /*
            batch.CreateDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Add(batch);
                return PartialView("~/Views/Donation/_AddSuccess.cshtml", batch);
            }
            */
            //an invalid submission shall return the form with some validation error messages.
            return PartialView("~/Views/Donation/_AddForm.cshtml", donation);
        }
        #endregion

        #region MadeByMs
        // GET: Batch
        public ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}