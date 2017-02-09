using DMSLite.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DMSLite.Entities;
using DMSLite.DataContexts;
using DMSLite.Models;
using DMSLite.Tests.Mocks;

namespace DMSLite.Tests.Controllers
{
    [TestClass]
    public class DonationAccountControllerTest
    {
        private FakeOrganizationDb db = new FakeOrganizationDb();

        [TestMethod]
        //Tests fetching the list of all account
        public void TestFetchAccounts()
        {
            DonationAccountController dac = new DonationAccountController(db);
            List<Account> dbAccounts = db.Accounts.ToList();
            List<Account> testAccounts = dac.FetchAllAccounts();
            int i = 0;
            foreach (Account a in dbAccounts)
            {
                Assert.AreEqual(a.Id, dbAccounts.ElementAt(i).Id);
                Assert.AreEqual(a.Title, dbAccounts.ElementAt(i).Title);
                i++;
            }
        }

        [TestMethod]
        //Tests fetching a account by the title
        public void TestFetchAccountByTitle()
        {
            //adds a new testing account to the db
            DonationAccountController dac = new DonationAccountController(db);
            Account a = new Account()
            {
                Title = "TestFetchAccount",
            };
            a = (Account)(((PartialViewResult)(dac.Add(a))).Model);
            List<Account> dbAccounts = db.Accounts.Where(x => x.Title == "TestFetchAccount").ToList();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            //searches for that batch by title TestFetchAccount merge
            parameters.Add("title", "TestFetchAccount");
            //parameters.Add("postype", "");
            List<Account> testAccounts = dac.FindAccounts(parameters);
            try
            {
                Assert.AreEqual(dbAccounts.Count, testAccounts.Count);
                Assert.AreEqual(dbAccounts.First().Title, testAccounts.First().Title);
            }
            finally
            {
                //remove testing batch
                dac.Remove(a);
            }
        }

        [TestMethod]
        //Tests that adding a account does add the account to the db and that all data is persisted
        public void TestAddAccount()
        {
            DonationAccountController dac = new DonationAccountController(db);
            Account a = new Account()
            {
                Title = "Roswell",
            };
            a = (Account)(((PartialViewResult)(dac.Add(a))).Model);
            try
            {
                //check db to see if Roswell exists
                List<Account> Roswells = db.Accounts.Where(x => x.Id == a.Id).ToList();
                if (Roswells.Count != 1)
                {
                    Assert.Fail();
                }
                Assert.IsTrue(a.isEqualTo(Roswells.ElementAt<Account>(0)));
            }
            finally
            {
                dac.Remove(a);
            }
        }

        [TestMethod]
        //Tests that adding an invalid account does NOT add the account to the db and that no data is persisted
        public void TestAddInvalidBatch()
        {
            DonationAccountController dac = new DonationAccountController(db);
            Account a = new Account();//invalid as it has no title
            try
            {
                a = (Account)(((PartialViewResult)(dac.Add(a))).Model);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true);
                return;
            }
            //check db to see if Roswell exists
            List<Account> Roswells = db.Accounts.Where(x => x.Id == a.Id).ToList();
            if (Roswells.Count != 1)
            {
                Assert.Fail();
            }
            try
            {
                Assert.IsFalse(a.isEqualTo(Roswells.ElementAt<Account>(0)));
            }
            finally
            {
                dac.Remove(a);
            }
        }
    }
}
