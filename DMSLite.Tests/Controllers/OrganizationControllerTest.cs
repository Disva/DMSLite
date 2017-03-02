using DMSLite.Controllers;
using DMSLite.Entities;
using DMSLite.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Tests.Controllers
{
    // Tests to verify crud functionality of organizations
    [TestClass]
    public class OrganizationControllerTest
    {
        private FakeOrganizationDb db = new FakeOrganizationDb();

        [TestMethod]
        public void TestNewOrganization()
        {
            OrganizationsController oc = new OrganizationsController(db);

            Organization org = new Organization() {
                Name = "TestOrganization1"
            };

            try
            {
                oc.Create(org);

                Assert.AreEqual(1, db.Organizations.Where(x => x.Name == "TestOrganization1").Count());
            }
            finally
            {
                db.Organizations.Remove(org);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void TestEditOrganization()
        {
            OrganizationsController oc = new OrganizationsController(db);

            Organization org = new Organization()
            {
                Name = "TestOrganization2"
            };

            try
            {
                oc.Create(org);

                Assert.AreEqual(1, db.Organizations.Where(x => x.Name == "TestOrganization2").Count());

                org.Name = "TestOrganization2Edit";

                oc.Edit(org);

                Assert.AreEqual(1, db.Organizations.Where(x => x.Name == "TestOrganization2Edit").Count());
            }
            finally
            {
                db.Organizations.Remove(org);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void TestDeleteOrganization()
        {
            OrganizationsController oc = new OrganizationsController(db);

            Organization org = new Organization()
            {
                Name = "TestOrganization3"
            };

            oc.Create(org);

            Assert.AreEqual(1, db.Organizations.Where(x => x.Name == "TestOrganization3").Count());

            oc.DeleteConfirmed(org.Id);

            Assert.AreEqual(0, db.Organizations.Where(x => x.Name == "TestOrganization3").Count());
        }

    }
}
