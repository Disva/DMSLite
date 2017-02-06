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
using DMSLite.DataContexts;
using DMSLite.Controllers;
using PdfSharp.Drawing.Layout;

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

        public ActionResult Test()
        {
            // we are using this MemoryStream to be able to access the PDF as an ActionResult.
            // source: https://stackoverflow.com/questions/15121876/how-do-i-display-a-pdf-using-pdfsharp-in-asp-net-mvc#15145131
            using (MemoryStream stream = new MemoryStream())
            { 
                // Create a new PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Created with PDFsharp";
            
                // Create an empty page
                PdfPage page = document.AddPage();

                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Create a font
                XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

                // Draw the text
                gfx.DrawString("Hello, World!", font, XBrushes.Black,
                    new XRect(0, 0, page.Width, page.Height),
                    XStringFormats.Center);

                // Save the document...
                document.Save(stream, false);

                return File(stream.ToArray(), "application/pdf");
            }
        }

        public ActionResult TestWithDonor()
        {
            Donor donor = db.Donors.First();
            List<Donation> donations = db.Donations.Where(x => x.DonationDonor_Id.Equals(donor.Id)).ToList();
            using (MemoryStream stream = new MemoryStream())
            {
                // Create output string
                string full = donor.FirstName + " " + donor.LastName + "\n";
                full += donor.Address + "\n";
                full += "Donations:\n";
                foreach (var donation in donations)
                {
                    donation.DonationBatch = db.Batches.Where(x => x.Id.Equals(donation.DonationBatch_Id)).First();
                    if (donation.Value > 0)
                        full += "$" + donation.Value + " to " + donation.DonationBatch.Title
                             + " on " + donation.DonationBatch.CreateDate + "\n";
                    if (donation.ObjectDescription != "")
                        full += "Description: " + donation.ObjectDescription + "\n";
                }
                // Create a new PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Created with PDFsharp";
                // Create an empty page
                PdfPage page = document.AddPage();
                // Get an XGraphics object for drawing
                XGraphics gfx = XGraphics.FromPdfPage(page);
                // Create a font
                XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
                // Create a text formatter to make sure newlines work on the rendered string
                XTextFormatter tf = new XTextFormatter(gfx);
                // Draw a rectangle to contain the text
                XRect rect = new XRect(100, 100, page.Width - 100, page.Height - 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                // Draw the text using the text formatter
                tf.DrawString(full, font, XBrushes.Black, rect, XStringFormats.TopLeft);
                // Save the document...
                document.Save(stream, false);

                return File(stream.ToArray(), "application/pdf");
            }
        }
    }
}