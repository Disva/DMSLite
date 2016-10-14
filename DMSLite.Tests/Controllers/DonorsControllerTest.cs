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

namespace DMSLite.Tests.Controllers
{
    [TestClass]
    public class DonorsControllerTest
    {
        private OrganizationDb db = new OrganizationDb();

        [TestMethod]
        public void ViewDonors()
        {
            HomeController hc = new HomeController();
            FormCollection fc = new FormCollection();
            fc.Add("mainInput", "Show donors");
            PartialViewResult returnedView = (PartialViewResult)hc.SendInput(fc);
            var returnedModel = ((IList<Donor>)returnedView.ViewData.Model).ToList();

            List<Donor> allDonors = db.Donors.ToList();

            Assert.AreEqual(allDonors.Count(), returnedModel.Count());
            Assert.AreEqual(allDonors[0].FirstName, returnedModel[0].FirstName);
        }

        [TestMethod]
        public void ViewSpecificDonor()
        {
            HomeController hc = new HomeController();
            FormCollection fc = new FormCollection();
            fc.Add("mainInput", "Show me Steve");
            PartialViewResult returnedView = (PartialViewResult)hc.SendInput(fc);
            var returnedModel = ((IList<Donor>)returnedView.ViewData.Model).ToList();

            List<Donor> steve = db.Donors.Where(x => x.FirstName == "Steve").ToList();
            
            Assert.AreEqual(steve.Count(), returnedModel.Count());
            Assert.AreEqual(steve[0].FirstName, returnedModel[0].FirstName);
        }

        [TestMethod]
        public void ViewMultipleDonors()
        {
            HomeController hc = new HomeController();
            FormCollection fc = new FormCollection();
            fc.Add("mainInput", "Show me Squidward");
            PartialViewResult returnedView = (PartialViewResult)hc.SendInput(fc);
            var returnedModel = ((IList<Donor>)returnedView.ViewData.Model).ToList();

            List<Donor> squids = db.Donors.Where(x => x.FirstName == "Squidward").ToList();

            Assert.AreEqual(squids.Count(), returnedModel.Count());
            Assert.AreEqual(squids[0].FirstName, returnedModel[0].FirstName);
        }

        [TestMethod]
        public void ViewByPhoneNumber()
        {
            HomeController hc = new HomeController();
            FormCollection fc = new FormCollection();
            fc.Add("mainInput", "Show me 555-555-5555");
            PartialViewResult returnedView = (PartialViewResult)hc.SendInput(fc);
            var returnedModel = ((IList<Donor>)returnedView.ViewData.Model).ToList();

            List<Donor> phone = db.Donors.Where(x => x.PhoneNumber == "555-555-5555").ToList();

            Assert.AreEqual(phone.Count(), returnedModel.Count());
            Assert.AreEqual(phone[0].PhoneNumber, returnedModel[0].PhoneNumber);
        }

        [TestMethod]
        public void ViewByEmail()
        {
            HomeController hc = new HomeController();
            FormCollection fc = new FormCollection();
            fc.Add("mainInput", "Show me steve@stevemail.com");
            PartialViewResult returnedView = (PartialViewResult)hc.SendInput(fc);
            var returnedModel = ((IList<Donor>)returnedView.ViewData.Model).ToList();

            List<Donor> email = db.Donors.Where(x => x.Email == "steve@stevemail.com").ToList();

            Assert.AreEqual(email.Count(), returnedModel.Count());
            Assert.AreEqual(email[0].Email, returnedModel[0].Email);
        }
    }
}
