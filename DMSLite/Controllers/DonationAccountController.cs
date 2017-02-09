using DMSLite.DataContexts;
using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DMSLite.Controllers
{
    using DateRange = Tuple<DateTime, DateTime>;

    [Authorize]
    public class DonationAccountController : Controller
    {
        private OrganizationDb db;

        public DonationAccountController()
        {
            db = new OrganizationDb();
        }

        public DonationAccountController(OrganizationDb db)
        {
            this.db = db;
        }

        #region Fetch
        public ActionResult FetchAccounts(Dictionary<string, object> parameters)
        {
            List<Account> filteredAccounts = FindAccounts(parameters);
            if (filteredAccounts == null)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no parameters were recognized");
            if (filteredAccounts.Count == 0)
                return PartialView("~/Views/Shared/_ErrorMessage.cshtml", "no accounts were found");

            return PartialView("~/Views/DonationAccount/_FetchIndex.cshtml", filteredAccounts);
        }

        public List<Account> FindAccounts(Dictionary<string, object> parameters)
        {
            List<Account> filteredAccounts = new List<Account>();

            bool paramsExist = !String.IsNullOrEmpty(parameters["title"].ToString());

            if (!paramsExist)
                return FetchAllAccounts();

            if (!String.IsNullOrEmpty(parameters["title"].ToString()))
                FetchByTitle(ref filteredAccounts, parameters["title"].ToString());

            return filteredAccounts;
        }

        private void FetchByTitle(ref List<Account> list, string Title)
        {
            //searching through the db uses LINQ, which is picky about what variables can be passed.
            //For instance, LINQ does not accept ArrayIndex variables in queries,
            //so they are individual string variables in this query instead.
            if (list.Count == 0)
            {
                //look for batches that contain the specified title (case insensitive)
                list.AddRange(db.Accounts.Where(x => x.Title.ToUpper().Contains(Title.ToUpper())));
            }
            else
            {
                list = list.Where(x => x.Title.ToUpper().Contains(Title.ToUpper())).ToList();
            }
        }

        public List<Account> FindByTitle(string title)
        {
            List<Account> matchingAccounts = FetchAllAccounts();
            FetchByTitle(ref matchingAccounts, title);
            return matchingAccounts;
        }

    public List<Account> FetchAllAccounts()
        {
            List<Account> allAccounts = db.Accounts.ToList();
            return allAccounts;
        }

        #endregion

        #region Modify
        #endregion

        #region Add
        public ActionResult AddMenu(Dictionary<string, object> parameters)
        {
            return PartialView("~/Views/DonationAccount/_Add.cshtml", parameters);
        }

        public ActionResult AddForm(Dictionary<string, object> parameters)
        {
            Account newAccount = new Account();
            if (parameters.ContainsKey("title"))
                newAccount.Title = parameters["title"].ToString();
            return PartialView("~/Views/DonationAccount/_AddForm.cshtml", newAccount);
        }

        // TODO: Anti-forgery
        public ActionResult Add(Account account)
        {
            List<Account> similarAccounts = db.Accounts.Where(x => x.Title == account.Title).ToList();
            if (similarAccounts.Count() > 0)
            {
                //return an error
                ModelState.AddModelError("Title", "An existing account has this title.");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Add(account);
                    return PartialView("~/Views/DonationAccount/_AddSuccess.cshtml", account);
                }
            }
            //an invalid submission shall return the form with some validation error messages.
            return PartialView("~/Views/DonationAccount/_AddForm.cshtml", account);
        }

        // Action to search for donors by name and obtain a json result
        public ActionResult SearchAccounts(string searchKey)
        {
            if (string.IsNullOrEmpty(searchKey))
            {
                return new JsonResult { Data = new { results = new List<Account>() }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            var accounts = db.Accounts.Where(x => x.Title.ToLower().StartsWith(searchKey.ToLower()));
            return new JsonResult { Data = new { results = accounts.Select(x => new { title = x.Title, id = x.Id }) }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
        public ActionResult Remove(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Remove(account);
                db.SaveChanges();
                return Content("Removed", "text/html");
            }
            return PartialView("~/Views/DonationAccount/_Add.cshtml", account);
            //TODO: make sure the name field is recognized as valid by api.ai
        }
        // GET: Batch
        public ActionResult Index()
        {
            return View();
        }
    }
}