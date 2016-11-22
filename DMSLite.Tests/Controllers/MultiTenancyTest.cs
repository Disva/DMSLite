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
        FakeOrganizationDb db = new FakeOrganizationDb(0);

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
            db.TenantId = 0;

            DonorsController dc = new DonorsController(db);
            Donor d = new Donor
            {
                FirstName = "fName_TestAddMultiTNewValidDonor",
                LastName = "lName_TestAddMultiTNewValidDonor",
                Email = "test.email@email.com",
                PhoneNumber = "111-111-1111",
            };
            var arReturned = dc.Add(d);

            var foundDonors = findDonors(db, d);

            Assert.IsTrue(foundDonors.Count() == 1, "Donor could not be added to the database for testing");

            db.TenantId = 1;

            foundDonors = findDonors(db, d);

            Assert.IsTrue(foundDonors.Count() == 0, "Donor was still in the database even though we switched tenants");

            db.TenantId = 0;

            dc.Remove(d);

            foundDonors = findDonors(db, d);

            Assert.IsTrue(foundDonors.Count() == 0, "Donor was still in the database even though we tried to delete it");
        }

        [TestMethod]
        public void TestMultiTenantEditDonor()
        {
            db.TenantId = 0;

            DonorsController dc = new DonorsController(db);
            Donor d = new Donor
            {
                FirstName = "fName_TestModifyMultiTNewValidDonor",
                LastName = "lName_TestModifyMultiTNewValidDonor",
                Email = "test.email@email.com",
                PhoneNumber = "111-111-1111",
            };
            var arReturned = dc.Add(d);

            var foundDonors = findDonors(db, d);

            Assert.IsTrue(foundDonors.Count() == 1, "Donor could not be added to the database for testing");

            Donor foundEditedDonor = foundDonors.First();

            foundEditedDonor.Email = "test.test@email.ca";

            dc.Modify(foundEditedDonor);

            foundDonors = findDonors(db, d);

            Assert.IsTrue(foundDonors.First().Email == foundEditedDonor.Email, "The Donor was not successfully edited");

            db.TenantId = 1;

            foundDonors = findDonors(db, d);

            int foundCount = foundDonors.Count();
            Assert.IsTrue(foundCount == 0, "Donor was still in the database even though we switched tenants. Found " + foundCount.ToString() + " donors");

            db.TenantId = 0;

            dc.Remove(foundEditedDonor);
        }
    }
}
