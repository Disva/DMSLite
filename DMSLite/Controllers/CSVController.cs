using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using System.IO;
using DMSLite.DataContexts;
using DMSLite.Entities;
using CsvHelper.Configuration;
using DMSLite.CSVMaps;

namespace DMSLite.Controllers
{
    public class CSVController : Controller
    {
        private OrganizationDb db = new OrganizationDb(); 

        // GET: CSV
        public ActionResult Index()
        {
            return View();
        }

        public FileStreamResult ExportAllDonors()
        {
            var result = WriteCsvToMemory(db.Donors);
            var memoryStream = new MemoryStream(result);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "donors.csv" };
        }

        public FileStreamResult ExportAllDonations()
        {
            //var donationMap = new DonationClassMap();
            //var memoryStream = new MemoryStream();
            //var streamWriter = new StreamWriter(memoryStream);
            //var csvWriter = new CsvWriter(streamWriter);

            //csvWriter.Configuration.RegisterClassMap(donationMap);
            //csvWriter.WriteRecords(db.Donations);
            //streamWriter.Flush();

            var donationMap = new DonationClassMap();
            var result = WriteCsvToMemory(db.Donations, donationMap);
            var memoryStream = new MemoryStream(result);

            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "donations.csv" };
        }

        public FileStreamResult ExportAllBatches()
        {
            var result = WriteCsvToMemory(db.Batches);
            var memoryStream = new MemoryStream(result);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "batches.csv" };
        }

        public FileStreamResult ExportAllAccounts()
        {
            var result = WriteCsvToMemory(db.Accounts);
            var memoryStream = new MemoryStream(result);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "accounts.csv" };
        }

        public ActionResult ImportDonors(HttpPostedFileBase donorupload)
        {
            var csv = new CsvReader(new StreamReader(donorupload.InputStream));
            var records = csv.GetRecords<Donor>();
            int i = 0;

            foreach(var record in records)
            {
                i++;
                db.Add(record);
            }

            return new ContentResult { Content = string.Format("Added {0} donors", i) };
        }

        public ActionResult ImportDonations(HttpPostedFileBase donorupload)
        {
            var csv = new CsvReader(new StreamReader(donorupload.InputStream));
            csv.Configuration.RegisterClassMap(new DonationClassMap());
            var records = csv.GetRecords<Donation>();
            int i = 0;

            foreach (var record in records)
            {
                record.DonationDonor = db.Donors.First(x => x.Id == record.DonationDonor_Id);
                record.DonationBatch = db.Batches.First(x => x.Id == record.DonationBatch_Id);
                db.Add(record);
                i++;
            }

            return new ContentResult { Content = string.Format("Added {0} donations", i) };
        }

        public ActionResult ImportBatches(HttpPostedFileBase donorupload)
        {
            var csv = new CsvReader(new StreamReader(donorupload.InputStream));
            var records = csv.GetRecords<Batch>();
            int i = 0;

            foreach (var record in records)
            {
                i++;
                db.Add(record);
            }

            return new ContentResult { Content = string.Format("Added {0} batches", i) };
        }

        public ActionResult ImportAccount(HttpPostedFileBase donorupload)
        {
            var csv = new CsvReader(new StreamReader(donorupload.InputStream));
            var records = csv.GetRecords<Account>();
            int i = 0;

            foreach (var record in records)
            {
                i++;
                db.Add(record);
            }

            return new ContentResult { Content = string.Format("Added {0} accounts", i) };
        }

        public byte[] WriteCsvToMemory<T>(IEnumerable<T> records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        public byte[] WriteCsvToMemory<T>(IEnumerable<T> records, CsvClassMap map)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.Configuration.RegisterClassMap(map);
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}