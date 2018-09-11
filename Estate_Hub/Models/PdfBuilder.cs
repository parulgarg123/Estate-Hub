using iTextSharp.text;
using iTextSharp.text.pdf;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proptiwise.Models
{

    public class PdfBuilder
    {
        private readonly tbl_inventory _post;
        private readonly string _file;
        private readonly string _css;
        public PdfBuilder(tbl_inventory post, string file, string css)
        {
            _post = post;
            _file = file;
            _css = css;
        }
        public FileContentResult GetPdf()
        {
            var html = GetHtml();
            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                using (var doc = new Document(PageSize.A4, 80, 50, 30, 65))
                {
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        doc.Open();
                        try
                        {
                            using (var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                            {
                                iTextSharp.tool.xml.XMLWorkerHelper.GetInstance()
                                    .ParseXHtml(writer, doc, msHtml, System.Text.Encoding.UTF8);
                            }
                        }
                        finally
                        {
                            doc.Close();
                        }
                    }
                }
                bytes = ms.ToArray();
            }
            return new FileContentResult(bytes, "application/pdf");
        }

        public FileStreamResult GetPdf1()
        {
            var html = GetHtml();
            var css = GetCss();
            var bytes = System.Text.Encoding.UTF8.GetBytes(html);
            var bytes1 = System.Text.Encoding.UTF8.GetBytes(css);
            var output = new MemoryStream();
            using (var input = new MemoryStream(bytes))
            {
                var input1 = new MemoryStream(bytes1);
                // this MemoryStream is closed by FileStreamResult

                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4_LANDSCAPE, 50, 50, 50, 50);
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;
                document.Open();

                var xmlWorker = iTextSharp.tool.xml.XMLWorkerHelper.GetInstance();

                xmlWorker.ParseXHtml(writer, document, input, input1);
                document.Close();
                output.Position = 0;
            }
            return new FileStreamResult(output, "application/pdf");


        }
        private string GetHtml()
        {
            var html = File.ReadAllText(_file);

            return Razor.Parse(html, _post);
        }
        private string GetCss()
        {
            var css = File.ReadAllText(_css);
            return css;
        }


        public Byte[] GetPdfbytes()
        {
            var html = GetHtml();
            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                using (var doc = new Document(PageSize.A4, 80, 50, 30, 65))
                {
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        doc.Open();
                        try
                        {
                            using (var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                            {
                                iTextSharp.tool.xml.XMLWorkerHelper.GetInstance()
                                    .ParseXHtml(writer, doc, msHtml, System.Text.Encoding.UTF8);
                            }
                        }
                        finally
                        {
                            doc.Close();
                        }
                    }
                }
                bytes = ms.ToArray();
            }
            return bytes;
        }
    }
}