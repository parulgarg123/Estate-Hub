using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Proptiwise.Models
{
    public class SendEmails
    {
        public static string SmtpUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPUsername"];
            }
        }

        public static string SmtpPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPPassword"];
            }
        }
        public static string SmtpPort
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPServerPort"];
            }
        }
        public static string SenderEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["SenderEmail"];
            }
        }
        public static string SmtpHost
        {
            get
            {
                return ConfigurationManager.AppSettings["SmtpHost"];
            }
        }
        public static void SendPDFEmail(DataTable dt, string to, string cc, string bcc, string subject, string body, string heading, string filename)
        {

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    // string companyName = "ASPSnippets";
                    // int orderNo = 2303;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(heading);
                    sb.Append("<br>");
                    sb.Append("<br>");
                    sb.Append("<br>");
                    sb.Append("<table border = '1'>");
                    sb.Append("<tr>");

                    for (int k = 0; k < dt.Columns.Count - 3; k++)
                    {

                        // sb.Append("<th style = 'background-color: #D20B0C;color:#ffffff'>");
                        sb.Append("<th>");
                        sb.Append(dt.Columns[k].ToString());
                        sb.Append("</th>");

                    }
                    sb.Append("</tr>");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("<tr>");
                        for (int j = 0; j < dt.Columns.Count - 3; j++)
                        {

                            sb.Append("<td>");
                            sb.Append(dt.Rows[i][j].ToString());
                            sb.Append("</td>");

                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                    StringReader sr = new StringReader(sb.ToString());
                    Document pdfDoc = new Document();
                    pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                        pdfDoc.Open();
                        htmlparser.Parse(sr);
                        pdfDoc.Close();
                        byte[] bytes = memoryStream.ToArray();
                        memoryStream.Close();
                        MailMessage mm = new MailMessage();
                        mm.From = new MailAddress(SenderEmail);
                        if (to != null && to != "")
                        {
                            string[] multito = to.Split(',');
                            foreach (string idto in multito)
                                mm.To.Add(idto);
                        }
                        if (cc != null && cc != "")
                        {
                            string[] multicc = cc.Split(',');
                            foreach (string idcc in multicc)
                                mm.CC.Add(idcc);
                        }
                        if (bcc != null && bcc != "")
                        {
                            string[] multibcc = bcc.Split(',');
                            foreach (string idbcc in multibcc)
                                mm.Bcc.Add(idbcc);
                        }
                        mm.Subject = subject;
                        mm.Body = body;
                        mm.Attachments.Add(new Attachment(new MemoryStream(bytes), filename + ".pdf"));
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = SmtpHost;
                        smtp.EnableSsl = false;
                        NetworkCredential NetworkCred = new NetworkCredential();
                        NetworkCred.UserName = SmtpUserName;
                        NetworkCred.Password = SmtpPassword;
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = Convert.ToInt32(SmtpPort);
                        smtp.Send(mm);
                    }
                }
            }
        }

        public static void SendNotificationToLandlord(string to, string subject, string body)
        {
            try
            {
                MailMessage mm = new MailMessage();
                mm.From = new MailAddress(SenderEmail);
                mm.To.Add(to);
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = SmtpHost;
                smtp.EnableSsl = false;
                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = SmtpUserName;
                NetworkCred.Password = SmtpPassword;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = Convert.ToInt32(SmtpPort);
                smtp.Send(mm);
            }
            catch(Exception e)
            {
                Console.WriteLine("Email not sent to landlord: '{0}'", e);
            }
        }

        public static void SendNotificationToTenant(string to, string subject, string body)
        {
            try
            {
            MailMessage mm = new MailMessage();
            mm.From = new MailAddress(SenderEmail);
            mm.To.Add(to);
            mm.Subject = subject;
            mm.Body = body;
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = SmtpHost;
            smtp.EnableSsl = false;
            NetworkCredential NetworkCred = new NetworkCredential();
            NetworkCred.UserName = SmtpUserName;
            NetworkCred.Password = SmtpPassword;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = Convert.ToInt32(SmtpPort);
            smtp.Send(mm);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email not sent to tenant: '{0}'", ex);
            }
        }



        }
    }
