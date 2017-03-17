using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using DMSLite.Entities;
using System.IO;
using System.IO.Compression;
using DMSLite.DataContexts;
using DMSLite.Controllers;
using PdfSharp.Drawing.Layout;
using DMSLite.Helpers;
using DMSLite.Models;

namespace DMSLite.Controllers
{
    public class ReceiptController : Controller
    {
        private OrganizationDb db = new OrganizationDb();

        public ReceiptController()
        {
            db = new OrganizationDb();
        }

        public ReceiptController(OrganizationDb db)
        {
            this.db = db;
        }

        // GET: Receipt
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReceiptMenu()
        {
            return PartialView("~/Views/Receipt/_Receipt.cshtml");
        }

        public ActionResult ReceiptForm()
        {
            ReceiptFormModel rfm = new ReceiptFormModel();
            rfm.donors = db.Donors.ToList();
            rfm.batches = db.Batches.Where(x => x.CloseDate != null).ToList();
            return PartialView("~/Views/Receipt/_ReceiptForm.cshtml", rfm);
        }
        
        [HttpGet]
        public ActionResult FetchReceiptByDonation(Donation donation)
        {
            // get donor
            Donor donor = db.Donors.Where(x => x.Id.Equals(donation.DonationDonor_Id)).First();
            // get list of donations
            List<Donation> donations = db.Donations.Where(x => x.DonationReceipt_Id.Equals(donation.DonationReceipt_Id)).ToList();

            return File(PrintReceipt(donor, donations).ToArray(), donor.FirstName + "_" + donor.LastName + "_" + donations[0].DonationReceipt_Id + ".pdf", "application/pdf");
        }

        // pass ajax form data nowhere!
        public ActionResult ZipRequest(int[] donors, int[] batches, bool allDonors = false, bool allBatches = false)
        {
            return null;
        }

        [HttpGet]
        public ActionResult ZipReceipts(int[] donors, int[] batches, bool allDonors = false, bool allBatches = false)
        {
            //generate ReceiptFormModel from the form
            ReceiptFormModel rfm = new ReceiptFormModel()
            {
                donors = allDonors ? db.Donors.ToList() : db.Donors.Where(x => donors.Contains(x.Id)).ToList(),
                batches = allBatches ? db.Batches.Where(x => x.CloseDate != null).ToList() : db.Batches.Where(x => batches.Contains(x.Id)).ToList()
            };

            // rebuild the int[] batches list if necessary
            if(allBatches)
            {
                batches = new int[rfm.batches.Count()];
                for (int i = 0; i < rfm.batches.Count(); i++)
                    batches[i] = rfm.batches[i].Id;
            }

            using (MemoryStream outputZip = new MemoryStream())
            {
                using (var archive = new ZipArchive(outputZip, ZipArchiveMode.Create, true))
                {
                    //compile a pdf for each donor
                    foreach (var donor in rfm.donors)
                    {
                        //compile a list of all donations made by that donor for the selected batches
                        List<Donation> donations = db.Donations.Where(x => x.DonationDonor_Id.Equals(donor.Id)              // match donor ID
                                                                        && batches.Any(y => y.Equals(x.DonationBatch_Id))   // match batch ID
                                                                        && x.DonationReceipt_Id.Equals(0)).ToList();        // only add unreceipted donations

                        //only create the PDF if the donor has made donations to the selected batches
                        if(donations.Count() > 0)
                        {
                            // Create receipt object with current date and time, as well as donor's current address
                            Receipt receipt = new Receipt
                            {
                                IssueDate = DateTime.Now,
                                Address = donor.Address
                            };
                            db.Add(receipt);

                            // Add receipt's ID to each donation
                            foreach (var donation in donations)
                            { 
                                donation.DonationReceipt_Id = receipt.Id;
                                db.Modify(donation);
                            }

                            string filename = donor.FirstName + "_" + donor.LastName + "_" + receipt.Id + ".pdf";
                            ZipArchiveEntry file = archive.CreateEntry(filename);
                            using (BinaryWriter bw = new BinaryWriter(file.Open()))
                                bw.Write(PrintReceipt(donor, donations).ToArray());
                        }
                    }
                }
                return File(outputZip.ToArray(), "application/zip", "receipts_" + DateTime.Now.ToString() + ".zip");
            }
        }

        public MemoryStream PrintReceipt(Donor donor, List<Donation> donations)
        {
            // Create output string
            List<string> outputString = new List<string>();
            var d = donations[0];

            Receipt receipt = db.Receipts.Where(x => x.Id.Equals(d.DonationReceipt_Id)).First();
            outputString.Add("Receipt ID: " + receipt.Id);
            outputString.Add("Issued on: " + receipt.IssueDate.ToShortDateString());
            outputString.Add("\n");

            outputString.Add(donor.FirstName + " " + donor.LastName);
            outputString.Add(receipt.Address);
            outputString.Add("\n");

            outputString.Add("Donations:");
            outputString.Add("\n");

            foreach (var donation in donations)
            {
                donation.DonationBatch = db.Batches.Where(x => x.Id.Equals(donation.DonationBatch_Id)).First();
                if (donation.Value > 0)
                {
                    outputString.Add(donation.DonationBatch.CreateDate.ToShortDateString() + " : "
                        + "$" + donation.Value + " to " + donation.DonationBatch.Title);
                    if (!string.IsNullOrWhiteSpace(donation.ObjectDescription))
                        outputString.Add("     Description: " + donation.ObjectDescription);
                }
                else if (donation.ObjectDescription != "")
                    outputString.Add(donation.DonationBatch.CreateDate.ToShortDateString() + " : "
                    + donation.ObjectDescription + " to " + donation.DonationBatch.Title);
                outputString.Add("\n");
            }

            //The resulting PDF will be output to stream
            using (MemoryStream stream = new MemoryStream())
            {
                // Create a new PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Created with PDFsharp";

                // Create a font
                int fontSize = 10;
                XFont font = new XFont("Verdana", fontSize, XFontStyle.Regular);

                //setup margins
                double margin = 2.5;

                //use Helpers/LayoutHelper.cs for multiple page support.
                //Source: http://www.pdfsharp.net/wiki/MultiplePages-sample.ashx
                LayoutHelper helper = new LayoutHelper(document, XUnit.FromCentimeter(margin), XUnit.FromCentimeter(29.7 - margin));
                XUnit left = XUnit.FromCentimeter(margin);

                foreach (var line in outputString)
                {
                    XUnit top = helper.GetLinePosition(fontSize, fontSize);
                    if (line != null)
                        helper.Gfx.DrawString(line, font, XBrushes.Black, left, top, XStringFormats.TopLeft);
                }

                // Save the document...
                document.Save(stream, false);

                //return File(stream.ToArray(), "application/pdf");
                return stream;
            }
        }
    }
}