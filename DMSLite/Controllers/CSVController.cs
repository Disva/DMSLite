using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using System.IO;
using DMSLite.DataContexts;
using DMSLite.Entities;

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
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "export.csv" };
        }

        public ActionResult ImportDonors(FormCollection form, HttpPostedFileBase donorupload)
        {
            var csv = new CsvReader(new StreamReader(donorupload.InputStream));
            var records = csv.GetRecords<Donor>();
            db.Add(records.First());
            return null;
        }

        public FileStreamResult ExportAllDonations()
        {
            var result = WriteCsvToMemory(db.Donations);
            var memoryStream = new MemoryStream(result);
            return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = "export.csv" };
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
    }
}