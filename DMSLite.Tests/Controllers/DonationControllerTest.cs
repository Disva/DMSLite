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
    public class DonationControllerTest
    {
        private FakeOrganizationDb db = new FakeOrganizationDb();

        [TestMethod]
        //Tests that ...
        public void TestFetchDonation()
        {
            //Not implemented yet
        }

        [TestMethod]
        //Tests that ...
        public void TestModifyDonation()
        {
            //Not implemented yet
        }

        [TestMethod]
        //Tests that adding a donation does add the donation to the db and that all data is persisted
        public void TestAddDonation()
        {
            //Add(Donation donation, int donationDonor, int donationBatch)

            DonationController dc = new DonationController(db);
            Donation d = new Donation() {
                Value = 123,
                ObjectDescription = "wow",
                DonationDonor = db.Donors.First<Donor>(),
                DonationBatch = db.Batches.First<Batch>(),
            };
            d = (Donation)(((PartialViewResult)(dc.Add(d, d.DonationDonor.Id, d.DonationBatch.Id))).Model);
            //check db to see if Roswell exists
            List<Donation> Donations = db.Donations.Where(x => x.Id == d.Id).ToList();
            if (Donations.Count != 1)
            {
                Assert.Fail();
            }
            Assert.IsTrue(d.isEqualTo(Donations.ElementAt<Donation>(0)));
            dc.Remove(d);
        }

        [TestMethod]
        //Tests that adding an invalid donation does NOT add the donation to the db and that no data is persisted
        public void TestAddInvalidDonation()
        {
            //Add(Donation donation, int donationDonor, int donationBatch)

            DonationController dc = new DonationController(db);
            Donation d = new Donation()
            {
            };
            try
            {
                d = (Donation)(((PartialViewResult)(dc.Add(d, 0, 0))).Model);
            }
            catch(Exception e)
            {
                Assert.IsTrue(true);
                return;
            }
            //check db to see if Roswell exists
            List<Donation> Donations = db.Donations.Where(x => x.Id == d.Id).ToList();
            if (Donations.Count != 1)
            {
                Assert.Fail();
            }
            Assert.IsFalse(d.isEqualTo(Donations.ElementAt<Donation>(0)));
            dc.Remove(d);
        }

    }
}
