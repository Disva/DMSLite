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
    public class BatchControllerTest
    {
        private FakeOrganizationDb db = new FakeOrganizationDb();

        [TestMethod]
        //Tests that ...
        public void TestFetchBatch()
        {
            //Not implemented yet
        }

        [TestMethod]
        //Tests that ...
        public void TestModifyBatch()
        {
            //Not implemented yet
        }

        [TestMethod]
        //Tests that adding a batch does add the batch to the db and that all data is persisted
        public void TestAddBatch()
        {
            BatchController bc = new BatchController(db);
            Batch b = new Batch()
            {
                Title = "Roswell",
            };
            b = (Batch)(((PartialViewResult)(bc.Add(b))).Model);
            //check db to see if Roswell exists
            List<Batch> Roswells = db.Batches.Where(x => x.Id == b.Id).ToList();
            if(Roswells.Count != 1)
            {
                Assert.Fail();
            }
            Assert.IsTrue(b.isEqualTo(Roswells.ElementAt<Batch>(0)));
            bc.Remove(b);
        }

        [TestMethod]
        //Tests that adding an invalid batch does NOT add the batch to the db and that no data is persisted
        public void TestAddInvalidBatch()
        {
            BatchController bc = new BatchController(db);
            Batch b = new Batch();//invalid as it has not title
            try
            {
                b = (Batch)(((PartialViewResult)(bc.Add(b))).Model);
            }
            catch(Exception e)
            {
                Assert.IsTrue(true);
                return;
            }
            //check db to see if Roswell exists
            List<Batch> Roswells = db.Batches.Where(x => x.Id == b.Id).ToList();
            if (Roswells.Count != 1)
            {
                Assert.Fail();
            }
            Assert.IsFalse(b.isEqualTo(Roswells.ElementAt<Batch>(0)));
            bc.Remove(b);
        }

    }
}
