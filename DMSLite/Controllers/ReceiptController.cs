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

namespace DMSLite.Controllers
{
    public class ReceiptController : Controller
    {
        // GET: Receipt
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            // we are using this MemoryStream to be able to access the PDF as an ActionResult.
            // source: https://stackoverflow.com/questions/15121876/how-do-i-display-a-pdf-using-pdfsharp-in-asp-net-mvc#15145131
            //lol
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
    }
}