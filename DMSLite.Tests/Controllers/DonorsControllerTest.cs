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
            fc.Add("mainInput", "Show me Denis");
            PartialViewResult returnedView = (PartialViewResult)hc.SendInput(fc);
            var returnedModel = ((IList<Donor>)returnedView.ViewData.Model).ToList();

            List<Donor> denis = db.Donors.Where(x => x.FirstName == "Denis").ToList();
            
            Assert.AreEqual(denis.Count(), returnedModel.Count());
            Assert.AreEqual(denis[0].FirstName, returnedModel[0].FirstName);
        }
    }
}
