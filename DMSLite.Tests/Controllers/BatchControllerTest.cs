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
        public void TestFetchBatches()
        {
            BatchController bc = new BatchController(db);
            List<Batch> dbBatches = db.Batches.ToList();
            List<Batch> testBatches = bc.FetchAllBatches();
            int i = 0;
            foreach(Batch b in dbBatches)
            {
                Assert.AreEqual(b.Id, testBatches.ElementAt(i).Id);
                Assert.AreEqual(b.Title, testBatches.ElementAt(i).Title);
                Assert.AreEqual(b.CreateDate, testBatches.ElementAt(i).CreateDate);
                i++;
            }
        }

        public void TestFetchBatchByTitle()
        {
            BatchController bc = new BatchController(db);
            List<Batch> dbBatches = db.Batches.Where(x => x.Title == "Batch for the new tenant").ToList();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("title", "Batch for the new tenant");
            List<Batch> testBatches = bc.FindBatches(parameters);
            Assert.AreEqual(dbBatches.Count, testBatches.Count);
            Assert.AreEqual(dbBatches.First().Title, testBatches.First().Title);
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
