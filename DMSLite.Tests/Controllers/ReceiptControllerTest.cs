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
            Donor donor = new Donor
            {
                FirstName = "X",
                LastName = "Y",
                Address = "Z"
            };
            db.Add(donor);
            Batch batch = new Batch
            {
                CreateDate = DateTime.Now,
                Title = "AAA"
            };
            db.Add(batch);
            Donation donation = new Donation
            {
                DonationBatch_Id = batch.Id,
                DonationDonor_Id = donor.Id,
            };
            db.Add(donation);
            batch.CloseDate = DateTime.Now;
            db.Modify(batch);

            ReceiptController rc = new ReceiptController(db);
            int[] da = { donor.Id };
            int[] ba = { batch.Id };
            rc.ZipReceipts(da, ba);

            //make the receipt
            FileContentResult fcr = (FileContentResult)rc.ZipReceipts(da, ba);
            ZipArchive za = new ZipArchive(new MemoryStream(fcr.FileContents), ZipArchiveMode.Read);
            Assert.IsTrue(za.Entries.ToList().Count == 1);

            db.Donors.Remove(donor);
            db.Batches.Remove(batch);
            db.Donations.Remove(donation);
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

        [TestMethod]
        // test to see if a receipt printed for a donor in the past stores a different address than that donor's current address
        public void TestPrintReceiptNewAddress()
        {
            Donor donor = new Donor
            {
                FirstName = "X",
                LastName = "Y",
                Address = "Z"
            };
            db.Add(donor);
            Batch batch = new Batch
            {
                CreateDate = DateTime.Now,
                Title = "AAA"
            };
            db.Add(batch);
            Donation donation = new Donation
            {
                DonationBatch_Id = batch.Id,
                DonationDonor_Id = donor.Id,
            };
            db.Add(donation);
            batch.CloseDate = DateTime.Now;
            db.Modify(batch);

            ReceiptController rc = new ReceiptController(db);
            int[] da = { donor.Id };
            int[] ba = { batch.Id };
            rc.ZipReceipts(da, ba);

            donor.Address = "W";
            db.Modify(donor);

            rc.FetchReceiptByDonation(donation);
            Receipt receipt = db.Receipts.Single(x => x.Id == donation.DonationReceipt_Id);

            Assert.IsFalse(receipt.Address == donor.Address);

            db.Donors.Remove(donor);
            db.Batches.Remove(batch);
            db.Donations.Remove(donation);
            db.Receipts.Remove(receipt);

        }
    }
}
