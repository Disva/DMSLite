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
                XFont font = new XFont("Verdana", 12, XFontStyle.Regular);

                string nameField = donor.FirstName + " " + donor.LastName;
                string addressField = donor.Address;
                string full = nameField + "\n" + addressField + "\n";

                // Draw the text
                gfx.DrawString(full, font, XBrushes.Black,
                    new XRect(100, 100, page.Width - 100, page.Height - 100),
                    XStringFormats.TopLeft);

                // Save the document...
                document.Save(stream, false);

                return File(stream.ToArray(), "application/pdf");
            }
        }
    }
}