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
        //Tests fetching the list of all batches
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

        [TestMethod]
        //Tests fetching the list of all open batches
        public void TestFetchOpenBatches()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("title", "");
            parameters.Add("type", "open");

            BatchController bc = new BatchController(db);
            PartialViewResult pvr = (PartialViewResult)bc.FetchBatches(parameters);
            if (pvr.ViewName == "~/Views/Shared/_ErrorMessage.cshtml")
            {
                //this is the case where there are no batches of that type
                Assert.IsTrue(true);
                return;
            }
            List<Batch> fetchedOpenBatches = ((List<Batch>)pvr.ViewData.Model).ToList();
            List<Batch> dbOpenBatches = db.Batches.Where(x => x.CloseDate == null).ToList();
            int i = 0;
            if (dbOpenBatches.Count() == 0 && fetchedOpenBatches.Count() == 0)
            {
                Assert.IsTrue(true);
            }
            else if (dbOpenBatches.Count() == fetchedOpenBatches.Count()) {
                foreach (Batch b in dbOpenBatches)
                {
                    Assert.AreEqual(b.Id, fetchedOpenBatches.ElementAt(i).Id);
                    Assert.IsNull(fetchedOpenBatches.ElementAt(i).CloseDate);
                    i++;
                }
            }
        }

        [TestMethod]
        //Tests fetching the list of all open batches
        public void TestFetchClosedBatches()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("title", "");
            parameters.Add("type", "closed");

            BatchController bc = new BatchController(db);
            PartialViewResult pvr = (PartialViewResult)bc.FetchBatches(parameters);
            if (pvr.ViewName == "~/Views/Shared/_ErrorMessage.cshtml")
            {
                //this is the case where there are no batches of that type
                Assert.IsTrue(true);
                return;
            }
            List<Batch> fetchedClosedBatches = ((List<Batch>)pvr.ViewData.Model).ToList();
            List<Batch> dbClosedBatches = db.Batches.Where(x => x.CloseDate != null).ToList();
            int i = 0;
            if (dbClosedBatches.Count() == 0 && fetchedClosedBatches.Count() == 0)
            {
                Assert.IsTrue(true);
            }
            else if (dbClosedBatches.Count() == fetchedClosedBatches.Count())
            {
                foreach (Batch b in dbClosedBatches)
                {
                    Assert.AreEqual(b.Id, fetchedClosedBatches.ElementAt(i).Id);
                    Assert.AreEqual(b.CloseDate, fetchedClosedBatches.ElementAt(i).CloseDate);
                    i++;
                }
            }
        }

        [TestMethod]
        //Tests fetching a batch by the title
        public void TestFetchBatchByTitle()
        {
            //adds a new testing batch to the db
            BatchController bc = new BatchController(db);
            Batch b = new Batch()
            {
                Title = "TestFetchBatch",
            };
            b = (Batch)(((PartialViewResult)(bc.Add(b))).Model);
            List<Batch> dbBatches = db.Batches.Where(x => x.Title == "TestFetchBatch").ToList();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            //searches for that batch by title TestFetchBatch merge
            parameters.Add("title", "TestFetchBatch");
            parameters.Add("type", "");
            List<Batch> testBatches = bc.FindBatches(parameters);
            Assert.AreEqual(dbBatches.Count, testBatches.Count);
            Assert.AreEqual(dbBatches.First().Title, testBatches.First().Title);
            //remove testing batch
            bc.Remove(b);
        }

        [TestMethod]
        //Tests that viewing a fetced batch either returns a view of the batches donations or an error message if it has none
        public void TestViewFetchedBatch()
        {
            BatchController bc = new BatchController(db);
            DonationController dc = new DonationController(db);
            Batch testBatch = new Batch()
            {
                Title = "TestBatch",
            };
            testBatch = (Batch)(((PartialViewResult)(bc.Add(testBatch))).Model);
            PartialViewResult pvrReturned = (PartialViewResult)dc.FetchByBatchId(testBatch);
            //make sure empty batch returns an error message
            if ((pvrReturned.GetType().ToString().Equals("System.Web.Mvc.PartialViewResult"))
                && (((PartialViewResult)pvrReturned).ViewName.Equals("~/Views/Shared/_ErrorMessage.cshtml"))
                )
            {
                Assert.IsTrue(true);
            }
            //add donations to batch and ensure that it returns the batch view
            Donation testDonation = new Donation()
            {
                Value = 5,
                ObjectDescription = "a test donation",
                DonationBatch = testBatch,
                DonationBatch_Id = testBatch.Id,
            };
            dc.Add(testDonation, db.Donors.First<Donor>().Id, testBatch.Id);
            pvrReturned = (PartialViewResult)dc.FetchByBatchId(testBatch);
            if ((pvrReturned.GetType().ToString().Equals("System.Web.Mvc.PartialViewResult"))
                && (((PartialViewResult)pvrReturned).ViewName.Equals("~/Views/Shared/_FetchIndex.cshtml"))
                )
            {
                Assert.IsTrue(true);
            }
            dc.Remove(testDonation);
            bc.Remove(testBatch);
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
            Batch b = new Batch();//invalid as it has no title
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
