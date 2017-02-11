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
using System.IO;
using System.IO.Compression;

namespace DMSLite.Tests.Controllers
{
    [TestClass]
    public class ReceiptControllerTest
    {
        private FakeOrganizationDb db = new FakeOrganizationDb();

        [TestMethod]
        // basic test to see if printing receipts works
        public void TestPrintReceipts()
        {
            //collect donor and batch IDs
            ReceiptController rc = new ReceiptController(db);
            List<Donor> donors = db.Donors.ToList();
            int[] donorIds = new int[donors.Count];
            for (int i = 0; i < donors.Count; i++)
                donorIds[i] = donors[i].Id;

            List<Batch> batches = db.Batches.ToList();
            int[] batchIds = new int[batches.Count];
            for (int i = 0; i < batches.Count; i++)
                batchIds[i] = batches[i].Id;

            //make the receipts
            FileContentResult fcr = (FileContentResult)rc.ZipReceipts(donorIds, batchIds);
            ZipArchive za = new ZipArchive(new MemoryStream(fcr.FileContents), ZipArchiveMode.Read);
            Assert.IsTrue(za.Entries.ToList().Count > 0);
        }

        [TestMethod]
        // test to see if a ReceiptFormModel with no donations produces no receipts
        public void TestPrintNoReceipts()
        {
            //collect donor and batch IDs
            ReceiptController rc = new ReceiptController(db);
            Donor donor = db.Donors.ToList()[1];
            Batch batch = db.Batches.First();
            int[] donorId = { donor.Id };
            int[] batchId = { batch.Id };

            //make the receipts
            FileContentResult fcr = (FileContentResult)rc.ZipReceipts(donorId, batchId);
            ZipArchive za = new ZipArchive(new MemoryStream(fcr.FileContents), ZipArchiveMode.Read);
            Assert.IsTrue(za.Entries.ToList().Count == 0);
        }
    }
}
