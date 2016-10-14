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
        public void TestViewDonors()
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
        public void TestViewSpecificDonor()
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
        public void TestViewMultipleDonors()
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
        public void TestViewByPhoneNumber()
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
        public void TestViewByEmail()
        {
            HomeController hc = new HomeController();
            FormCollection fc = new FormCollection();
            fc.Add("mainInput", "Show me steve@stevemail.com");
            PartialViewResult returnedView = (PartialViewResult)hc.SendInput(fc);
            var returnedModel = ((IList<Donor>)returnedView.ViewData.Model).ToList();

            List<Donor> steve = db.Donors.Where(x => x.FirstName == "Steve").ToList();
            
            Assert.AreEqual(steve.Count(), returnedModel.Count());
            Assert.AreEqual(steve[0].FirstName, returnedModel[0].FirstName);
        }

        [TestMethod]
        //tests a case where a donor with a duplicate first name is searched for with their unique last name and only one proper donor is returned
        public void TestViewSpecificDonorWithDuplicateName()
        {
            HomeController hc = new HomeController();
            FormCollection fc = new FormCollection();
            fc.Add("mainInput", "Show me Squidward Tentacles");
            PartialViewResult returnedView = (PartialViewResult)hc.SendInput(fc);
            var returnedModel = ((IList<Donor>)returnedView.ViewData.Model).ToList();

            List<Donor> squidward = db.Donors.Where(x => x.FirstName == "Squidward" && x.LastName == "Tentacles").ToList();

            Assert.AreEqual(squidward.Count(), 1);
            Assert.AreEqual(squidward[0].FirstName, returnedModel[0].FirstName);
            Assert.AreEqual(squidward[0].LastName, returnedModel[0].LastName);
        }
    }
}
