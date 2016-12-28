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
        //Tests for successful modification of donations
        public void TestModifyDonation()
        {
            DonationController donationController = new DonationController(db);
            DonorsController donorsController = new DonorsController(db);
            BatchController batchController = new BatchController(db);
            //make a new donation from a new donor to a new open batch
            Donor donor = new Donor()
            {
                FirstName = "fName_TestModifyDonation",
                LastName = "lName_TestModifyDonation",
                Email = "email@testmodify.com",
                PhoneNumber = "000-111-9191",
            };
            donorsController.Add(donor);

            //this batch cannot be located during DonationController's Modify method
            Batch batch = new Batch()
            { 
                Title="title_TestModifyDonation"
            };
            batchController.Add(batch);
            //Assumed this was the problem, was wrong
            //batch.TenantOrganizationId = 1;
            //db.Modify(batch);

            Donation donation = new Donation()
            {
                DonationDonor = donor,
                DonationBatch = batch,
                ObjectDescription = "desc_TestModifyDonation",
                Value = 200
            };

            //delete this line after this metod works, and delete stray test data from failed tests:
            //it just bypasses the fact that only one donor with the test data exists and a new one can't be made
            donor = db.Donors.First(x => x.FirstName == donor.FirstName);

            donationController.Add(donation, donor.Id, batch.Id);

            //modify that donation
            donation = db.Donations.First(x => x.ObjectDescription.Equals(donation.ObjectDescription));
            donation.ObjectDescription = "desc2_TestModifyDonation";

            donationController.Modify(donation, donor.Id, donation.Id);

            //check for success in db
            donation = db.Donations.First(x => x.ObjectDescription.Equals(donation.ObjectDescription));
            Assert.Equals(donation.ObjectDescription, "desc2_TestModifyDonation");

            //ITERATION 6: close the batch
            //ITERATION 6: modifying a closed batch is not possible

            //delete all temporary objects
            donationController.Remove(donation);
            donorsController.Remove(donor);
            batchController.Remove(batch);
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
