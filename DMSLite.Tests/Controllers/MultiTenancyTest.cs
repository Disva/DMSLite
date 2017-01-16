using DMSLite.DataContexts;
using DMSLite.Entities;
using DMSLite.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DMSLite.Tests.Controllers
{
    [TestClass]
    public class MultiTenancyTest
    {
        private IEnumerable<Donor> findDonors(OrganizationDb dbp, Donor d)
        {
            return dbp.Donors.Where(x => x.FirstName == d.FirstName && x.LastName == d.LastName).ToList();
        }
        /*
            Test to see if an added donor can be viewed
            by a user from a different organization
        */
        [TestMethod]
        public void TestMultiTenantAddDonor()
        {
            FakeOrganizationDb db = new FakeOrganizationDb();

            db.SetTenantId(0);

            DonorsController dc = new DonorsController(db);
            Donor d = new Donor
            {
                FirstName = "fName_TestAddMultiTNewValidDonor",
                LastName = "lName_TestAddMultiTNewValidDonor"
            };
            var arReturned = dc.Add(d);

            var foundDonors = findDonors(db, d);

            // Ensure that the donor was in fact added
            Assert.IsTrue(foundDonors.Count() == 1, "Donor could not be added to the database for testing");

            // Switch to another tenant
            db.SetTenantId(1);

            foundDonors = findDonors(db, d);

            // Check if we can now access the donor
            Assert.IsTrue(foundDonors.Count() == 0, "Donor was still in the database even though we switched tenants");

            db.SetTenantId(0);

            // Clean up
            dc.Remove(d);

            foundDonors = findDonors(db, d);

            Assert.IsTrue(foundDonors.Count() == 0, "Donor was still in the database even though we tried to delete it");
        }

        // Check if a donor retains its tenant after modification
        [TestMethod]
        public void TestMultiTenantEditDonor()
        {
            FakeOrganizationDb db = new FakeOrganizationDb();

            db.SetTenantId(2);

            DonorsController dc = new DonorsController(db);
            Donor d = new Donor
            {
                FirstName = "fName_TestModifyMultiTNewValidDonor",
                LastName = "lName_TestModifyMultiTNewValidDonor"
            };
            var arReturned = dc.Add(d);

            var foundDonors = findDonors(db, d);

            // Check if the donor was added to the database
            Assert.IsTrue(foundDonors.Count() == 1, "Donor could not be added to the database for testing");

            Donor foundEditedDonor = foundDonors.First();

            foundEditedDonor.Email = "test.test@email.ca";

            // Make a change to the donor
            dc.Modify(foundEditedDonor);

            foundDonors = findDonors(db, d);

            // Ensure that the donor was modified
            Assert.IsTrue(foundDonors.First().Email == foundEditedDonor.Email, "The Donor was not successfully edited");

            db.SetTenantId(1);

            foundDonors = findDonors(db, foundEditedDonor);

            // Check to see if we can find the donor from the other tenant
            int foundCount = foundDonors.Count();
            Assert.IsTrue(foundCount == 0, "Donor was still in the database even though we switched tenants. Found " + foundCount.ToString() + " donors");

            db.SetTenantId(2);

            // Clean up
            dc.Remove(foundEditedDonor);

            foundDonors = findDonors(db, foundEditedDonor);

            Assert.IsTrue(foundDonors.Count() == 0, "Donor was still in the database even though we tried to delete it");
        }
    }
}
