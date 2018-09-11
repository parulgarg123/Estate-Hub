using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proptiwise.Models;
using Proptiwise.ViewModels;
using System.Net.Mail;
using System.Data;
using PagedList;
using System.Net;
using System.IO;
using System.Text;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Reflection;
using iTextSharp.text.html.simpleparser;
using Microsoft.Reporting.WebForms;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Web.Script.Serialization;


namespace Proptiwise.Controllers
{
    public class FrontController : Controller
    {



        dbrealestateEntities db = new dbrealestateEntities();
        public static int pagesize1, i = 0;
        public static bool ifprpdetail = false;
        static int? page1;
        static int cmd = 0;

        public static string regSmtpUserName
        {
            get
            {
                return ConfigurationManager.AppSettings["regSMTPUsername"];
            }
        }

        public static string regSmtpPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["regSMTPPassword"];
            }
        }

        public static string regSenderEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["regSenderEmail"];
            }
        }

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
        private void SendPDFEmail(DataTable dt, string to, string cc, string bcc, string subject, string body, string heading, string filename)
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

                    for (int k = 0; k < dt.Columns.Count; k++)
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
                        for (int j = 0; j < dt.Columns.Count; j++)
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

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public void ExportToPdf(DataTable myDataTable, string heading)
        {
            //Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            Document pdfDoc = new Document();
            pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            try
            {
                PdfWriter.GetInstance(pdfDoc, System.Web.HttpContext.Current.Response.OutputStream);

                pdfDoc.Open();
                Chunk c = new Chunk(heading, FontFactory.GetFont("Verdana", 10));
                Paragraph p = new Paragraph();
                p.Alignment = Element.ALIGN_CENTER;
                p.Add(c);
                pdfDoc.Add(p);
                Phrase phrase1 = new Phrase(Environment.NewLine);
                pdfDoc.Add(phrase1);
                Font font8 = FontFactory.GetFont("ARIAL", 7);
                DataTable dt = myDataTable;
                if (dt != null)
                {
                    //Craete instance of the pdf table and set the number of column in that table  
                    PdfPTable PdfTable = new PdfPTable(dt.Columns.Count - 3);

                    PdfPCell PdfPCell = null;
                    for (int column = 0; column < dt.Columns.Count - 3; column++)
                    {
                        PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].ToString(), font8)));
                        PdfPCell.Padding = 3;
                        PdfTable.AddCell(PdfPCell);
                    }
                    for (int rows = 0; rows < dt.Rows.Count; rows++)
                    {
                        for (int column = 0; column < dt.Columns.Count - 3; column++)
                        {
                            PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), font8)));
                            PdfPCell.Padding = 3;
                            PdfTable.AddCell(PdfPCell);
                        }
                    }
                    PdfTable.SpacingBefore = 15f; // Give some space after the text or it may overlap the table            
                    pdfDoc.Add(PdfTable); // add pdf table to the document   
                }
                pdfDoc.Close();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename= " + heading + ".pdf");
                System.Web.HttpContext.Current.Response.Write(pdfDoc);
                Response.Flush();
                Response.End();
                //HttpContext.Current.ApplicationInstance.CompleteRequest();  
            }
            catch (DocumentException de)
            {
                System.Web.HttpContext.Current.Response.Write(de.Message);
            }
            catch (IOException ioEx)
            {
                System.Web.HttpContext.Current.Response.Write(ioEx.Message);
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write(ex.Message);
            }
        }

        // GET: /Front/Send Email
        private static void SendMail(string FirstName, string LastName, string Email, string Password, string Mobile, int? UserTypId)
        {
            MailMessage mail1 = new MailMessage();
            string add = Email;
            mail1.To.Add(add);
            mail1.From = new MailAddress(regSenderEmail);
            mail1.Subject = "Proptiwise Confirmation Email";

            string s = "<b>Hi " + FirstName + " " + LastName + "</b><br/>";
            s += "<b>Greetings!!</b><br/>";
            s += "<br/>";
            s += "<br/>";
            if (UserTypId == 1)
                s += "<b>You Are Successfully added as a Landlord in Proptiwise</b>";
            else if (UserTypId == 2)
                s += "<b>You Are Successfully added as a Tenant in Proptiwise</b>";
            else if (UserTypId == 3)
                s += "<b>You Are Successfully added as a Service Provider in Proptiwise</b>";
            s += "<br/>";
            s += "<br/>";
            if (UserTypId == 4)
            {
                s += "<b>User Name : </b>" + Email + "<br/>";
                s += "<b>Password : </b>" + Password + "<br/>";
            }
            else
                s += "<b>User Name : </b>" + Email + "<br/>";
            s += "<b>Phone Number : </b>" + Mobile + "<br/>";
            s += "<br/>";
            s += "Thanking you for your consideration and forthcoming response.<br/>";
            s += "<br/>";
            s += "Warm Regards<br/>";
            s += "Proptiwise<br/>";
            mail1.Body = s;
            mail1.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = SmtpHost;
            smtp.Credentials = new System.Net.NetworkCredential
                 (regSmtpUserName, regSmtpPassword);
            smtp.Port = Convert.ToInt32(SmtpPort);
            smtp.EnableSsl = false;
            smtp.Send(mail1);
            MailMessage mail = new MailMessage();
            mail.To.Add(regSenderEmail);
            mail.From = new MailAddress(regSenderEmail);
            mail.Subject = "Proptiwise Confirmation Email";
            //  string Body = "Hi, this mail is to test sending mail" + "using Gmail in ASP.NET";

            string s1 = "<b>Hi Admin.....</b><br/>";
            s1 += "<b>Greetings!!</b><br/>";
            s1 += "<br/>";
            s1 += "<br/>";
            s1 += "<b>first Name : </b>" + Email + "<br/>";
            s1 += "<b>Last Name : </b>" + Email + "<br/>";
            s1 += "<b>Email Address : </b>" + Email + "<br/>";

            s1 += "<b>Phone Number : </b>" + Mobile + "<br/>";
            s1 += "<br/>";
            s1 += "Thanking you for your consideration and forthcoming response.<br/>";
            s1 += "<br/>";
            s1 += "Warm Regards<br/>";
            s1 += "Proptiwise<br/>";
            mail.Body = s1;
            mail.IsBodyHtml = true;
            SmtpClient smtp1 = new SmtpClient();
            smtp1.Host = SmtpHost;
            smtp1.Credentials = new System.Net.NetworkCredential
                 (regSmtpUserName, regSmtpPassword);
            smtp1.EnableSsl = false;
            smtp1.Send(mail);
        }
        // GET: /Front/AboutUs
        public ActionResult AboutUs()
        {
            return View();
        }
        // GET: /Front/Contact
        public ActionResult ContactUs()
        {
            return View();
        }
        // GET: /Front/Tour
        public ActionResult Tour()
        {
            return View();
        }

        public ActionResult tour_tenants()
        {
            return View();
        }

        public ActionResult tour_properties()
        {
            return View();
        }

        public ActionResult tour_leases()
        {
            return View();
        }

        public ActionResult tour_payments()
        {
            return View();
        }

        public ActionResult tour_accounting()
        {
            return View();
        }

        public ActionResult tour_workorder()
        {
            return View();
        }

        public ActionResult tour_form()
        {
            return View();
        }

        public ActionResult tour_reports()
        {
            return View();
        }

        public ActionResult tour_users()
        {
            return View();
        }

        public ActionResult tour_tenantportal()
        {
            return View();
        }

        public ActionResult tour_tenantscreening()
        {
            return View();
        }

        public ActionResult tour_extras()
        {
            return View();
        }

        public ActionResult tour_screenshots()
        {
            return View();
        }

        // GET: /Front/Home
        public ActionResult Home()
        {
            HttpCookie countryCookie = Request.Cookies["countryCookie"];
            if (countryCookie != null)
            {
                Session["Country"] = countryCookie["Country"];
                //country name displaying
                int c = Convert.ToInt32(Session["Country"]);
                var coo = db.Countries.Where(p => p.countryId == c).FirstOrDefault();
                Session["cname"] = coo.countryName;
            }
                ViewBag.country = new SelectList(db.Countries.ToList(), "countryId", "countryName");
            if (Session["LandlordId"] != null && Session["UserRole"] != null)
            {
                Int64 lid = Convert.ToInt64(Session["LandlordId"]);
                var data = db.Users.Where(p => p.UserID == lid).FirstOrDefault();
                ViewBag.UserName = data.FirstName + " " + data.LastName;
            }

            if (Session["TenantId"] != null)
            {
                Int64 tid = Convert.ToInt64(Session["TenantId"]);
                var data1 = db.Tenants.Where(p => p.Tenant_Id == tid).FirstOrDefault();
                ViewBag.UserName = data1.FirstName + " " + data1.LastName;
            }
            if (Session["ProviderId"] != null)
            {
                Int64 pid = Convert.ToInt64(Session["ProviderId"]);
                var data2 = db.Serviceproviders.Where(p => p.ProviderId == pid).FirstOrDefault();
                ViewBag.UserName = data2.FirstName + " " + data2.LastName;
            }

            return View();
        }
        [HttpPost]
        public ActionResult Home(int? country)
        {
            ViewBag.country = new SelectList(db.Countries.ToList(), "countryId", "countryName");
            if (country != null)
            {
                HttpCookie countryCookie1 = Request.Cookies["countryCookie"];
                if (countryCookie1 == null)
                {
                    HttpCookie countryCookie = new HttpCookie("countryCookie");
                    countryCookie.Name = "countryCookie";
                    countryCookie.Expires = DateTime.Now.AddMinutes(30);
                    countryCookie.Values.Add("Country", country.ToString());
                    Response.Cookies.Set(countryCookie);
                    Session["Country"] = country;
                    int c = Convert.ToInt32(Session["Country"]);
                    var coo = db.Countries.Where(p => p.countryId == c).FirstOrDefault();
                    Session["cname"] = coo.countryName;
                }               
            }
            else
                ViewBag.ErrorMessage = "Please Select country ";
                
            
            return View();
        }

        // GET: /Front/Login
        public ActionResult Login(LoginModel user, int? i)
        {
            if (Session["LandlordId"] != null && Session["UserRole"] != null)
                return RedirectToAction("Dashboard");
            if (Session["TenantId"] != null)
                return RedirectToAction("TenantHome", "Tenant");
            if (Session["ProviderId"] != null)
                return RedirectToAction("Myaccountprovider");
            HttpCookie UserDetailsCookie = Request.Cookies["UserDetails"];
            if (UserDetailsCookie != null)
            {
                user.Email = UserDetailsCookie["UserName"];
                user.Password = UserDetailsCookie["Password"];
                return View(user);
            }
            else
                return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel loginmodel)
        {
            var data = db.Users.Where(p => p.Email == loginmodel.Email && p.Password == loginmodel.Password).FirstOrDefault();
            var data1 = db.Tenants.Where(p => p.Email == loginmodel.Email && p.Password == loginmodel.Password).FirstOrDefault();
            var data2 = db.Serviceproviders.Where(p => p.Email == loginmodel.Email && p.Password == loginmodel.Password).FirstOrDefault();
            if (data != null)
            {
                if (loginmodel.rememberme == true)
                {
                    HttpCookie UserDetailsCookie = new HttpCookie("UserDetails");
                    UserDetailsCookie.Name = "UserDetails";
                    UserDetailsCookie.Expires = DateTime.MaxValue;
                    UserDetailsCookie.Values.Add("UserName", loginmodel.Email);
                    UserDetailsCookie.Values.Add("Password", loginmodel.Password);
                    UserDetailsCookie.Values.Add("LandlordIdCookies", data.UserID.ToString());
                    Response.Cookies.Set(UserDetailsCookie);
                }
                else
                {
                    Response.Cookies["UserDetails"].Expires = DateTime.Now;
                }
                if (data.UserStatusID == 2 || data.UserStatusID == 4)
                {
                    Session["EmailId"] = data.Email;
                    Session["EmailId1"] = data.Email;
                    Session["LandlordId"] = data.UserID;
                    Session["UserRole"] = data.UserRole1.RoleName;
                    Session["UserRoleId"] = data.RoleId;
                    Session["Country"] = data.Country;
                    HttpCookie UserDetailsCookie = new HttpCookie("EmailIdCookies");
                    UserDetailsCookie.Name = "EmailIdCookies";
                    UserDetailsCookie.Expires = DateTime.Now.AddMinutes(60);
                    UserDetailsCookie.Values.Add("EmailIdCookies", data.Email);
                    UserDetailsCookie.Values.Add("LandlordIdCookies", data.UserID.ToString());
                    Response.Cookies.Set(UserDetailsCookie);
                    return RedirectToAction("Dashboard");
                }
                else if (data.UserStatusID == 1)
                {
                    Session["EmailId"] = data.Email;
                    Session["LandlordId"] = data.UserID;
                    HttpCookie UserDetailsCookie = new HttpCookie("EmailIdCookies");
                    UserDetailsCookie.Name = "EmailIdCookies";
                    UserDetailsCookie.Expires = DateTime.Now.AddMinutes(60);
                    UserDetailsCookie.Values.Add("EmailIdCookies", data.Email);
                    Response.Cookies.Set(UserDetailsCookie);
                    return RedirectToAction("Plans");
                }
            }
            else if (data1 != null)
            {
                if (loginmodel.rememberme == true)
                {
                    HttpCookie UserDetailsCookie = new HttpCookie("UserDetails");
                    UserDetailsCookie.Name = "UserDetails";
                    UserDetailsCookie.Expires = DateTime.MaxValue;
                    UserDetailsCookie.Values.Add("UserName", loginmodel.Email);
                    UserDetailsCookie.Values.Add("Password", loginmodel.Password);
                    Response.Cookies.Set(UserDetailsCookie);
                }
                else
                {
                    Response.Cookies["UserDetails"].Expires = DateTime.Now;
                }
                Session["EmailId1"] = data1.Email;
                Session["EmailIdTenant"] = data1.Email;
                Session["TenantId"] = data1.Tenant_Id;
                HttpCookie UserDetailsCookie1 = new HttpCookie("EmailIdCookies");
                UserDetailsCookie1.Name = "EmailIdCookies";
                UserDetailsCookie1.Expires = DateTime.Now.AddMinutes(60);
                UserDetailsCookie1.Values.Add("EmailIdCookies", data1.Email);
                Response.Cookies.Set(UserDetailsCookie1);
                return RedirectToAction("TenantHome", "Tenant");
            }
            else if (data2 != null)
            {
                if (loginmodel.rememberme == true)
                {
                    HttpCookie UserDetailsCookie = new HttpCookie("UserDetails");
                    UserDetailsCookie.Name = "UserDetails";
                    UserDetailsCookie.Expires = DateTime.MaxValue;
                    UserDetailsCookie.Values.Add("UserName", loginmodel.Email);
                    UserDetailsCookie.Values.Add("Password", loginmodel.Password);
                    Response.Cookies.Set(UserDetailsCookie);
                }
                else
                {
                    Response.Cookies["UserDetails"].Expires = DateTime.Now;
                }
                Session["EmailId1"] = data2.Email;
                Session["emailidprovider"] = data2.Email;
                Session["ProviderId"] = data2.ProviderId;
                HttpCookie UserDetailsCookie1 = new HttpCookie("EmailIdCookies");
                UserDetailsCookie1.Name = "EmailIdCookies";
                UserDetailsCookie1.Expires = DateTime.Now.AddMinutes(60);
                UserDetailsCookie1.Values.Add("EmailIdCookies", data2.Email);
                Response.Cookies.Set(UserDetailsCookie1);
                return RedirectToAction("Myaccountprovider");
            }
            else
            {
                ViewBag.LoginMessage = "Wrong UserName/Password";
                return View("Login");
            }

            return View("Login");
        }
        // GET: /Front/Register
        public ActionResult Register()
        {
            if (Session["LandlordId"] != null && Session["UserRole"] != null)
                return RedirectToAction("Dashboard");
            if (Session["TenantId"] != null)
                return RedirectToAction("TenantHome", "Tenant");
            if (Session["ProviderId"] != null)
                return RedirectToAction("Myaccountprovider");
            if (Session["Country"] == null)
                return RedirectToAction("Home");
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserModel usermodel)
        {
            ViewBag.country = new SelectList(db.Countries.ToList(), "countryId", "countryName");
            if (ModelState.IsValid)
            {
                User users = new User();
                HttpCookie countryCookie = Request.Cookies["countryCookie"];
                if (countryCookie != null)
                {
                    users.Country = Convert.ToInt32(countryCookie["Country"]);                  
                    //return View(countryCookie);
                }               
                users.FirstName = usermodel.FirstName;
                users.LastName = usermodel.LastName;
                users.Email = usermodel.Email;
                users.Phone = usermodel.Phone;
                users.Mobile = usermodel.Mobile;
                users.Password = usermodel.Password;
               // users.Country = usermodel.Country;
                var existingUser = db.Users.FirstOrDefault(u => u.Email == users.Email);
                var existingUser1 = db.Tenants.FirstOrDefault(u => u.Email == users.Email);
                var existingUser2 = db.Serviceproviders.FirstOrDefault(u => u.Email == users.Email);
                if (existingUser != null)
                {
                    ViewBag.Register = "Landlord already exist.";
                    return View(usermodel);
                }
                else if (existingUser1 != null)
                {
                    ViewBag.Register = "Tenant already exist.";
                    return View(usermodel);
                }
                else if (existingUser2 != null)
                {
                    ViewBag.Register = "Service Provider already exist.";
                    return View(usermodel);
                }
                else
                {
                    if (usermodel.UserTypeID == 1)
                    {
                        User user = new User();
                        user.FirstName = users.FirstName;
                        user.LastName = users.LastName;
                        user.Email = users.Email;
                        user.Phone = users.Phone;
                        user.Mobile = users.Mobile;
                        user.Password = users.Password;
                        user.UserTypeID = usermodel.UserTypeID;
                        user.UserStatusID = 1;
                        user.DateCreated = DateTime.Now.Date;
                        user.Country = users.Country;
                        db.Users.Add(user);
                        db.SaveChanges();
                        Session["EmailId"] = user.Email;
                        Session["LandlordId"] = user.UserID;
                        SendMail(user.FirstName, user.LastName, user.Email, "", user.Mobile, usermodel.UserTypeID);
                        return RedirectToAction("Plans");
                    }
                    else if (usermodel.UserTypeID == 2)
                    {
                        Tenant abc = new Tenant();
                        abc.FirstName = users.FirstName;
                        abc.LastName = users.LastName;
                        abc.Telephone = users.Phone;
                        abc.Cellular = users.Mobile;
                        abc.Email = users.Email;
                        abc.Password = users.Password;
                        abc.Country = usermodel.Country;
                        db.Tenants.Add(abc);
                        db.SaveChanges();
                        SendMail(abc.FirstName, abc.LastName, abc.Email, "", abc.Cellular, usermodel.UserTypeID);
                        Session["Tenant"] = abc.Email;
                        return RedirectToAction("SendInvites");
                    }
                    else if (usermodel.UserTypeID == 3)
                    {
                        Serviceprovider abc = new Serviceprovider();
                        abc.FirstName = users.FirstName;
                        abc.LastName = users.LastName;
                        abc.Phone = users.Phone;
                        abc.Mobile = users.Mobile;
                        abc.Email = users.Email;
                        abc.Password = users.Password;
                        //abc.couc = users.Country;
                        db.Serviceproviders.Add(abc);
                        db.SaveChanges();
                        SendMail(abc.FirstName, abc.LastName, abc.Email, abc.Password, abc.Mobile, usermodel.UserTypeID);
                        Session["provideremail"] = abc.Email;
                        return RedirectToAction("providerhome");
                    }
                    else if (usermodel.UserTypeID == 4)
                    {
                        User user = new User();
                        user.FirstName = users.FirstName;
                        user.LastName = users.LastName;
                        user.Email = users.Email;
                        user.Phone = users.Phone;
                        user.Mobile = users.Mobile;
                        user.Password = users.Password;
                        user.UserTypeID = usermodel.UserTypeID;
                        user.RoleId = 1;
                        user.UserStatusID = 2;
                        user.DateCreated = DateTime.Now.Date;
                        user.Country = usermodel.Country;
                        db.Users.Add(user);
                        db.SaveChanges();
                        Session["EmailId1"] = user.Email;
                        Session["LandlordId"] = user.UserID;
                        //SendMail(user.FirstName, user.LastName, user.Email, "", user.Mobile, UserTypeID);
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        return View();
                    }

                }

            }
            else
            {
                //ViewBag.Register = "Enter Valid Data";
                return View(usermodel);
            }
        }
        //Front/Plans
        public ActionResult Plans()
        {
            var q = db.Plans.Distinct().ToList();
            ViewBag.GetPlansData = q;
            return View(q);
        }
        [HttpPost]
        public ActionResult Plans(int? submit)
        {
            if (Session["EmailId"] != null)
            {
                Session["Plan"] = submit;
                return RedirectToAction("Signup1");
            }
            else
                return RedirectToAction("Register");
        }
        // Front : Get States
        //public JsonResult GetStates(int id)
        //{
        //    var q = db.States.Where(p => p.CountryId == id).ToList();
        //    return Json(new SelectList(q.ToArray(), "State_Id", "StateName"), JsonRequestBehavior.AllowGet);
        //}
        // Front : Get Cities
        //public JsonResult GetCities(int id)
        //{
        //    var q = db.Cities.Where(p => p.StateId == id).ToList();
        //    return Json(new SelectList(q.ToArray(), "City_Id", "CityName"), JsonRequestBehavior.AllowGet);
        //}
        // Front : Registration step 2
        public ActionResult Signup1()
        {
            string email = "";
            if (Session["EmailId"] != null)
            {
                email = Session["EmailId"].ToString();
            }
            var q = db.Users.Where(p => p.Email == email).FirstOrDefault();
            ViewBag.country = new SelectList(db.Countries.ToList(), "countryId", "countryName");
            ViewBag.UserValues = q;
            return View();
        }
        [HttpPost]
        public ActionResult Signup1(UserModel users)
        {
            User user = db.Users.Where(p => p.Email == users.Email).FirstOrDefault();
            if (user != null)
            {
                user.FirstName = users.FirstName;
                user.LastName = users.LastName;
                user.Address1 = users.Address1;
                user.BuildingName = users.BuildingName;
                user.UnitNumber = users.UnitNumber;
                user.Suberb = users.Suberb;
                user.Province = users.Province;
                user.StreetAddress = users.StreetAddress;
                user.Town = users.Town;
                user.City = users.City;
                user.Country = user.Country;
                user.County = users.County;
                user.Zip = users.Zip;
                user.PostCode = users.PostCode;
                user.State = users.State;
                user.Phone = users.Phone;
                user.Email = user.Email;
                user.UserStatusID = 1;
                user.RoleId = 1;
                user.UserRole = 1;
                user.DateCreated = DateTime.Now.Date;
                db.SaveChanges();
                Session["EmailId"] = user.Email;
                return RedirectToAction("Signup2");
            }
            else
            {
                return View();
            }

        }
        // Front : Registration step 3
        public ActionResult Signup2()
        {
            if (Session["Plan"] != null)
            {
                int planid = Convert.ToInt32(Session["Plan"].ToString());

                var q = db.Plans.Where(p => p.PlanID == planid).FirstOrDefault();
                ViewBag.PlanData = q;

            }
            return View();
        }
        [HttpPost]
        public ActionResult Signup2(User user)
        {
            return View();
        }
        [SessionExpire]
        public ActionResult Calender()
        {
            return View();
        }
        [SessionExpire]
        public ActionResult getevents()
        {
            List<Cal_Events> lstceve = new List<Cal_Events>();
            var eventList = db.Events.ToList();
            foreach (var q in eventList)
            {
                string startdate = q.startDate.Value.ToString("s");
                string enddate = q.enddate.Value.ToString("s");
                Cal_Events ceve = new Cal_Events();
                ceve.id = Convert.ToString(q.EventId);
                ceve.title = q.Title;
                ceve.start = startdate;
                ceve.end = enddate;
                ceve.allDay = Convert.ToBoolean(q.alldays);
                lstceve.Add(ceve);
            }
            var rows = lstceve.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        [SessionExpire]
        public ActionResult AddEvents(int? id)
        {
            long? lid = 0;
            if (Session["LandlordId"] != null)
                lid = Convert.ToInt64(Session["LandlordId"]);
            else
                return RedirectToAction("login");
            var q = db.Events.Where(p => p.Landlord_Id == lid && p.EventId == id).FirstOrDefault();
            return View(q);
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult AddEvents(Event events)
        {
            long? id = 0;
            if (Session["LandlordId"] != null)
                id = Convert.ToInt64(Session["LandlordId"]);
            else
                return RedirectToAction("login");
            Event evnt = db.Events.Find(events.EventId);
            if (evnt == null)
            {
                Event evntadd = new Event();
                evntadd.Title = events.Title;
                evntadd.startDate = events.startDate;
                evntadd.enddate = events.enddate;
                evntadd.dateAdded = DateTime.UtcNow.Date;
                evntadd.alldays = events.alldays;
                evntadd.Details = events.Details;
                evntadd.Landlord_Id = id;
                db.Events.Add(evntadd);
                db.SaveChanges();
                return RedirectToAction("Calender");
            }
            else
            {
                evnt.Title = events.Title;
                evnt.startDate = events.startDate;
                evnt.Details = events.Details;
                evnt.enddate = events.enddate;
                evnt.alldays = events.alldays;
                db.SaveChanges();
                return RedirectToAction("Calender");
            }
        }
        [SessionExpire]
        public ActionResult EditEvents(int? id)
        {
            var q = db.Events.Find(id);
            return View(q);
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult EditEvents(int? id, Event events)
        {
            Event evnt = db.Events.Find(id);
            evnt.Title = events.Title;
            evnt.startDate = events.startDate;
            evnt.enddate = events.enddate;
            evnt.alldays = events.alldays;
            db.SaveChanges();
            //return Json(new { success = true });
            return RedirectToAction("Calender");
        }
        [SessionExpire]

        public ActionResult paypal_paid()
        {
            //if (Request["Payment_status"] == "Completed")
            //{
            string trans = Request["tx"];
            string sAmountPaid = Request["amt"];
            long? id = Convert.ToInt64(Request["cm"]);
            var emailid = (db.Transactions.Select(p => p.TransId).Contains(trans));
            //if (emailid == false)
            //{
            var data = db.Users.Where(p => p.UserID == id).FirstOrDefault();
            data.UserStatusID = 2;
            data.RoleId = 1;
            db.SaveChanges();
            Transaction td = new Transaction();
            td.TransId = trans;
            td.AmountPaid = sAmountPaid;
            td.LandlordId = data.UserID;
            td.DateAdded = DateTime.Now;
            db.Transactions.Add(td);
            db.SaveChanges();
            Session["EmailId1"] = data.Email;
            Session["UserRoleId"] = data.RoleId;
            Session["UserRole"] = data.UserRole1.RoleName;
            Session["EmailId"] = data.Email;
            MailMessage mail1 = new MailMessage();
            string add = data.Email;
            mail1.To.Add(add);
            mail1.From = new MailAddress(regSenderEmail);
            mail1.Subject = "Account Created Successfully";
            string s = "<b>Helloooooo  </b><br/>";
            s += "<b>Greetings!!" + data.FirstName + " " + data.LastName + "</b><br/>";
            s += "Congrats your Account has been successfully created.<br/> Thanks";
            mail1.Body = s;
            mail1.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = SmtpHost;
            smtp.Credentials = new System.Net.NetworkCredential(regSmtpUserName, regSmtpPassword);
            smtp.Port = Convert.ToInt32(SmtpPort);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = false;
            smtp.Send(mail1);
            return RedirectToAction("Dashboard");
            // }
            //  else
            // {

            // }
            //}
            //return View();
        }

        [SessionExpire]
        public ActionResult Ipn2(long? id)
        {
            if (Request["Payment_status"] == "Completed")
            {
                string trans = Request["txn_id"];
                string sAmountPaid = Request["mc_gross"];
                string deviceID = Request["custom"];
                var emailid = (db.Transactions.Select(p => p.TransId).Contains(trans));
                if (emailid == false)
                {
                    var data = db.Users.Where(p => p.UserID == id).FirstOrDefault();
                    data.UserStatusID = 2;
                    data.RoleId = 1;
                    db.SaveChanges();
                    Transaction td = new Transaction();
                    td.TransId = trans;
                    td.AmountPaid = sAmountPaid;
                    td.LandlordId = data.UserID;
                    td.DateAdded = DateTime.Now;
                    db.Transactions.Add(td);
                    db.SaveChanges();
                    Session["EmailId1"] = data.Email;
                    Session["UserRoleId"] = data.RoleId;
                    Session["UserRole"] = data.UserRole1.RoleName;
                    Session["EmailId"] = data.Email;
                    MailMessage mail1 = new MailMessage();
                    string add = data.Email;
                    mail1.To.Add(add);
                    mail1.From = new MailAddress(regSenderEmail);
                    mail1.Subject = "Account Created Successfully";
                    string s = "<b>Helloooooo  </b><br/>";
                    s += "<b>Greetings!!" + data.FirstName + " " + data.LastName + "</b><br/>";
                    s += "Congrats your Account has been successfully created.<br/> Thanks";
                    mail1.Body = s;
                    mail1.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = SmtpHost;
                    smtp.Credentials = new System.Net.NetworkCredential(regSmtpUserName, regSmtpPassword);
                    smtp.Port = Convert.ToInt32(SmtpPort);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = false;
                    smtp.Send(mail1);
                }
                else
                {

                }
            }
            return View();
        }

        // Front / Invite
        public ActionResult SendInvites()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendInvites(Invite invite)
        {
            if (Session["Tenant"] != null)
            {
                Invite obj = new Invite();
                obj.LandlordEmail = invite.LandlordEmail;
                obj.LandlordName = invite.LandlordName;
                obj.PropertyAddress = invite.PropertyAddress;
                obj.PropertyType = invite.PropertyType;

                obj.Tenant_id = Session["Tenant"].ToString();
                obj.Message = invite.Message;
                db.Invites.Add(obj);
                db.SaveChanges();

                MailMessage mail1 = new MailMessage();
                string add = obj.LandlordEmail;
                mail1.To.Add(add);
                mail1.From = new MailAddress(SenderEmail);
                mail1.Subject = "Proptiwise Confirmation Email";
                //  string Body = "Hi, this mail is to test sending mail" + "using Gmail in ASP.NET";

                string s = "<b>Hi " + obj.LandlordEmail + "</b><br/>";
                s += "<b>Greetings!!</b><br/>";
                s += "<br/>";
                s += "<br/>";
                s += "<b>Please Register in Proptiwise as a Landlord.</b>";
                s += "<br/>";
                s += "<br/>";

                s += "Warm Regards<br/>";
                s += obj.Tenant_id + "<br/>";
                s += "Proptiwise<br/>";
                mail1.Body = s;
                mail1.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = SmtpHost;
                smtp.Credentials = new System.Net.NetworkCredential
                     (regSmtpUserName, regSmtpPassword);
                smtp.Port = Convert.ToInt32(SmtpPort);
                smtp.EnableSsl = false;
                smtp.Send(mail1);
                ViewBag.SendInvites = "Invited Successfully. Please Login to Continue with your account";
                return View();
            }
            else
            {
                ViewBag.SendInvites = "Something Went Wrong";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Login");

        }

        public ActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(string Email)
        {
            var lst = db.Tenants.Where(p => p.Email == Email).FirstOrDefault();
            var lst1 = db.Users.Where(p => p.Email == Email).FirstOrDefault();
            if (lst1 != null)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(Email);
                mail.From = new MailAddress(SenderEmail);
                mail.Subject = "Password Email";
                string s = "<b>Hi...</b><br/>";
                s += "<b>Greetings!!</b><br/>";
                s += "<br/>";
                s += "<br/>";
                s += "<b>Email Address : </b>" + Email + "<br/>";
                s += "<b>Password : </b>" + lst1.Password + "<br/>";
                s += "<br/>";
                s += "<br/>";
                s += "<b> Best Regards,</b><br/>";
                s += "<b>RvTech Team</b><br/>";
                mail.Body = s;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = SmtpHost;
                smtp.Credentials = new System.Net.NetworkCredential
                     (regSmtpUserName, regSmtpPassword);
                smtp.Port = Convert.ToInt32(SmtpPort);
                smtp.EnableSsl = false;
                smtp.Send(mail);
                ViewBag.Message = "Your Password has Been Sent To Your Email.Thanks";
                return View();
            }
            if (lst != null)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(Email);
                mail.From = new MailAddress(SenderEmail);
                mail.Subject = "Password Email";
                string s = "<b>Hi...</b><br/>";
                s += "<b>Greetings!!</b><br/>";
                s += "<br/>";
                s += "<br/>";
                s += "<b>Email Address : </b>" + Email + "<br/>";
                s += "<b>Password : </b>" + lst.Password + "<br/>";
                s += "<br/>";
                s += "<br/>";
                s += "<b> Best Regards,</b><br/>";
                s += "<b>RVtech Team</b><br/>";
                mail.Body = s;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = SmtpHost;
                smtp.Credentials = new System.Net.NetworkCredential
                     (regSmtpUserName, regSmtpPassword);
                smtp.Port = Convert.ToInt32(SmtpPort);
                smtp.EnableSsl = false;
                smtp.Send(mail);
                ViewBag.Message = "Your Password has Been Sent To Your Email.Thanks";
                return View();
            }
            else
            {
                ViewBag.Message = "Customer does not have an account with this email.Please enter valid email.Thanks";
                return View();
            }

        }
        // Front / Dashboard
        [SessionExpire]
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult CreateProperty(long? id)
        {
            HttpCookie UserDetailsCookie = Request.Cookies["UserDetails"];
            if (UserDetailsCookie != null)
            {
                Session["LandlordId"] = UserDetailsCookie["LandlordIdCookies"];
            }
            long lid = Convert.ToInt64(Session["LandlordId"]);
            var q = db.PropertyTypes.OrderBy(p => p.PropertyTypeName).ToList();
            ViewBag.PropertyTypeId = new SelectList(q, "PropertyType_Id", "PropertyTypeName");
            ViewBag.Building = new SelectList(db.Buildings.Where(p => p.LandlordId == lid).ToList(), "Building_Id", "BuildingName");
            ViewBag.Country = db.Users.Where(p => p.UserID == lid).Select(p => p.Country).FirstOrDefault();
            int pid = Convert.ToInt32(Session["propertyid"]);
            var data = db.Properties.Where(t => t.Property_Id == pid).FirstOrDefault();
            //  ViewBag.PropertyType = new SelectList(db.PropertyTypes, "PropertyType_Id", "PropertyTypeName");
            ViewBag.PropertyStyle = new SelectList(db.PropertyStyles, "Propertystyle_Id", "Style_Name");
            ViewBag.Parking = new SelectList(db.PropertyParkings, "Propertyparking_Id", "ParkingName");
            ViewBag.Heating = new SelectList(db.PropertyHeatings, "PropertyHeating_Id", "HeatingName");
            ViewBag.AirConditioning = new SelectList(db.PropertyAirConditionings, "AirConditioning_Id", "AirConditioningName");
            ViewBag.Flooring = new SelectList(db.PropertyFloorings, "PropertyFlooring_Id", "FlooringName");
            var prop = db.Properties.Where(p => p.Property_Id == id).FirstOrDefault();
            return View(prop);
        }
        [HttpPost]
        public ActionResult CreateProperty(Property prop, IEnumerable<HttpPostedFileBase> ImageName, int? HoldingFee, int? BuildingId, int? PropertyTypeId, int? PropertyStyleId)
        {
            var property = db.Properties.Where(p => p.Property_Id == prop.Property_Id).FirstOrDefault();
            string PropertyImages = "";
            if (ImageName != null)
            {
                string path = Server.MapPath("~/Folders/PropertyImages/");
                foreach (HttpPostedFileBase images in ImageName)
                {
                    if (images != null)
                    {
                        if (images.ContentLength > 0)
                        {
                            images.SaveAs(path + images.FileName);
                        }
                        PropertyImages += images.FileName + ",";
                    }
                }
            }
            long? lid = Convert.ToInt64(Session["LandlordId"]);
            string country = db.Users.Where(p => p.UserID == lid).Select(p => p.Country1.countryName).FirstOrDefault();
            if (property == null)
            {
                Property obj = new Property();
                obj.PropertyName = prop.PropertyName;
                obj.PostCode = prop.PostCode;
                obj.BuildingName = prop.BuildingName;
                obj.UnitNumber = prop.UnitNumber;
                obj.Suberb = prop.Suberb;
                obj.Province = prop.Province;
                obj.AddressLine1 = prop.AddressLine1;
                obj.StreetAddress = prop.StreetAddress;
                obj.Town = prop.Town;
                obj.City = prop.City;
                obj.Country = country;
                obj.Zip = prop.Zip;
                obj.State = prop.State;
                obj.PropertyTypeId = PropertyTypeId;
                //obj.BuildingId = BuildingId;
                obj.Size = prop.Size;
                obj.leasestatus = 0;
                obj.Created = DateTime.UtcNow.Date;
                obj.Archievestatus = 0;
                obj.Latitude = prop.Latitude;
                obj.Longitude = prop.Longitude;
                obj.DepositReq = prop.DepositReq;
                obj.Price = prop.Price;
                obj.MinimumRent = prop.MinimumRent;
                obj.LateFee = (prop.Price * 0.1);
                obj.Furnished = prop.Furnished;
                obj.VacancyDate = DateTime.Now.Date;
                obj.Landlord_Id = lid;
                obj.Country = country;
                obj.HoldingFee = HoldingFee;
                obj.IfHoldingFee = prop.IfHoldingFee;
                obj.PropertyStyleId = PropertyStyleId;
                obj.Parking_Id = prop.Parking_Id;
                obj.Heating_Id = prop.Heating_Id;
                obj.AirConditioning_Id = prop.AirConditioning_Id;
                obj.Flooring_Id = prop.Flooring_Id;
                obj.SqFt = prop.SqFt;
                obj.Bedroom = prop.Bedroom;
                obj.Bathroom = prop.Bathroom;
                obj.YearBuilt = prop.YearBuilt;
                obj.studyRoom = prop.studyRoom;
                obj.toilet = prop.toilet;
                obj.UtilityRoom = prop.UtilityRoom;
                obj.Garage = prop.Garage;
                obj.Kitchen = prop.Kitchen;
                obj.Description = prop.Description;
                obj.PropertyPhotos = PropertyImages.TrimEnd(',');
                db.Properties.Add(obj);
                db.SaveChanges();
            }
            else
            {
                if (property.PropertyPhotos != null)
                {
                    if (PropertyImages != "")
                        property.PropertyPhotos += "," + PropertyImages.TrimEnd(',');
                }
                else if (PropertyImages != "")
                    property.PropertyPhotos = PropertyImages.TrimEnd(',');
                property.PropertyName = prop.PropertyName;
                property.PostCode = prop.PostCode;
                property.BuildingName = prop.BuildingName;
                property.UnitNumber = prop.UnitNumber;
                property.Suberb = prop.Suberb;
                property.Province = prop.Province;
                property.AddressLine1 = prop.AddressLine1;
                property.StreetAddress = prop.StreetAddress;
                property.Town = prop.Town;
                property.City = prop.City;
                property.Zip = prop.Zip;
                property.State = prop.State;
                property.PropertyTypeId = PropertyTypeId;
                property.BuildingId = BuildingId;
                property.Latitude = prop.Latitude;
                property.Longitude = prop.Longitude;
                property.DepositReq = prop.DepositReq;
                property.Price = prop.Price;
                property.MinimumRent = prop.MinimumRent;
                property.LateFee = (prop.Price * 0.1);
                property.Furnished = prop.Furnished;
                property.VacancyDate = DateTime.Now.Date;
                property.Landlord_Id = lid;
                property.Country = country;
                property.HoldingFee = HoldingFee;
                property.IfHoldingFee = prop.IfHoldingFee;
                property.PropertyStyleId = PropertyStyleId;
                property.Parking_Id = prop.Parking_Id;
                property.Heating_Id = prop.Heating_Id;
                property.AirConditioning_Id = prop.AirConditioning_Id;
                property.Flooring_Id = prop.Flooring_Id;
                property.SqFt = prop.SqFt;
                property.Bedroom = prop.Bedroom;
                property.Bathroom = prop.Bathroom;
                property.YearBuilt = prop.YearBuilt;
                property.studyRoom = prop.studyRoom;
                property.toilet = prop.toilet;
                property.UtilityRoom = prop.UtilityRoom;
                property.Garage = prop.Garage;
                property.Kitchen = prop.Kitchen;
                property.Description = prop.Description;
                db.SaveChanges();
            }
            if (ifprpdetail == true)
            {
                return RedirectToAction("property_details", new { id = prop.Property_Id });
            }
            else
            {
                return RedirectToAction("propertyIndex", new { tab = "tab-2" });
            }
        }

        [SessionExpire]
        private void GetSearchBy_Property()
        {
            var selectedvalue = Request.Form["SearchBy"];
            var SearchBy = new List<SelectListItem>
            {
                   new SelectListItem{ Text="Property Name", Value = "PropertyName" },
                new SelectListItem{ Text="City", Value = "City" },
                new SelectListItem{ Text="Size", Value = "Size" },
                  new SelectListItem{ Text="Bedroom", Value = "Bed" },
                new SelectListItem{ Text="Bathroom", Value = "Bath" },
                new SelectListItem{ Text="Building", Value = "BuildingId" }
            };
            ViewBag.SearchBy = new SelectList(SearchBy.ToList(), "Value", "Text", selectedvalue);
        }
        [SessionExpire]
        public ActionResult PropertyIndex(int? page, string tab, string searchdata, string Command, string SearchBy, string SearchBy1)
        {
            ifprpdetail = false;
            GetSearchBy_Property();
            long? lid = 0;
            if(Session["LandlordId"]!=null)
            lid =Convert.ToInt64(Session["LandlordId"]);
            var lst = db.Properties.Where(p => p.leasestatus == 0 && p.Archievestatus == 0 && p.Landlord_Id==lid).ToList();
            var lst1 = db.Properties.Where(p => p.leasestatus == 1 && p.Archievestatus == 0 && p.Landlord_Id == lid).ToList();
            var arlist = db.Properties.Where(p => p.Archievestatus == 1 && p.Landlord_Id == lid).ToList();

            ViewBag.searchdata = searchdata;

            Session["arlist"] = arlist.OrderByDescending(p => p.Property_Id);
            int pageSize = 10;
            if (pagesize1 != 0)
                pageSize = pagesize1;
            int pageNumber = (page ?? 1);
            page1 = page;
            ViewBag.CurrentPage = page1;
            if (tab == "tab-1" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["lst1"] = lst1.OrderByDescending(p => p.Property_Id).ToPagedList(pageNumber, pageSize);
            }
            if (tab == "tab-2" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["lst"] = lst.OrderByDescending(p => p.Property_Id).ToPagedList(pageNumber, pageSize);
            }
            ViewBag.tab = tab;
            return View();

        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        [SessionExpire]
        public ActionResult PropertyIndex(int? page, string tab, string getid, string getid1, string Command, string lnkto1, string lnkcc1, string lnkBcc1, string subject, string searchdata1, string body, string searchdata, string SearchBy, string SearchBy1)
        {
            ViewBag.tab = tab;
            GetSearchBy_Property();
            if (getid != null && getid != "")
            {
                string[] gid = getid.Split(',');
                List<int> cid = new List<int>();
                int ii = gid.Length;
                for (int i = 0; i < ii; i++)
                {
                    cid.Add(Convert.ToInt32(gid[i]));
                }
                foreach (var id in cid)
                {
                    var dep = db.Properties.Single(s => s.Property_Id == id);
                    dep.Archievestatus = 1;
                    db.SaveChanges();
                }
            }
            if (getid1 != null && getid1 != "")
            {
                string[] gid = getid1.Split(',');
                List<int> cid = new List<int>();
                int ii = gid.Length;
                for (int i = 0; i < ii; i++)
                {
                    cid.Add(Convert.ToInt32(gid[i]));
                }
                foreach (var id in cid)
                {
                    var dep = db.Properties.Single(s => s.Property_Id == id);
                    db.Properties.Remove(dep);
                    db.SaveChanges();
                }
            }
            long? lid = 0;
            if (Session["LandlordId"] != null)
                lid = Convert.ToInt64(Session["LandlordId"]);
            var lst = db.Properties.Where(prp => prp.leasestatus == 1 && prp.Archievestatus == 0 && prp.Landlord_Id==lid)
                .Join(
                    db.Tenants,
                    prp => prp.LeasedTo,
                    tnt => tnt.Tenant_Id,
                    (prp, tnt) => new { Properties = prp, Tenants = tnt })
                .GroupJoin(
                    db.Buildings,
                    prp1 => prp1.Properties.BuildingId,
                    b => b.Building_Id,
                    (prp1, b) => new { prp1.Properties, prp1.Tenants, b = b.FirstOrDefault() })
                .Select(c => new PropertyDataLease
                {
                    PropertyName = c.Properties.PropertyName,
                    Address1 = c.Properties.AddressLine1,
                    Address2 = c.Properties.AddressLine2,
                    Size = c.Properties.Size,
                    LeasedTo = c.Tenants.FirstName + " " + c.Tenants.LastName,
                    LeasedEnd = c.Properties.LeasedEnd,
                    BuildingName = c.b.BuildingName
                }).ToList();

            DataTable dt = FrontController.ToDataTable<PropertyDataLease>(lst);
            if (Command == "Email")
            {
                SendPDFEmail(dt, lnkto1, lnkcc1, lnkBcc1, subject, body, "Properties", "Properties");
            }
            if (Command == "Pdf")
            {
                ExportToPdf(dt, "LeasedProperty");
            }
            var lstvacant = db.Properties.Where(prp => prp.leasestatus == 0 && prp.Archievestatus == 0 && prp.Landlord_Id == lid)
               .GroupJoin(
                   db.PropertyStyles,
                   prp => prp.PropertyStyleId,
                   ps => ps.Propertystyle_Id,
                   (prp, ps) => new { Properties = prp, PropertyStyles = ps.FirstOrDefault() })
               .GroupJoin(
                   db.PropertyTypes,
                   prp1 => prp1.Properties.PropertyTypeId,
                   pt => pt.PropertyType_Id,
                   (prp1, pt) => new { prp1.Properties, prp1.PropertyStyles, PropertyTypes = pt.FirstOrDefault() })
                   .GroupJoin(db.Buildings,
                   prp2 => prp2.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp2, b) => new { prp2.Properties, prp2.PropertyStyles, prp2.PropertyTypes, b = b.FirstOrDefault() }
                   )
               .Select(c => new PropertyDataVacant
               {
                   PropertyName = c.Properties.PropertyName,
                   Address1 = c.Properties.AddressLine1,
                   Address2 = c.Properties.AddressLine2,
                   Size = c.Properties.Size,
                   PropertyType = c.PropertyTypes.PropertyTypeName,
                   PropertyStyle = c.PropertyStyles.Style_Name,
                   Bedroom = c.Properties.Bedroom,
                   Bathroom = c.Properties.Bathroom,
                   BuildingName = c.b.BuildingName
               }).ToList();
            DataTable dt111 = FrontController.ToDataTable<PropertyDataVacant>(lstvacant);
            var lstt = db.Properties.Where(p => p.leasestatus == 0 && p.Archievestatus == 0 && p.Landlord_Id == lid).ToList();

            var lst1 = db.Properties.Where(p => p.leasestatus == 1 && p.Archievestatus == 0 && p.Landlord_Id == lid).ToList();

            var arlist = db.Properties.Where(p => p.Archievestatus == 1 && p.Landlord_Id == lid).ToList();

            if (Command == "Email1")
            {
                SendPDFEmail(dt111, lnkto1, lnkcc1, lnkBcc1, subject, body, "Properties", "Properties");
            }
            if (Command == "Pdf1")
            {
                ExportToPdf(dt111, "VacantProperty");
            }
            if (Command == "StatusFilter")
            {

            }
            if (Command == "Sort")
            {
                ViewBag.tab = "tab-1";
                if (SearchBy.ToUpper() == "PROPERTYNAME")
                {
                    lst1 = lst1.OrderBy(p => p.PropertyName).ToList();
                }
                else if (SearchBy.ToUpper() == "ADDRESS1")
                {
                    lst1 = lst1.OrderBy(p => p.AddressLine1).ToList();
                }
                else if (SearchBy.ToUpper() == "ADDRESS2")
                {
                    lst1 = lst1.OrderBy(p => p.AddressLine2).ToList();
                }
                else if (SearchBy.ToUpper() == "CITY")
                {
                    lst1 = (lst1.OrderBy(p => p.City)).ToList();
                }
                else if (SearchBy.ToUpper() == "SIZE")
                {
                    lst1 = (lst1.OrderBy(p => p.Size)).ToList();
                }
                else if (SearchBy.ToUpper() == "BEDROOM")
                {
                    lst1 = (lst1.OrderBy(p => p.Bedroom)).ToList();
                }
                else if (SearchBy.ToUpper() == "BATHROOM")
                {
                    lst1 = (lst1.OrderBy(p => p.Bathroom)).ToList();
                }
            }
            else if (Command == "Search")
            {
                ViewBag.tab = "tab-1";
                if (searchdata != null)
                {
                    if (SearchBy.ToUpper() == "PROPERTYNAME")
                    {
                        lst1 = lst1.Where(p => p.PropertyName.ToUpper().StartsWith(searchdata.ToUpper())).ToList();
                    }
                    else if (SearchBy.ToUpper() == "ADDRESS1")
                    {
                        lst1 = lst1.Where(p => p.AddressLine1.ToUpper().StartsWith(searchdata.ToUpper())).ToList();
                    }
                    else if (SearchBy.ToUpper() == "ADDRESS2")
                    {
                        lst1 = lst1.Where(p => p.AddressLine2.ToUpper().StartsWith(searchdata.ToUpper())).ToList();
                    }
                    else if (SearchBy.ToUpper() == "CITY")
                    {
                        lst1 = lst1.Where(p => p.City.ToUpper().StartsWith(searchdata.ToUpper())).ToList();
                    }
                    else if (SearchBy.ToUpper() == "SIZE")
                    {
                        lst1 = lst1.Where(p => p.Size.ToUpper() == searchdata.ToUpper()).ToList();
                    }
                    else if (SearchBy.ToUpper() == "BEDROOM")
                    {
                        lst1 = lst1.Where(p => p.Bedroom == Convert.ToInt32(searchdata)).ToList();
                    }
                    else if (SearchBy.ToUpper() == "BATHROOM")
                    {
                        lst1 = lst1.Where(p => p.Bathroom == Convert.ToInt32(searchdata)).ToList();
                    }
                }
            }
            if (Command == "RemoveFilter")
            {
                lstt = db.Properties.ToList();
            }

            if (Command == "Sort1")
            {
                ViewBag.tab = "tab-2";
                if (SearchBy1.ToUpper() == "PROPERTYNAME")
                {
                    lstt = lstt.OrderBy(p => p.PropertyName).ToList();
                }
                //else if (SearchBy1.ToUpper() == "ADDRESS1")
                //{
                //    lstt = lstt.OrderBy(p => p.AddressLine1).ToList();
                //}
                //else if (SearchBy1.ToUpper() == "ADDRESS2")
                //{
                //    lstt = lstt.OrderBy(p => p.AddressLine2).ToList();
                //}
                else if (SearchBy1.ToUpper() == "CITY")
                {
                    lstt = (lstt.OrderBy(p => p.City)).ToList();
                }
                else if (SearchBy1.ToUpper() == "SIZE")
                {
                    lstt = (lstt.OrderBy(p => p.Size)).ToList();
                }
                else if (SearchBy1.ToUpper() == "BEDROOM")
                {
                    lstt = (lstt.OrderBy(p => p.Bedroom)).ToList();
                }
                else if (SearchBy1.ToUpper() == "BATHROOM")
                {
                    lstt = (lstt.OrderBy(p => p.Bathroom)).ToList();
                }
            }
            if (Command == "Search1")
            {
                ViewBag.tab = "tab-2";
                if (searchdata1 != null)
                {
                    if (SearchBy1.ToUpper() == "PROPERTYNAME")
                    {
                        lstt = lstt.Where(p => p.PropertyName.ToUpper().StartsWith(searchdata1.ToUpper())).ToList();
                    }
                    //else if (SearchBy1.ToUpper() == "ADDRESS1")
                    //{
                    //    lstt = lstt.Where(p => p.AddressLine1.ToUpper().StartsWith(searchdata1.ToUpper())).ToList();
                    //}
                    //else if (SearchBy1.ToUpper() == "ADDRESS2")
                    //{
                    //    lstt = lstt.Where(p => p.AddressLine2.ToUpper().StartsWith(searchdata1.ToUpper())).ToList();
                    //}
                    else if (SearchBy1.ToUpper() == "CITY")
                    {
                        lstt = lstt.Where(p => p.City.ToUpper().StartsWith(searchdata1.ToUpper())).ToList();
                    }
                    else if (SearchBy1.ToUpper() == "SIZE")
                    {
                        lstt = lstt.Where(p => p.Size.ToUpper() == searchdata1.ToUpper()).ToList();
                    }
                    else if (SearchBy1.ToUpper() == "BEDROOM")
                    {
                        lstt = lstt.Where(p => p.Bedroom == Convert.ToInt32(searchdata1)).ToList();
                    }
                    else if (SearchBy1.ToUpper() == "BATHROOM")
                    {
                        lstt = lstt.Where(p => p.Bathroom == Convert.ToInt32(searchdata1)).ToList();
                    }
                }
            }
            else if (Command == "RemoveFilter1")
            {
                lstt = db.Properties.ToList();
            }

            Session["lst"] = lstt.OrderByDescending(p => p.Property_Id);

            Session["lst1"] = lst1.OrderByDescending(p => p.Property_Id);

            Session["arlist"] = arlist.OrderByDescending(p => p.Property_Id);
            int pageSize = 10;
            if (pagesize1 != 0)
                pageSize = pagesize1;
            int pageNumber = (page ?? 1);
            page1 = page;
            ViewBag.CurrentPage = page1;
            if (tab == "tab-1" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["lst1"] = lst1.OrderByDescending(p => p.Property_Id).ToPagedList(pageNumber, pageSize);
            }
            if (tab == "tab-2" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["lst"] = lstt.OrderByDescending(p => p.Property_Id).ToPagedList(pageNumber, pageSize);
            }

            return View();
        }

        public ActionResult _VacantPropertyPartial()
        {
            return View("_VacantPropertyPartial");
        }
        public ActionResult _LeasedPropertyPartial()
        {
            return View("_LeasedPropertyPartial");
        }

        [SessionExpire]
        public ActionResult property_details(long? id)
        {
            ifprpdetail = true;
            if (id == null)
            {
                id = Convert.ToInt64(Session["propertyid"]);
            }
            Session["propertyid"] = id;
            var data = propertydetailsData(id);
            return View(data);
        }

        [SessionExpire]
        private Property propertydetailsData(long? id)
        {
            var data = db.Properties.Where(t => t.Property_Id == id).FirstOrDefault();
            var prpimages = db.PropertyImages.Where(p => p.PropertyId == id).ToList();
            ViewBag.prpimages = prpimages;
            ViewBag.workorders = db.Faults.Where(p => p.PropertyId == id).ToList();
            ViewBag.inventory = db.tbl_inventory.Where(p => p.PropertyId == id).ToList();
            return data;
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult property_details(long? id, IEnumerable<HttpPostedFileBase> ImageName)
        {
            ifprpdetail = true;
            foreach (var images in ImageName)
            {
                if (images != null)
                {
                    if (images.ContentLength > 0)
                    {
                        images.SaveAs(Server.MapPath("~/Folders/PropertyImages/" + images.FileName));
                        PropertyImage prp = new PropertyImage();
                        prp.LandlordId = Convert.ToInt64(Session["LandlordId"]);
                        prp.PropertyId = id;
                        prp.ImageName = images.FileName;
                        db.PropertyImages.Add(prp);
                        db.SaveChanges();
                        
                    }
                }
                else
                {
                    TempData["ImgMsg"] = "Select At Least On Images First";
                    var data = propertydetailsData(id);
                    return View(data);
                }
            }
            if (id == null)
            {
                id = Convert.ToInt32(Session["propertyid"]);
            }
            Session["propertyid"] = id;
            var data1 = propertydetailsData(id);
            return View(data1);
        }

        [SessionExpire]
        public ActionResult inspectionReport(int? id)
        {
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult inspectionReport(string Command, int? id, string checkstatus, Inspection inspection, string[] roomname, string[] rnotes, string[] roomimages)
        {
            var prp = db.Properties.Where(p => p.Property_Id == id).FirstOrDefault();
            if (checkstatus == "1")
            {
                for (int j = 0; j < roomname.Length; j++)
                {
                    Inspection insp = new Inspection();
                    insp.PropertyId = prp.Property_Id;
                    insp.LandlordId = prp.Landlord_Id;
                    insp.TenantId = prp.LeasedTo;
                    insp.RoomType = roomname[j].ToString();
                    insp.RoomNotes = rnotes[j].ToString();
                    // insp.Images = roomimages[j].ToString();
                    insp.InspectionNotes = inspection.InspectionNotes;
                    insp.ActionRequired = inspection.ActionRequired;
                    insp.CreatedDate = DateTime.UtcNow.Date;
                    insp.InspectionStatus = 0;
                    db.Inspections.Add(insp);
                    db.SaveChanges();
                }
            }
            else
                ViewBag.ErrorMessageIn = "Add Rooms Details";
            //  return Json(new { success = true });
            return RedirectToAction("PropertyIndex");
        }

        [SessionExpire]
        public ActionResult propertyDelete(long? id)
        {
            try
            {
                Property Applicant = db.Properties.Find(id);
                db.Properties.Remove(Applicant);
                db.SaveChanges();
                return RedirectToAction("propertyindex");

            }

            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                          validationErrors.Entry.Entity.ToString(),
                       validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            catch
            {
                return RedirectToAction("propertyindex");
            }
        }

        [SessionExpire]
        public ActionResult MyAccount()
        {
            long id = 0;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt64(Session["LandlordId"]);
            }
            Session["Landlord"] = id;
            var data = db.Users.Where(t => t.UserID == id).FirstOrDefault();
            return View(data);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult MyAccount(long? id, User user, string Command, HttpPostedFileBase Signature, HttpPostedFileBase ImageName)
        {
            User usr = new User();
            var usar = db.Users.Where(a => a.UserID == user.UserID).FirstOrDefault();
            string ProfileImage = "";
            string ProfileSignature = "";

            if (ImageName != null)
            {
                string path = Server.MapPath("~/Folders/ProfileImages/");

                if (ImageName.ContentLength > 0)
                {
                    ImageName.SaveAs(path + ImageName.FileName);
                }
                ProfileImage = ImageName.FileName;

            }
            if (Signature != null)
            {
                string path = Server.MapPath("~/Folders/ProfileImages/");

                if (Signature.ContentLength > 0)
                {
                    Signature.SaveAs(path + Signature.FileName);
                }
                ProfileSignature = Signature.FileName;
            }

            long? lid = 0;
            if (Session["LandlordId"] != null)
                lid = Convert.ToInt64(Session["LandlordId"]);
            if (usr == null)
            {
                usr.FirstName = user.FirstName;
                usr.LastName = user.LastName;
                usr.Phone = user.Phone;
                usr.Mobile = user.Mobile;
                usr.Email = user.Email;
                usr.Address1 = user.Address1;
                usr.BuildingName = user.BuildingName;
                usr.UnitNumber = user.UnitNumber;
                usr.Suberb = user.Suberb;
                usr.Province = user.Province;
                usr.StreetAddress = user.StreetAddress;
                usr.Town = user.Town;
                usr.City = user.City;
                usr.Country = user.Country;
                usr.County = user.County;
                usr.Zip = user.Zip;
                usr.State = user.State;
                usr.PostCode = user.PostCode;
                usr.ProfilePhoto = ProfileImage;
                usr.Signature = ProfileSignature;
                db.Users.Add(usr);
                db.SaveChanges();
                return RedirectToAction("MyAccount");
            }
            else
            {
                if (ProfileImage != null && ProfileImage != "")
                    usar.ProfilePhoto = ProfileImage;
                else
                    usar.ProfilePhoto = usar.ProfilePhoto;
                if (ProfileSignature != null && ProfileSignature != "")
                    usar.Signature = ProfileSignature;
                else
                    usar.Signature = usar.Signature;
                usar.FirstName = user.FirstName;
                usar.LastName = user.LastName;
                usar.Phone = user.Phone;
                usar.Mobile = user.Mobile;
                usar.Email = user.Email;
                usar.Address1 = user.Address1;
                usar.BuildingName = user.BuildingName;
                usar.UnitNumber = user.UnitNumber;
                usar.Suberb = user.Suberb;
                usar.Province = user.Province;
                usar.StreetAddress = user.StreetAddress;
                usar.Town = user.Town;
                usar.City = user.City;
                usar.Country = usar.Country;
                usar.County = user.County;
                usar.Zip = user.Zip;
                usar.State = user.State;
                usar.PostCode = user.PostCode;
                db.SaveChanges();
                //   SendMail(tenant.FirstName, tenant.LastName, tenant.Email, tenant.Password, tenant.Cellular, 2);
                return RedirectToAction("MyAccount");
            }
        }

        [SessionExpire]
        public ActionResult tenant_index(int? page, string tab, string Command, string searchdata, string SearchBy)
        {
            GetLeaseFilters();
            Session["Check"] = "tenant";
            GetSearchBy_Tenant();
            long? lid = 0;
            if (Session["LandlordId"] != null)
                lid = Convert.ToInt64(Session["LandlordId"]);
            var lst = db.Tenants.Where(p => p.leasestatus == 0 && p.Landlord_id ==lid).ToList();                         //

            var lstclosed = db.Tenants.Where(p => p.leasestatus == 2 && p.Landlord_id==lid).ToList();

            DateTime dt = DateTime.Now.Date;
            var lst1 = db.Tenants.Where(p => p.leasestatus == 1 && p.LeaseEnds >= dt && p.Landlord_id==lid).ToList();

            var ExpiredList = db.Tenants.Where(p => p.leasestatus == 1 && p.LeaseEnds < dt && p.Landlord_id==lid).ToList();


            if (Command == "Sort")
            {
                if (SearchBy.ToUpper() == "LASTNAME")
                    lst1 = lst1.OrderBy(p => p.LastName).ToList();
                else if (SearchBy.ToUpper() == "FIRSTNAME")
                    lst1 = lst1.OrderBy(p => p.FirstName).ToList();
            }
            else if (Command == "Search")
            {
                if (searchdata != null)
                {
                    if (SearchBy.ToUpper() == "LASTNAME")
                        lst1 = lst1.Where(p => p.LastName.ToUpper() == searchdata.ToUpper()).ToList();
                    else if (SearchBy.ToUpper() == "FIRSTNAME")
                        lst1 = lst1.Where(p => p.FirstName.ToUpper() == searchdata.ToUpper()).ToList();
                }
            }
            else if (Command == "RemoveFilter" || Command == "Refresh")
            {
                lst1 = lst1.Where(p => p.leasestatus == 1 && p.LeaseEnds >= dt).ToList();
            }

            ViewBag.searchdata = searchdata;
            int pageSize = 10;
            if (pagesize1 != 0)
                pageSize = pagesize1;
            int pageNumber = (page ?? 1);
            page1 = page;
            ViewBag.CurrentPage = page1;
            if (tab == "tab-1" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["tenantlst1"] = lst1.OrderByDescending(p => p.Tenant_Id).ToPagedList(pageNumber, pageSize);
            }
            if (tab == "tab-2" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["ExpiredList"] = ExpiredList.OrderByDescending(p => p.Tenant_Id).ToPagedList(pageNumber, pageSize);
            }
            if (tab == "tab-3" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["tenantlst"] = lst.OrderByDescending(p => p.Tenant_Id).ToPagedList(pageNumber, pageSize);
            }
            if (tab == "tab-4" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["closedlst"] = lstclosed.OrderByDescending(p => p.Tenant_Id).ToPagedList(pageNumber, pageSize);
            }
            ViewBag.tab = tab;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        [SessionExpire]
        public ActionResult tenant_index(int? page, string tab, string Command, string SearchBy3, string searchdata3, string SearchBy2, string searchdata2, string SearchBy1, string searchdata1, string searchdata, string SearchBy, string LeaseFilter, string lnkto1, string lnkcc1, string lnkBcc1, string subject, string body)
        {
            GetLeaseFilters();
            ViewBag.searchdata = searchdata;
            ViewBag.tab = tab;
            Session["Check"] = "tenant";
            GetSearchBy_Tenant();
            long? lid = 0;
            if (Session["LandlordId"] != null)
            lid = Convert.ToInt64(Session["LandlordId"]);
            var lst = db.Tenants.Where(p => p.leasestatus == 0 && p.Landlord_id==lid).ToList();
            var lstclosed = db.Tenants.Where(p => p.leasestatus == 2 && p.Landlord_id==lid).ToList();
            DateTime dt = DateTime.Now.Date;
            var lst1 = db.Tenants.Where(p => p.leasestatus == 1 && p.LeaseEnds >= dt && p.Landlord_id==lid).ToList();
            var ExpiredList = db.Tenants.Where(p => p.leasestatus == 1 && p.LeaseEnds < dt && p.Landlord_id==lid).ToList();

            if (Command == "Sort1")
            {
                ViewBag.tab = "tab-3";
                if (SearchBy1.ToUpper() == "LASTNAME")
                    lst = lst.OrderBy(p => p.LastName).ToList();
                else if (SearchBy1.ToUpper() == "FIRSTNAME")
                    lst = lst.OrderBy(p => p.FirstName).ToList();
            }
            else if (Command == "Search1")
            {
                ViewBag.tab = "tab-3";
                if (searchdata1 != null)
                {
                    if (SearchBy1.ToUpper() == "LASTNAME")
                        lst = lst.Where(p => p.LastName.ToUpper() == searchdata1.ToUpper()).ToList();
                    else if (SearchBy1.ToUpper() == "FIRSTNAME")
                        lst = lst.Where(p => p.FirstName.ToUpper() == searchdata1.ToUpper()).ToList();
                    ViewBag.searchdata = searchdata1;
                }
            }

            if (Command == "Sort")
            {
                ViewBag.tab = "tab-1";
                if (SearchBy.ToUpper() == "LASTNAME")
                    lst1 = lst1.OrderBy(p => p.LastName).ToList();
                else if (SearchBy.ToUpper() == "FIRSTNAME")
                    lst1 = lst1.OrderBy(p => p.FirstName).ToList();
            }
            else if (Command == "Search")
            {
                ViewBag.tab = "tab-1";
                if (searchdata != null)
                {
                    if (SearchBy.ToUpper() == "LASTNAME")
                        lst1 = lst1.Where(p => p.LastName.ToUpper() == searchdata.ToUpper()).ToList();
                    else if (SearchBy.ToUpper() == "FIRSTNAME")
                        lst1 = lst1.Where(p => p.FirstName.ToUpper() == searchdata.ToUpper()).ToList();
                }
            }
            if (Command == "Sort2")
            {
                ViewBag.tab = "tab-2";
                if (SearchBy2.ToUpper() == "LASTNAME")
                    ExpiredList = ExpiredList.OrderBy(p => p.LastName).ToList();
                else if (SearchBy2.ToUpper() == "FIRSTNAME")
                    ExpiredList = ExpiredList.OrderBy(p => p.FirstName).ToList();

            }
            else if (Command == "Search2")
            {
                ViewBag.tab = "tab-2";
                if (searchdata2 != null)
                {
                    if (SearchBy2.ToUpper() == "LASTNAME")
                        ExpiredList = ExpiredList.Where(p => p.LastName.ToUpper() == searchdata2.ToUpper()).ToList();
                    else if (SearchBy2.ToUpper() == "FIRSTNAME")
                        ExpiredList = ExpiredList.Where(p => p.FirstName.ToUpper() == searchdata2.ToUpper()).ToList();
                    ViewBag.searchdata = searchdata2;
                }
            }
            if (Command == "Sort3")
            {
                ViewBag.tab = "tab-4";
                if (SearchBy3.ToUpper() == "LASTNAME")
                    lstclosed = lstclosed.OrderBy(p => p.LastName).ToList();
                else if (SearchBy3.ToUpper() == "FIRSTNAME")
                    lstclosed = lstclosed.OrderBy(p => p.FirstName).ToList();
            }
            else if (Command == "Search3")
            {
                ViewBag.tab = "tab-4";
                if (searchdata3 != null)
                {
                    if (SearchBy3.ToUpper() == "LASTNAME")
                        lstclosed = lstclosed.Where(p => p.LastName.ToUpper() == searchdata3.ToUpper()).ToList();
                    else if (SearchBy3.ToUpper() == "FIRSTNAME")
                        lstclosed = lstclosed.Where(p => p.FirstName.ToUpper() == searchdata3.ToUpper()).ToList();
                    ViewBag.searchdata = searchdata3;
                }
            }
            if (LeaseFilter == "1")
            {
                DateTime dtseven = DateTime.UtcNow.Date.AddDays(7);
                lst1 = lst1.Where(p => p.LeaseEnds == dtseven).ToList();
            }
            else if (LeaseFilter == "2")
            {
                DateTime dtfourteen = DateTime.UtcNow.Date.AddDays(14);
                lst1 = lst1.Where(p => p.LeaseEnds == dtfourteen).ToList();
            }
            else if (LeaseFilter == "3")
            {
                DateTime dttwentyone = DateTime.UtcNow.Date.AddDays(30);
                lst1 = lst1.Where(p => p.LeaseEnds == dttwentyone).ToList();
            }
            else if (LeaseFilter == "4")
            {
                DateTime dtthirty = DateTime.UtcNow.Date.AddDays(60);
                lst1 = lst1.Where(p => p.LeaseEnds == dtthirty).ToList();
            }
            else if (LeaseFilter == "5")
            {
                DateTime dtthirty = DateTime.UtcNow.Date.AddDays(120);
                lst1 = lst1.Where(p => p.LeaseEnds == dtthirty).ToList();
            }
            if (Command == "RemoveFilter1" || Command == "Refresh1")
            {
                lst1 = lst1.Where(p => p.leasestatus == 1 && p.LeaseEnds >= dt).ToList();
            }
            var lst11 = db.Properties.
               Join(db.Tenants.Where(p => p.leasestatus == 1 && p.LeaseEnds >= dt && p.Landlord_id==lid), u => u.Property_Id, uir => uir.Property_Id,
               (u, uir) => new { u, uir })
               .Select(m => new TenantEmailData
               {
                   Name = m.uir.FirstName + " " + m.uir.LastName,
                   Address1 = m.uir.Address1,
                   Address2 = m.uir.Address2,
                   PropertyName = m.u.PropertyName,
                   Telephone = m.uir.Telephone,
                   Cellular = m.uir.Cellular,
                   Balance = m.uir.Balance,
                   LastPmtOn = m.uir.LastPmtOn,
                   Nextpmtdue = m.uir.Nextpmtdue,
                   LeaseEnds = m.uir.LeaseEnds,
                   leasestatus = m.uir.leasestatus
               }).ToList();

            DataTable dt1 = FrontController.ToDataTable<TenantEmailData>(lst11);
            if (Command == "Print")
            {
                ExportToPdf(dt1, "TenantsWithActiveLease");
                return RedirectToAction("tenant_index");
            }
            if (Command == "Email")
            {
                SendEmails.SendPDFEmail(dt1, lnkto1, lnkcc1, lnkBcc1, subject, body, "TenantsWithActiveLease", "TenantsWithActiveLease");
                return RedirectToAction("tenant_index");
            }
            var lst12 = db.Tenants.Where(p => p.leasestatus == 1 && p.LeaseEnds < dt && p.Landlord_id==lid)
                 .Select(m => new TenantEmailDataOthers
                 {
                     Name = m.FirstName + " " + m.LastName,
                     Address1 = m.Address1,
                     Address2 = m.Address2,
                     Telephone = m.Telephone,
                     Cellular = m.Cellular,
                     Balance = m.Balance,
                     LastPmtOn = m.LastPmtOn,
                     Nextpmtdue = m.Nextpmtdue
                 }).ToList();

            DataTable dt2 = FrontController.ToDataTable<TenantEmailDataOthers>(lst12);
            if (Command == "Pdf2")
            {
                ExportToPdf(dt2, "TenantsWithExpiredLease");
                return RedirectToAction("tenant_index");
            }
            if (Command == "Email1")
            {
                SendEmails.SendPDFEmail(dt2, lnkto1, lnkcc1, lnkBcc1, subject, body, "TenantsWithExpiredLease", "TenantsWithExpiredLease");
                return RedirectToAction("tenant_index");
            }
            var lst13 = db.Tenants.Where(p => p.leasestatus == 0 && p.Landlord_id==lid)
            .Select(m => new TenantEmailDataOthers
            {
                Name = m.FirstName + " " + m.LastName,
                Address1 = m.Address1,
                Address2 = m.Address2,
                Telephone = m.Telephone,
                Cellular = m.Cellular,
                Balance = m.Balance,
                LastPmtOn = m.LastPmtOn,
                Nextpmtdue = m.Nextpmtdue
            }).ToList();

            DataTable dt3 = FrontController.ToDataTable<TenantEmailDataOthers>(lst13);
            if (Command == "Pdf1")
            {
                ExportToPdf(dt3, "TenantsWithoutLease");
                return RedirectToAction("tenant_index");
            }
            if (Command == "Email2")
            {
                SendEmails.SendPDFEmail(dt3, lnkto1, lnkcc1, lnkBcc1, subject, body, "TenantsWithoutLease", "TenantsWithoutLease");
                return RedirectToAction("tenant_index");
            }
            var lst14 = db.Tenants.Where(p => p.leasestatus == 2 && p.Landlord_id==lid)
                .Select(m => new TenantEmailDataOthers
                {
                    Name = m.FirstName + " " + m.LastName,
                    Address1 = m.Address1,
                    Address2 = m.Address2,
                    Telephone = m.Telephone,
                    Cellular = m.Cellular,
                    Balance = m.Balance,
                    LastPmtOn = m.LastPmtOn,
                    Nextpmtdue = m.Nextpmtdue
                }).ToList();

            DataTable dt4 = FrontController.ToDataTable<TenantEmailDataOthers>(lst14);
            if (Command == "Pdf3")
            {
                ExportToPdf(dt4, "TenantsWithFormerLease");
                return RedirectToAction("tenant_index");
            }
            if (Command == "Email3")
            {
                SendEmails.SendPDFEmail(dt4, lnkto1, lnkcc1, lnkBcc1, subject, body, "TenantsWithFormerLease", "TenantsWithFormerLease");
                return RedirectToAction("tenant_index");
            }
            Session["tenantlst1"] = lst1;

            int pageSize = 10;
            if (pagesize1 != 0)
                pageSize = pagesize1;
            int pageNumber = (page ?? 1);
            page1 = page;
            ViewBag.CurrentPage = page1;
            if (tab == "tab-1" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["tenantlst1"] = lst1.OrderByDescending(p => p.Tenant_Id).ToPagedList(pageNumber, pageSize);
            }
            if (tab == "tab-2" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["ExpiredList"] = ExpiredList.OrderByDescending(p => p.Tenant_Id).ToPagedList(pageNumber, pageSize);
            }
            if (tab == "tab-3" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["tenantlst"] = lst.OrderByDescending(p => p.Tenant_Id).ToPagedList(pageNumber, pageSize);
            }
            if (tab == "tab-4" || tab == null)
            {
                pageNumber = page.HasValue ? Convert.ToInt32(page) : 1;
                Session["closedlst"] = lstclosed.OrderByDescending(p => p.Tenant_Id).ToPagedList(pageNumber, pageSize);
            }

            return View();
        }

        private void GetSearchBy_Tenant()
        {
            var selectedvalue = Request.Form["SearchBy"];
            var SearchBy = new List<SelectListItem>
            {
                   new SelectListItem{ Text="Last Name", Value = "LastName" },
                new SelectListItem{ Text="First Name", Value = "FirstName" }
            };
            ViewBag.SearchBy = new SelectList(SearchBy.ToList(), "Value", "Text", selectedvalue);
        }
        //Tenant Create
        [SessionExpire]
        public ActionResult CreateTenant(int? id)
        {
            long? lid = Convert.ToInt64(Session["LandlordId"]);
            ViewBag.Country = db.Users.Where(p => p.UserID == lid).Select(p => p.Country).FirstOrDefault();
            var tenant = db.Tenants.Where(a => a.Tenant_Id == id).FirstOrDefault();
            return View(tenant);
        }

        [HttpPost]
        public ActionResult CreateTenant(Tenant tnt, HttpPostedFileBase ImageName, int? Tenant_Id, string gender, int? maritalstatus, short? pets, short? children)
        {
            long? lid = Convert.ToInt64(Session["LandlordId"]);
            lid = 32;
            int? country = db.Users.Where(p => p.UserID == lid).Select(p => p.Country).FirstOrDefault();
            Tenant tenant = new Tenant();
            var tnant = db.Tenants.Where(a => a.Tenant_Id == Tenant_Id).FirstOrDefault();
            string PropertyImage = "";
            if (ImageName != null)
            {
                string path = Server.MapPath("~/Folders/ProfileImages/");
                if (ImageName.ContentLength > 0)
                {
                    ImageName.SaveAs(path + ImageName.FileName);
                }
                PropertyImage += ImageName.FileName;
            }
            if (Session["LandlordId"] != null)
                lid = Convert.ToInt64(Session["LandlordId"]);
            if (tnant == null)
            {
                tenant.FirstName = tnt.FirstName;
                tenant.MiddleName = tnt.MiddleName;
                tenant.LastName = tnt.LastName;
                tenant.Gender = gender;
                tenant.Dateofbirth = tnt.Dateofbirth;
                tenant.Telephone = tnt.Telephone;
                tenant.Cellular = tnt.Cellular;
                tenant.Email = tnt.Email;
                tenant.Address1 = tnt.Address1;
                tenant.BuildingName = tnt.BuildingName;
                tenant.UnitNumber = tnt.UnitNumber;
                tenant.Suberb = tnt.Suberb;
                tenant.Province = tnt.Province;
                tenant.StreetAddress = tnt.StreetAddress;
                tenant.Town = tnt.Town;
                tenant.City = tnt.City;
                tenant.Country = country;
                tenant.Zip = tnt.Zip;
                tenant.County = tnt.County;
                tenant.State = tnt.State;
                tenant.Telephone = tnt.Telephone;
                tenant.MaritalStatus = maritalstatus;
                tenant.PostCode = tnt.PostCode;
                tenant.IfVisa = tnt.IfVisa;
                tenant.Pets = pets;
                tenant.ifPets = tnt.ifPets;
                tenant.Children = children;
                tenant.IfChildren = tnt.IfChildren;
                tenant.DriverLicense = tnt.DriverLicense;
                tenant.PassportNo = tnt.PassportNo;
                tenant.Dateofbirth = tnt.Dateofbirth;
                tenant.SocialSecurity = tnt.SocialSecurity;
                tenant.Profile = tnt.Profile;
                tenant.ProfilePhoto = PropertyImage.TrimEnd(',');
                tenant.leasestatus = 0;
                tenant.Landlord_id = lid;
                db.Tenants.Add(tenant);
                db.SaveChanges();
                SendMail(tenant.FirstName, tenant.LastName, tenant.Email, tenant.Password, tenant.Cellular, 2);
                if (Request.UrlReferrer.ToString().Contains("ReturnUrl"))
                    return Redirect(Request.UrlReferrer.ToString().Split('=')[1]);

                else
                    return RedirectToAction("tenant_index");
            }
            else
            {
                if (ImageName != null)
                    tnant.ProfilePhoto = PropertyImage.TrimEnd(',');
                else
                    tnant.ProfilePhoto = tnant.ProfilePhoto;
                tnant.Password = tnant.Password;
                tnant.FirstName = tnt.FirstName;
                tnant.leasestatus = 0;
                tnant.MiddleName = tnt.MiddleName;
                tnant.Dateofbirth = tnt.Dateofbirth;
                tnant.LastName = tnt.LastName;
                tnant.Gender = gender;
                tnant.Telephone = tnt.Telephone;
                tnant.Cellular = tnt.Cellular;
                tnant.Email = tnant.Email;
                tnant.Address1 = tnt.Address1;
                tnant.BuildingName = tnt.BuildingName;
                tnant.UnitNumber = tnt.UnitNumber;
                tnant.Suberb = tnt.Suberb;
                tnant.Province = tnt.Province;
                tnant.StreetAddress = tnt.StreetAddress;
                tnant.Town = tnt.Town;
                tnant.City = tnt.City;
                tnant.Country = country;
                tnant.Zip = tnt.Zip;
                tnant.County = tnt.County;
                tnant.State = tnt.State;
                tnant.PostCode = tnt.PostCode;
                tnant.Telephone = tnt.Telephone;
                tnant.MaritalStatus = maritalstatus;
                tnant.IfVisa = tnt.IfVisa;
                tnant.Pets = pets;
                tnant.ifPets = tnt.ifPets;
                tnant.Children = children;
                tnant.IfChildren = tnt.IfChildren;
                tnant.DriverLicense = tnt.DriverLicense;
                tnant.PassportNo = tnt.PassportNo;
                tnant.Dateofbirth = tnt.Dateofbirth;
                tnant.SocialSecurity = tnt.SocialSecurity;
                tnant.Profile = tnt.Profile;
                db.SaveChanges();
                //   SendMail(tenant.FirstName, tenant.LastName, tenant.Email, tenant.Password, tenant.Cellular, 2);
                if (Request.UrlReferrer.ToString().Contains("ReturnUrl"))
                    return Redirect(Request.UrlReferrer.ToString().Split('=')[1]);

                else
                    return RedirectToAction("tenant_index");
            }
        }
        //Tenant Delete
        [SessionExpire]
        public ActionResult TenantDelete(int? id)
        {
            try
            {
                Tenant Tenant = db.Tenants.Find(id);
                db.Tenants.Remove(Tenant);
                db.SaveChanges();
                return RedirectToAction("tenant_index");
            }
            catch
            {
                return RedirectToAction("tenant_index");
            }
        }

        private void GetLeaseFilters()
        {
            var selectedvalue = Request.Form["LeaseFilter"];
            var LeaseFilter = new List<SelectListItem>
            {
                   new SelectListItem{ Text="All Leases", Value = "0" },
                new SelectListItem{ Text="Expiring next 7 days", Value = "1" },
                new SelectListItem{ Text="Expiring next 14 days", Value = "2" },
                new SelectListItem{ Text="Expiring next 30 days", Value = "3" },
                  new SelectListItem{ Text="Expiring next 60 days", Value = "4" },
                  new SelectListItem{ Text="Expiring next 120 days", Value = "5" }
            };
            ViewBag.LeaseFilter = new SelectList(LeaseFilter.ToList(), "Value", "Text", selectedvalue);
        }

        //Lease 


        public ActionResult Search()

        {
            ViewBag.PropertyStyle = new SelectList(db.PropertyStyles.ToList(), "Propertystyle_Id", "Style_Name");
            ViewBag.country = new SelectList(db.Countries.ToList(), "countryId", "countryName");
            
            return View();
        }

        [HttpPost]
        public ActionResult Search(string srchlocation, string PropertyFor, double? MinPrice, double? MaxPrice, int? PropertyStyle, int? bedroom ,int? country)
        {
            db.Configuration.ProxyCreationEnabled = false;
            ViewBag.PropertyStyle = new SelectList(db.PropertyStyles.ToList(), "Propertystyle_Id", "Style_Name");
            ViewBag.country = new SelectList(db.Countries.ToList(), "countryId", "countryName");
            Session["Country"] = country;
            var data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Bedroom == bedroom && p.Price >= MinPrice && p.Price <= MaxPrice && p.PropertyStyleId == PropertyStyle).ToList();
            //1
            if (MaxPrice == 0 && MinPrice == 0 && bedroom == 0 && PropertyStyle == null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Bedroom == bedroom && p.Price >= MinPrice && p.Price <= MaxPrice && p.PropertyStyleId == PropertyStyle).ToList();
                Session["propertydata"] = data;
            }
            //2
            else if (MaxPrice != 0 && MinPrice != 0 && bedroom != 0 && PropertyStyle != null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation)).ToList();
                Session["propertydata"] = data.Where(p => p.Bedroom == bedroom && p.Price >= MinPrice && p.Price <= MaxPrice && p.PropertyStyleId == PropertyStyle).ToList();
            }
            //3
            else if (MaxPrice != 0 && MinPrice != 0 && bedroom == 0 && PropertyStyle != null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Bedroom == bedroom).ToList();
                Session["propertydata"] = data.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice && p.PropertyStyleId == PropertyStyle).ToList();
            }
            //4
            else if (MaxPrice == 0 && MinPrice != 0 && bedroom == 0 && PropertyStyle != null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Bedroom == bedroom && p.Price <= MaxPrice).ToList();
                Session["propertydata"] = data.Where(p => p.Price >= MinPrice && p.PropertyStyleId == PropertyStyle).ToList();
            }
            //5
            else if (MaxPrice != 0 && MinPrice == 0 && bedroom == 0 && PropertyStyle != null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Bedroom == bedroom && p.Price >= MinPrice).ToList();
                Session["propertydata"] = data.Where(p => p.Price <= MaxPrice && p.PropertyStyleId == PropertyStyle).ToList();
            }
            //6
            else if (MaxPrice != 0 && MinPrice == 0 && bedroom != 0 && PropertyStyle != null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Price >= MinPrice).ToList();
                Session["propertydata"] = data.Where(p => p.Price <= MaxPrice && p.Bedroom == bedroom && p.PropertyStyleId == PropertyStyle).ToList();
            }
            //7
            else if (MaxPrice == 0 && MinPrice != 0 && bedroom != 0 && PropertyStyle != null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Price <= MaxPrice).ToList();
                Session["propertydata"] = data.Where(p => p.Price >= MinPrice && p.Bedroom == bedroom && p.PropertyStyleId == PropertyStyle).ToList();
            }
            //8
            else if (MaxPrice == 0 && MinPrice == 0 && bedroom != 0 && PropertyStyle != null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Price <= MaxPrice && p.Price >= MinPrice).ToList();
                Session["propertydata"] = data.Where(p => p.Bedroom == bedroom && p.PropertyStyleId == PropertyStyle).ToList();
            }
            //9
            else if (MaxPrice == 0 && MinPrice == 0 && bedroom != 0 && PropertyStyle == null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Price <= MaxPrice && p.Price >= MinPrice && p.PropertyStyleId == PropertyStyle).ToList();
                Session["propertydata"] = data.Where(p => p.Bedroom == bedroom).ToList();
            }
            //10 start
            else if (MaxPrice == 0 && MinPrice == 0 && bedroom == 0 && PropertyStyle != null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Price <= MaxPrice && p.Price >= MinPrice && p.Bedroom == bedroom).ToList();
                Session["propertydata"] = data.Where(p => p.PropertyStyleId == PropertyStyle).ToList();
            }
            //11
            else if (MaxPrice != 0 && MinPrice != 0 && bedroom == 0 && PropertyStyle == null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Bedroom == bedroom && p.PropertyStyleId == PropertyStyle).ToList();
                Session["propertydata"] = data.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice).ToList();
            }
            //12
            else if (MaxPrice != 0 && MinPrice != 0 && bedroom != 0 && PropertyStyle == null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.PropertyStyleId == PropertyStyle).ToList();
                Session["propertydata"] = data.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice && p.Bedroom == bedroom).ToList();
            }
            //13
            else if (MaxPrice == 0 && MinPrice == 0 && bedroom != 0 && PropertyStyle != null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Price <= MaxPrice && p.Price >= MinPrice).ToList();
                Session["propertydata"] = data.Where(p => p.Bedroom == bedroom && p.PropertyStyleId == PropertyStyle).ToList();
            }
            //14
            else if (MaxPrice != 0 && MinPrice == 0 && bedroom != 0 && PropertyStyle == null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Price >= MinPrice && p.PropertyStyleId == PropertyStyle).ToList();
                Session["propertydata"] = data.Where(p => p.Price <= MaxPrice && p.Bedroom == bedroom).ToList();
            }
            //15
            else if (MaxPrice == 0 && MinPrice != 0 && bedroom != 0 && PropertyStyle == null)
            {
                data = db.Properties.Where(p => p.City.Contains(srchlocation) || p.State.Contains(srchlocation) || p.Country.Contains(srchlocation) || p.PostCode.Contains(srchlocation) || p.BuildingName.Contains(srchlocation) || p.Suberb.Contains(srchlocation) || p.Province.Contains(srchlocation) || p.Zip.Contains(srchlocation) && p.Price <= MaxPrice && p.PropertyStyleId == PropertyStyle).ToList();
                Session["propertydata"] = data.Where(p => p.Price >= MinPrice && p.Bedroom == bedroom).ToList();
            }
            else { Session["propertydata"] = data; }

            var srrr = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            string srttt = srrr.Serialize(data.Select(x => new
            {
                Property_Id = x.Property_Id,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Street = x.Street,
                City = x.City,
                State = x.State,
                Zip = x.PostCode
            }));
            Session["SearchJson"] = srttt;
            return RedirectToAction("PropertyListing");
        }
        public ActionResult PropertyDetails(int? id)
        {
            var prp = GetPropertyDetails(id);
            return View(prp);
        }

        private Property GetPropertyDetails(int? id)
        {
            var prp = db.Properties.Where(p => p.Property_Id == id).FirstOrDefault();
            var prpImages = db.PropertyImages.Where(p => p.PropertyId == id).ToList();
            ViewBag.prpImages = prpImages;
            return prp;
        }

        public ActionResult BackToList()
        {
            return RedirectToAction("PropertyListing");
        }
        [SessionExpire]
        public ActionResult MyList()
        {
            return View();
        }
        [SessionExpire]
        public ActionResult MyListProperties()
        {
            SortMylist();
            GetPropertiesList();
            return View();
        }

        [SessionExpire]
        private void GetPropertiesList()
        {
            //Session["TenantId"] = 1021;
            long? id = Convert.ToInt32(Session["TenantId"]);
            var q = db.Mylists.Where(p => p.TenantId == id).ToList();
            List<Property> prplst = new List<Property>();

            foreach (var data in q)
            {
                Property prp = new Property();
                var pn = db.tb_PropertyNotes.Where(p => p.PropertyId == data.PropertyId).ToList();
                prp = new JavaScriptSerializer().Deserialize<Property>(data.ListDetails.ToString());
                string prpStyle_Name = db.PropertyStyles.Where(p => p.Propertystyle_Id == prp.PropertyStyleId).Select(p => p.Style_Name).FirstOrDefault();
                // prp.PropertyStyleId = prp;
                prp.tb_PropertyNotes = pn;
                prplst.Add(prp);
            }
            Session["ListData"] = prplst;
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult MyListProperties(string SortBy, string Command, long? propid, string UserNotes, long? editnotesval, string UserNotes1)
        {
            //Session["TenantId"] = 1021;
            long? id = Convert.ToInt32(Session["TenantId"]);
            GetPropertiesList();
            SortMylist();
            ViewBag.MyListSortBy = SortBy;
            if (Command == "SaveNotes")
            {
                tb_PropertyNotes notes = new tb_PropertyNotes();
                notes.Notes = UserNotes;
                notes.PropertyId = propid;
                notes.TenantId = id;
                notes.DateAdded = DateTime.Now;
                db.tb_PropertyNotes.Add(notes);
                db.SaveChanges();
                TempData["RemoveProperty"] = "Notes Added Successfully";
                return RedirectToAction("MyListProperties");
            }
            else if (Command == "EditNotes")
            {
                tb_PropertyNotes notes = db.tb_PropertyNotes.Where(p => p.PrpNotesId == editnotesval).FirstOrDefault();
                notes.Notes = UserNotes1;
                notes.DateAdded = DateTime.Now;
                db.SaveChanges();
                //  TempData["RemoveProperty"] = "Notes Updated Successfully";
            }



            return View();
        }
        [SessionExpire]
        public ActionResult RemoveProperty(int? id)
        {
            //Session["TenantId"] = 1021;
            if (Session["TenantId"] != null)
            {
                int? tid = Convert.ToInt32(Session["TenantId"]);
                var data = db.Mylists.Where(p => p.PropertyId == id && p.TenantId == tid).FirstOrDefault();
                db.Mylists.Remove(data);
                db.SaveChanges();
                return RedirectToAction("MyListProperties");
            }
            else
            {
                return View();
            }
        }

        private void SortMylist()
        {
            var slectd = Request.Form["SortBy"];
            var SortBy1 = new List<SelectListItem>
{
                  new SelectListItem{ Text="Select", Value = "0" },

     new SelectListItem{ Text="Highest Price", Value = "1" },
    new SelectListItem{ Text="Lowest Price", Value = "2" },

    new SelectListItem{ Text="Highest Sqft", Value = "3" },

    new SelectListItem{ Text="Last Added", Value = "4" }
};
            ViewBag.SortByList = new SelectList(SortBy1, "Value", "Text", slectd);
        }


        [SessionExpire]
        public ActionResult TenantHome()
        {
            return View();
        }

        private void Sortlist()
        {
            var slectd = Request.Form["SortBy"];
            var SortBy = new List<SelectListItem>
{
      new SelectListItem{ Text="Select", Value = "0" },
     new SelectListItem{ Text="Highest Price", Value = "1" },
    new SelectListItem{ Text="Lowest Price", Value = "2" },
    new SelectListItem{ Text="Highest Sqft", Value = "3" }
};
            ViewBag.SortByPropList = new SelectList(SortBy, "Value", "Text", slectd);
        }

        public ActionResult PropertyListing()
        {
            Sortlist();
            // Session["TenantId"] = 1021;
            long? id = Convert.ToInt32(Session["TenantId"]);
            // ViewBag.PropertyType = new SelectList(db.PropertyTypes.ToList(), "PropertyType_Id", "PropertyTypeName");
            ViewBag.PropertyStyle = new SelectList(db.PropertyStyles.ToList(), "PropertyStyle_Id", "Style_Name");
            // var cnt = db.Properties.ToList();
            if (id != 0)

                ViewBag.ExistingList = db.Mylists.Where(p => p.TenantId == id).ToList();
            else
                ViewBag.ExistingList = null;
            if (cmd == 1)
            {
                ViewBag.PropertyListMessage = "Property added to WishList Successfully";
                cmd = 0;
            }
            return View();
        }

        [HttpPost]
        public ActionResult PropertyListing(Mylist lst, int? pid, string Command, string SortBy)
        {
            //Session["TenantId"] = 1021;
            Sortlist();
            ViewBag.SortBy = SortBy;

            return View();
        }


        public ActionResult ReportBuilding()
        {
            List<Building> building = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                building = db.Buildings.Where(p => p.LandlordId == id).ToList();
                ViewBag.building = new SelectList(building.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName");
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\BuildingReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsBuilding", building);
            reportViewer.LocalReport.DataSources.Add(rdc);
            // reportViewer.LocalReport.Refresh();
            // reportViewer.LocalReport.DataSources.Add(new ReportDataSource("tbBuilding", ds.Tables[0]));
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult ReportBuilding(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort)
        {
            List<Building> building = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                building = db.Buildings.Where(p => p.LandlordId == id).ToList();
            }

            if (drpBuilding != null)
                building = building.Where(p => p.Building_Id == drpBuilding).ToList();
            //if (drpProperty != null)
            //    tenant = tenant.Where(p => p.PropertyId == drpProperty).ToList();
            //if (drpTenant != null)
            //    tenant = tenant.Where(p => p.TenantId == drpTenant).ToList();   
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\BuildingReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsBuilding", building);
            reportViewer.LocalReport.DataSources.Add(rdc);
            // reportViewer.LocalReport.Refresh();
            // reportViewer.LocalReport.DataSources.Add(new ReportDataSource("tbBuilding", ds.Tables[0]));
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportTenants(int? drpTenant)
        {
            long id = 0;
            List<Tenant> tenant = null;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
            }
            //id = 32;
            tenant = db.Tenants.Where(p => p.Landlord_id == id).ToList();
            ViewBag.tenants = new SelectList(db.Tenants.Where(p => p.Landlord_id == id).ToList(), "Tenant_Id", "FirstName");

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\TenantReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsTenants", tenant);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult ReportTenants(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort)
        {
            //int? drpTenant = data;
            long id = 0;
            List<Tenant> tenant = null;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
            }
            //id = 32;
            tenant = db.Tenants.Where(p => p.Landlord_id == id).ToList();
            ViewBag.tenants = new SelectList(db.Tenants.Where(p => p.Landlord_id == id).ToList(), "Tenant_Id", "FirstName");

            //if (drpBuilding != null)
            //    tenant = tenant.Where(p => p.BuildingId == drpBuilding).ToList();
            //if (drpProperty != null)
            //    tenant = tenant.Where(p => p.PropertyId == drpProperty).ToList();
            if (drpTenant != null)
                tenant = tenant.Where(p => p.Tenant_Id == drpTenant).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\TenantReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsTenants", tenant);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult tenant_details(int? id)
        {
            if (id == null)
            {
                id = Convert.ToInt32(Session["tenantid"]);
            }
            Session["tenantid"] = id;
            var data = db.Tenants.Where(t => t.Tenant_Id == id).FirstOrDefault();
            var leasedata = db.Properties.Where(t => t.LeasedTo == id).ToList();
            Session["leasedproperty"] = leasedata;
            var leaseddata = db.tbl_lease.Where(t => t.primaryTenant_Id == id).ToList();
            Session["leasetable"] = leaseddata;
            return View(data);
        }
        [SessionExpire]
        public ActionResult tenant_contactedit()
        {
            int id = Convert.ToInt32(Session["tenantid"]);
            var data = db.Tenants.Where(t => t.Tenant_Id == id).FirstOrDefault();
            ////ViewBag.firstname = data.FirstName;
            ////ViewBag.lastname = data.LastName;
            ////ViewBag.cellular = data.Cellular;
            ////ViewBag.telephone = data.Telephone;
            ////ViewBag.email = data.Email;
            return View(data);
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult tenant_contactedit(Tenant t1)
        {
            int id = Convert.ToInt32(Session["tenantid"]);
            var data = db.Tenants.Where(t => t.Tenant_Id == id).FirstOrDefault();
            data.FirstName = t1.FirstName;
            data.MiddleName = t1.MiddleName;
            data.LastName = t1.LastName;
            data.Telephone = t1.Telephone;
            data.Cellular = t1.Cellular;
            data.Email = t1.Email;
            data.Address1 = t1.Address1;
            data.Address2 = t1.Address2;
            data.City = t1.City;
            data.State = t1.State;
            data.Gender = t1.Gender;
            data.PostCode = t1.PostCode;
            db.SaveChanges();
            Session["tenantid"] = data.Tenant_Id;
            // return RedirectToAction("tenant_details");
            return Json(new { success = true });
        }

        [SessionExpire]
        public ActionResult tenant_infoedit()
        {
            int id = Convert.ToInt32(Session["tenantid"]);
            var data = db.Tenants.Where(t => t.Tenant_Id == id).FirstOrDefault();
            return View(data);
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult tenant_infoedit(Tenant t1, DateTime date, int? profile, bool? Smoker, string DriverLicense, string SocialSecurity)
        {
            int id = Convert.ToInt32(Session["tenantid"]);
            var data = db.Tenants.Where(t => t.Tenant_Id == id).FirstOrDefault();
            data.PassportNo = t1.PassportNo;
            data.DriverLicense = t1.DriverLicense;
            data.SocialSecurity = t1.SocialSecurity;
            data.Dateofbirth = date;
            data.Profile = profile;
            data.Smoker = Smoker;
            db.SaveChanges();
            return Json(new { success = true });
            //  return RedirectToAction("tenant_details");
        }

        [SessionExpire]
        public ActionResult tenant_otherInfotedit()
        {
            int id = Convert.ToInt32(Session["tenantid"]);
            var data = db.Tenants.Where(t => t.Tenant_Id == id).FirstOrDefault();
            return View(data);
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult tenant_otherInfotedit(long? Tenant_Id, string ifPets, int? MaritalStatus, string IfVisa, short? Pets, short? Children, string IfChildren)
        {
            //int id = Convert.ToInt32(Session["tenantid"]);
            var data = db.Tenants.Where(t => t.Tenant_Id == Tenant_Id).FirstOrDefault();
            data.MaritalStatus = MaritalStatus;
            data.IfVisa = IfVisa;
            data.Pets = Pets;
            if (Pets == 1)
                data.ifPets = ifPets;
            data.Children = Children;
            if (Children == 1)
                data.IfChildren = IfChildren;
            db.SaveChanges();
            return Json(new { success = true });
            //return RedirectToAction("tenant_details");
        }

        [SessionExpire]
        public ActionResult ReportTenantDetails()
        {
            ViewBag.tenants = new SelectList(db.Tenants.ToList(), "Tenant_Id", "FirstName");
            List<Tenant> tenant = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                tenant = db.Tenants.ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\TenantReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsTenants", tenant);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [HttpPost]
         public ActionResult ReportTenantDetails(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort)
        {
            ViewBag.tenants = new SelectList(db.Tenants.ToList(), "Tenant_Id", "FirstName");
            List<Tenant> tenant = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                tenant = db.Tenants.ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\TenantReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsTenants", tenant);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult TenantsWithLease()
        {

            List<TenantEmailDataOthers> tenant = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                // ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                //ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1).ToList(), "Property_Id", "PropertyName");
                ViewBag.tenants = new SelectList(db.Tenants.Where(p => p.Landlord_id == id && p.leasestatus == 1).ToList(), "Tenant_Id", "FirstName");
                DateTime dt = DateTime.Now.Date;
                tenant = db.Tenants.Where(p => p.leasestatus == 1 && p.LeaseEnds > dt && p.Landlord_id == id)
              .Join(
                  db.Properties,
                  tnt => tnt.Tenant_Id,
                  prp => prp.LeasedTo,
                  (tnt, prp) => new { Tenants = tnt, Properties = prp })
              .GroupJoin(
                  db.Buildings,
                  prp1 => prp1.Properties.BuildingId,
                  b => b.Building_Id,
                  (prp1, b) => new { prp1.Properties, prp1.Tenants, b = b.FirstOrDefault() }).Join(
                       db.tbl_lease,
                       leaseprp => leaseprp.Properties.Property_Id,
                       lease => lease.Property_Id,
                       (leaseprp, lease) => new { leaseprp.Properties, leaseprp.Tenants, leaseprp.b, lease = lease }
                       )
             .Select(c => new TenantEmailDataOthers
             {
                 Name = c.Tenants.FirstName + " " + c.Tenants.LastName,
                 Telephone = c.Tenants.Telephone,
                 Cellular = c.Tenants.Cellular,
                 Address1 = c.Tenants.Address1,
                 Address2 = c.Tenants.Address2,
                 PropertyName = c.Properties.PropertyName,
                 BuildingName = c.b.BuildingName,
                 Balance = c.Tenants.Balance,
                 LastPmtOn = c.Tenants.LastPmtOn,
                 Nextpmtdue = c.Tenants.Nextpmtdue,
                 BuildingId = c.b.Building_Id,
                 TenantId = c.Tenants.Tenant_Id,
                 PropertyId = c.Properties.Property_Id
             }).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\TenantsWithLeaseReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsTenantWithLease", tenant);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [HttpPost]
    
        public ActionResult TenantsWithLease(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort, DateTime? FromDate, DateTime? ToDate)
        {
            List<TenantEmailDataOthers> tenant = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                //ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1).ToList(), "Property_Id", "PropertyName");
                ViewBag.tenants = new SelectList(db.Tenants.Where(p => p.Landlord_id == id).ToList(), "Tenant_Id", "FirstName");
                DateTime? dt2 = DateTime.Now.Date;
                //   tenant = db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id)

                if (FromDate != null && ToDate != null)
                {
                    tenant = db.Tenants.Where(p => p.leasestatus == 1)
                  .Join(
                      db.Properties,
                      tnt => tnt.Tenant_Id,
                      prp => prp.LeasedTo,
                      (tnt, prp) => new { Tenants = tnt, Properties = prp })
                  .GroupJoin(
                      db.Buildings,
                      prp1 => prp1.Properties.BuildingId,
                      b => b.Building_Id,
                      (prp1, b) => new { prp1.Properties, prp1.Tenants, b = b.FirstOrDefault() }).Join(
                       db.tbl_lease.Where(p => EntityFunctions.TruncateTime(p.Startdate) >= FromDate && EntityFunctions.TruncateTime(p.Startdate) <= ToDate),
                       leaseprp => leaseprp.Properties.Property_Id,
                       lease => lease.Property_Id,
                       (leaseprp, lease) => new { leaseprp.Properties, leaseprp.Tenants, leaseprp.b, lease = lease }
                       )
                 .Select(c => new TenantEmailDataOthers
                 {
                     Name = c.Tenants.FirstName + " " + c.Tenants.LastName,
                     Telephone = c.Tenants.Telephone,
                     Cellular = c.Tenants.Cellular,
                     Address1 = c.Tenants.Address1,
                     Address2 = c.Tenants.Address2,
                     PropertyName = c.Properties.PropertyName,
                     BuildingName = c.b.BuildingName,
                     Balance = c.Tenants.Balance,
                     LastPmtOn = c.Tenants.LastPmtOn,
                     Nextpmtdue = c.Tenants.Nextpmtdue,
                     BuildingId = c.b.Building_Id,
                     TenantId = c.Tenants.Tenant_Id,
                     PropertyId = c.Properties.Property_Id
                 }).ToList();
                }
                else
                {
                    tenant = db.Tenants.Where(p => p.leasestatus == 1 && p.LeaseEnds > dt2 && p.Landlord_id == id)
            .Join(
                db.Properties,
                tnt => tnt.Tenant_Id,
                prp => prp.LeasedTo,
                (tnt, prp) => new { Tenants = tnt, Properties = prp })
            .GroupJoin(
                db.Buildings,
                prp1 => prp1.Properties.BuildingId,
                b => b.Building_Id,
                (prp1, b) => new { prp1.Properties, prp1.Tenants, b = b.FirstOrDefault() }).Join(
                     db.tbl_lease,
                     leaseprp => leaseprp.Properties.Property_Id,
                     lease => lease.Property_Id,
                     (leaseprp, lease) => new { leaseprp.Properties, leaseprp.Tenants, leaseprp.b, lease = lease }
                     )
           .Select(c => new TenantEmailDataOthers
           {
               Name = c.Tenants.FirstName + " " + c.Tenants.LastName,
               Telephone = c.Tenants.Telephone,
               Cellular = c.Tenants.Cellular,
               Address1 = c.Tenants.Address1,
               Address2 = c.Tenants.Address2,
               PropertyName = c.Properties.PropertyName,
               BuildingName = c.b.BuildingName,
               Balance = c.Tenants.Balance,
               LastPmtOn = c.Tenants.LastPmtOn,
               Nextpmtdue = c.Tenants.Nextpmtdue,
               BuildingId = c.b.Building_Id,
               TenantId = c.Tenants.Tenant_Id,
               PropertyId = c.Properties.Property_Id
           }).ToList();
                }
            }
            List<TenantEmailDataOthers> lstobj = new List<TenantEmailDataOthers>();
            if (db.Tenants.Where(p => p.leasestatus == 1).ToList().Count > 0)
            {
                if (drpBuilding != null)
                {
                    foreach (var q in tenant)
                    {
                        if (q.BuildingId != null)
                        {
                            if (q.BuildingId == drpBuilding)
                            {
                                TenantEmailDataOthers obj = new TenantEmailDataOthers();
                                obj = q;
                                lstobj.Add(obj);
                            }
                        }
                    }
                    if (drpProperty != null || drpTenant != null)
                    { }
                    else if (drpProperty == null && drpTenant == null)
                        tenant = lstobj.ToList();
                }
                if (drpProperty != null)
                    tenant = tenant.Where(p => p.PropertyId == drpProperty).ToList();
                if (drpTenant != null)
                    tenant = tenant.Where(p => p.TenantId == drpTenant).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\TenantsWithLeaseReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsTenantWithLease", tenant);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult AllReports()
        {

            return View();
        }
        [SessionExpire]
        public ActionResult ReportAllProperties()
        {
            var lstProperties = new List<AllProperties>();
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1).ToList(), "Property_Id", "PropertyName");
                //   int id = 19;
                lstProperties = db.Properties.Where(p => p.Landlord_Id == id)
               .Join(
                   db.Tenants,
                   prp => prp.LeasedTo,
                   tnt => tnt.Tenant_Id,
                   (prp, tnt) => new { Properties = prp, Tenants = tnt })
               .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Properties, prp1.Tenants, b = b.FirstOrDefault() })
              .Select(c => new AllProperties
              {
                  PropertyName = c.Properties.PropertyName,
                  Address = c.Properties.AddressLine1,
                  City = c.Properties.City,
                  State = c.Properties.State,
                  Zip = c.Properties.PostCode,
                  BuildingName = c.b.BuildingName,
                  LeasedTo = c.Tenants.FirstName + " " + c.Tenants.LastName
              }).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AllPropertiesReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsAllProperties", lstProperties);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult ReportAllProperties(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort, DateTime? FromDate, DateTime? ToDate)
        {
            var lstProperties = new List<AllProperties>();
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1).ToList(), "Property_Id", "PropertyName");
                if (FromDate != null && ToDate != null)
                {
                    lstProperties = db.Properties.Where(p => p.Landlord_Id == id)
                   .Join(
                       db.Tenants,
                       prp => prp.LeasedTo,
                       tnt => tnt.Tenant_Id,
                       (prp, tnt) => new { Properties = prp, Tenants = tnt })
                   .GroupJoin(
                       db.Buildings,
                       prp1 => prp1.Properties.BuildingId,
                       b => b.Building_Id,
                       (prp1, b) => new { prp1.Properties, prp1.Tenants, b = b.FirstOrDefault() }).Join(
                       db.tbl_lease.Where(p => EntityFunctions.TruncateTime(p.Startdate) >= FromDate && EntityFunctions.TruncateTime(p.Startdate) <= ToDate),
                       leaseprp => leaseprp.Properties.Property_Id,
                       lease => lease.Property_Id,
                       (leaseprp, lease) => new { leaseprp.Properties, leaseprp.Tenants, leaseprp.b, lease = lease }
                       )
                  .Select(c => new AllProperties
                  {
                      PropertyName = c.Properties.PropertyName,
                      Address = c.Properties.AddressLine1,
                      City = c.Properties.City,
                      State = c.Properties.State,
                      Zip = c.Properties.PostCode,
                      BuildingName = c.b.BuildingName,
                      LeasedTo = c.Tenants.FirstName + " " + c.Tenants.LastName,
                      BuildingId = c.b.Building_Id,
                      TenantId = c.Tenants.Tenant_Id,
                      PropertyId = c.Properties.Property_Id
                  }).ToList();
                }
                else
                {
                    lstProperties = db.Properties.Where(p => p.Landlord_Id == id).Join(
                    db.Tenants,
                    prp => prp.LeasedTo,
                    tnt => tnt.Tenant_Id,
                    (prp, tnt) => new { Properties = prp, Tenants = tnt })
                .GroupJoin(
                    db.Buildings,
                    prp1 => prp1.Properties.BuildingId,
                    b => b.Building_Id,
                    (prp1, b) => new { prp1.Properties, prp1.Tenants, b = b.FirstOrDefault() })
               .Select(c => new AllProperties
               {
                   PropertyName = c.Properties.PropertyName,
                   Address = c.Properties.AddressLine1,
                   City = c.Properties.City,
                   State = c.Properties.State,
                   Zip = c.Properties.PostCode,
                   BuildingName = c.b.BuildingName,
                   LeasedTo = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   PropertyId = c.Properties.Property_Id,
                   TenantId = c.Tenants.Tenant_Id,
                   BuildingId = c.b.Building_Id
               }).ToList();
                }
            }
            //if (drpBuilding != null)
            //    lstProperties = lstProperties.Where(p => p.BuildingId == drpBuilding).ToList();
            if (drpProperty != null)
                lstProperties = lstProperties.Where(p => p.PropertyId == drpProperty).ToList();
            if (drpTenant != null)
                lstProperties = lstProperties.Where(p => p.TenantId == drpTenant).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AllPropertiesReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsAllProperties", lstProperties);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportProperties()
        {
            var lstProperties = new List<PropertyDataVacant>();
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                // ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 0 && p.Archievestatus == 0).ToList(), "Property_Id", "PropertyName");
                lstProperties = db.Properties.Where(prp => prp.leasestatus == 0 && prp.Archievestatus == 0 && prp.Landlord_Id == id)
              .Join(
                  db.PropertyStyles,
                  prp => prp.PropertyStyleId,
                  ps => ps.Propertystyle_Id,
                  (prp, ps) => new { Properties = prp, PropertyStyles = ps })
                  .GroupJoin(db.Buildings,
                  prp2 => prp2.Properties.BuildingId,
                  b => b.Building_Id,
                  (prp2, b) => new { prp2.Properties, prp2.PropertyStyles, b = b.FirstOrDefault() }
                  )
              .Select(c => new PropertyDataVacant
              {
                  PropertyName = c.Properties.PropertyName,
                  Address1 = c.Properties.AddressLine1,
                  Address2 = c.Properties.AddressLine2,
                  Size = c.Properties.Size,
                  PropertyStyle = c.PropertyStyles.Style_Name,
                  Bedroom = c.Properties.Bedroom,
                  Bathroom = c.Properties.Bathroom,
                  BuildingName = c.b.BuildingName
              }).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\PropertyReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsProperties", lstProperties);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult ReportProperties(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort)
        {
            var lstProperties = new List<PropertyDataVacant>();
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 0).ToList(), "Property_Id", "PropertyName");
                lstProperties = db.Properties.Where(prp => prp.leasestatus == 0 && prp.Archievestatus == 0 && prp.Landlord_Id == id)
              .Join(
                  db.PropertyStyles,
                  prp => prp.PropertyStyleId,
                  ps => ps.Propertystyle_Id,
                  (prp, ps) => new { Properties = prp, PropertyStyles = ps })
                  .GroupJoin(db.Buildings,
                  prp2 => prp2.Properties.BuildingId,
                  b => b.Building_Id,
                  (prp2, b) => new { prp2.Properties, prp2.PropertyStyles, b = b.FirstOrDefault() }
                  )
              .Select(c => new PropertyDataVacant
              {
                  PropertyName = c.Properties.PropertyName,
                  Address1 = c.Properties.AddressLine1,
                  Address2 = c.Properties.AddressLine2,
                  Size = c.Properties.Size,
                  PropertyStyle = c.PropertyStyles.Style_Name,
                  Bedroom = c.Properties.Bedroom,
                  Bathroom = c.Properties.Bathroom,
                  BuildingName = c.b.BuildingName,
                  BuildingId = c.b.Building_Id,
                  // TenantId = c.Tenants.Tenant_Id,
                  PropertyId = c.Properties.Property_Id
              }).ToList();
            }
            //if (drpBuilding != null)
            //    lstProperties = lstProperties.Where(p => p.BuildingId == drpBuilding).ToList();
            if (drpProperty != null)
                lstProperties = lstProperties.Where(p => p.PropertyId == drpProperty).ToList();
            //if (drpTenant != null)
            //    lstProperties = lstProperties.Where(p => p.TenantId == drpTenant).ToList();   
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\PropertyReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsProperties", lstProperties);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]

        public ActionResult NewViewingPurposal(int? id)
        {
            long? id1 = db.RequestViews.Where(p => p.ReqviewId == id).Select(p => p.PropertyId).FirstOrDefault();
            var prp = db.Properties.Where(p => p.Property_Id == id1).FirstOrDefault();
            Session["propdataviewprop"] = prp;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult NewViewingPurposal(int? id, RequestView vp)
        {
            if (Session["LandlordId"] != null)
            {
                var rv = db.RequestViews.Where(p => p.ReqviewId == id && p.RequestStatus == 1).FirstOrDefault();
               // rv.Approve = 3;
                db.SaveChanges();
                RequestView rvs = new RequestView();
                rvs.FirstName = rv.FirstName;
                rvs.LastName = rv.LastName;
                rvs.Phone = rv.Phone;
                rvs.PreferredDate = vp.PreferredDate;
                rvs.PreferredTime = vp.PreferredTime;
                rvs.text = vp.text;
                rvs.LandlordId = rv.LandlordId;
                rvs.TenantId = rv.TenantId;
                rvs.Approve = 0;
                rvs.OfferId = 1;
                rvs.RequestStatus = 2;
                rvs.PropertyId = rv.PropertyId;
                //rvs.ReqviewId = rv.ReqviewId;
                rvs.RequestParentId = rv.ReqviewId;
                db.RequestViews.Add(rvs);
                db.SaveChanges();
                //var rd = db.ViewingPurposals.OrderByDescending(p => p.ViewingPurposalid).Take(1).FirstOrDefault();
                //TenancyDetail td = db.TenancyDetails.Where(p => p.TenantId == rv.TenantId && p.LandlordId == rv.LandlordId && p.PropertyId == rv.PropertyId && p.RequestViewingId == rv.ReqviewId).FirstOrDefault();
                //td.ProposalViewingId = rd.ViewingPurposalid;
                //db.SaveChanges();
                TempData["landlordviewingrequestMessage"] = "Proposal Sent Successfully";
                // return Json(new { success = true });
                return RedirectToAction("landlordviewingrequest");
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult EditViewingPurposal(int? id)
        {
            var vieww = db.ViewingPurposals.Where(p => p.ViewingPurposalid == id).FirstOrDefault();
            var prp = db.Properties.Where(p => p.Property_Id == vieww.PropertyId).FirstOrDefault();
            Session["propdataviewprop"] = prp;
            return View(vieww);
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult EditViewingPurposal(int? id, ViewingPurposal rv)
        {
            if (Session["LandlordId"] != null)
            {
                ViewingPurposal rvs = db.ViewingPurposals.Where(p => p.ViewingPurposalid == id).FirstOrDefault();
                rvs.FirstName = rvs.FirstName;
                rvs.LastName = rvs.LastName;
                rvs.Phone = rvs.Phone;
                rvs.PreferredDate = rv.PreferredDate;
                rvs.PreferredTime = rv.PreferredTime;
                rvs.text = rv.text;
                rvs.LandlordId = rvs.LandlordId;
                rvs.TenantId = rvs.TenantId;
                rvs.Approve = 0;
                rvs.PropertyId = rvs.PropertyId;
                rvs.RequestViewid = rvs.RequestViewid;
                db.SaveChanges();
                TempData["landlordviewingrequestMessage"] = "Viewing Request Updated Successfully";
                // return Json(new { success = true });
                return RedirectToAction("landlordviewingrequest");
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult AdminViewingProposal()
        {
            long? id = 0;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt64(Session["LandlordId"]);
                var data = db.ViewingPurposals.Where(p => p.LandlordId == id && p.Approve == 0).ToList();
                ViewBag.Pending = data;
                var data1 = db.ViewingPurposals.Where(p => p.LandlordId == id && p.Approve == 1).ToList();
                ViewBag.accepted = data1;
                var data2 = db.ViewingPurposals.Where(p => p.LandlordId == id && p.Approve == 2).ToList();
                ViewBag.rejected = data2;
                return View();
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        //public ActionResult acceptrequest(int id)
        //{
        //    var data = db.RequestViews.Where(p => p.ReqviewId == id && p.RequestStatus == 1).FirstOrDefault();
        //    data.Approve = 1;
        //    db.SaveChanges();//commented
        //    //string body = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Viewing Request Has been Aceepted for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
        //    //SendEmails.SendNotificationToTenant(data.Tenant.Email, "Viewing Request Accepted", body);
        //    //string body1 = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Accepted Viewing Request of " + data.Tenant.FirstName + " " + data.Tenant.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
        //    //SendEmails.SendNotificationToLandlord(data.User.Email, "Viewing Request Accepted", body1);
        //    return RedirectToAction("landlordviewingrequest");

        //}
        //[SessionExpire]
        //public ActionResult rejectrequest(int id)
        //{
        //    var data = db.RequestViews.Where(p => p.ReqviewId == id && p.RequestStatus == 1).FirstOrDefault();
        //    data.Approve = 2;
        //    db.SaveChanges();//Commnetd
        //    //string body = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Viewing Request Has been Rejected for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
        //    //SendEmails.SendNotificationToTenant(data.Tenant.Email, "Viewing Request Rejected", body);
        //    //string body1 = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Rejected Viewing Request of " + data.Tenant.FirstName + " " + data.Tenant.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
        //    //SendEmails.SendNotificationToLandlord(data.User.Email, "Viewing Request Rejected", body1);
        //    return RedirectToAction("landlordviewingrequest");
        //}
        //[SessionExpire]
        //public ActionResult landlordviewingrequest()
        //{
        //    long? id = 0;
            
        //    if (Session["LandlordId"] != null)
        //    {
        //        id = Convert.ToInt64(Session["LandlordId"]);
        //        var data = db.RequestViews.Where(p => p.LandlordId == id && p.Approve == 0 && p.RequestStatus ==1).OrderByDescending(p => p.ReqviewId).ToList();
        //        ViewBag.Pending = data;
        //        var data1 = db.RequestViews.Where(p => p.LandlordId == id && p.Approve == 1 && p.RequestStatus == 1).OrderByDescending(p => p.ReqviewId).ToList();
        //        ViewBag.accepted = data1;
        //        var data2 = db.RequestViews.Where(p => p.LandlordId == id && p.Approve == 2 && p.RequestStatus == 1).OrderByDescending(p => p.ReqviewId).ToList();
        //        ViewBag.rejected = data2;
        //        var data3 = db.RequestViews.Where(p => p.LandlordId == id && p.Approve == 3 && p.RequestStatus == 1).OrderByDescending(p => p.ReqviewId).ToList();
        //        ViewBag.purposal = data3;
                
        //        //var data3 = db.ViewingPurposals.Where(p => p.LandlordId == id && p.Approve == 0).ToList();
        //        //ViewBag.Pendingprop = data3;
        //        //var data4 = db.ViewingPurposals.Where(p => p.LandlordId == id && p.Approve == 1).ToList();
        //        //ViewBag.acceptedprop = data4;
        //        //var data5 = db.ViewingPurposals.Where(p => p.LandlordId == id && p.Approve == 2).ToList();
        //        //ViewBag.rejectedprop = data5;
        //        return View();
        //    }
        //    else
        //        return RedirectToAction("Login");
        //}
        //[SessionExpire]

        //public ActionResult offerrequest()
        //{
        //    long? id = 0;
        //    if (Session["LandlordId"] != null)
        //    {
        //        id = Convert.ToInt64(Session["LandlordId"]);
        //        var data = db.tbl_offer.Where(p => p.LandlordId == id && p.Approval == 0 ).OrderByDescending(p => p.OfferId).ToList();
        //        ViewBag.Pending = data;
        //        var data1 = db.tbl_offer.Where(p => p.LandlordId == id && p.Approval == 1).OrderByDescending(p => p.OfferId).ToList();
        //        ViewBag.accepted = data1;
        //        var data2 = db.tbl_offer.Where(p => p.LandlordId == id && p.Approval == 2).OrderByDescending(p => p.OfferId).ToList();
        //        ViewBag.Rejected = data2;
        //        //var data3 = db.tbl_offer.Where(p => p.LandlordId == id && p.PurposalStatus == 1 && p.Approval == 3).ToList();
        //        //ViewBag.purposal = data3;
        //        //var data4 = db.tbl_offer.Where(p => p.LandlordId == id && p.Approval == 4).ToList();
        //        //ViewBag.DepositReq = data4;
        //        //var data5 = db.Purposals.Where(p => p.LandlordId == id && p.Approval == 0).ToList();
        //        //ViewBag.PendingPurposal = data5;
        //        //var data6 = db.Purposals.Where(p => p.LandlordId == id && p.Approval == 1).ToList();
        //        //ViewBag.acceptedPurposal = data6;
        //        //var data7 = db.Purposals.Where(p => p.LandlordId == id && p.Approval == 2).ToList();
        //        //ViewBag.RejectedPurposal = data7;
        //        return View();
        //    }
        //    else
        //        return RedirectToAction("Login");
        //}
        //[SessionExpire]
       
        public ActionResult ReportPropertiesWithLease()
        {
            List<PropertyDataLease> property = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1).ToList(), "Property_Id", "PropertyName");
                property = db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id)
                     .Join(
                         db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                         prp => prp.LeasedTo,
                         tnt => tnt.Tenant_Id,
                         (prp, tnt) => new { Properties = prp, Tenants = tnt })
                    .GroupJoin(
                         db.Buildings,
                         prp1 => prp1.Properties.BuildingId,
                         b => b.Building_Id,
                         (prp1, b) => new { prp1.Properties, prp1.Tenants, b = b.FirstOrDefault() }).Join(
                         db.tbl_lease,
                         leaseprp => leaseprp.Properties.Property_Id,
                         lease => lease.Property_Id,
                         (leaseprp, lease) => new { leaseprp.Properties, leaseprp.Tenants, leaseprp.b, lease = lease }
                         )
                     .Select(c => new PropertyDataLease
                     {
                         PropertyName = c.Properties.PropertyName,
                         Address1 = c.Properties.AddressLine1,
                         Address2 = c.Properties.AddressLine2,
                         Size = c.Properties.Size,
                         LeasedTo = c.Tenants.FirstName + " " + c.Tenants.LastName,
                         LeasedEnd = c.Properties.LeasedEnd,
                         BuildingName = c.b.BuildingName,
                         BuildingId = c.b.Building_Id,
                         TenantId = c.Tenants.Tenant_Id,
                         PropertyId = c.Properties.Property_Id
                     }).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\PropertyReportWithLease.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsPropertyOnLease", property);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult ReportPropertiesWithLease(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort, DateTime? FromDate, DateTime? ToDate)
        {
            List<PropertyDataLease> property = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1).ToList(), "Property_Id", "PropertyName");
                //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                if (FromDate != null && ToDate != null)
                {
                    property = db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id)
                   .Join(
                       db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                       prp => prp.LeasedTo,
                       tnt => tnt.Tenant_Id,
                       (prp, tnt) => new { Properties = prp, Tenants = tnt })
                  .GroupJoin(
                       db.Buildings,
                       prp1 => prp1.Properties.BuildingId,
                       b => b.Building_Id,
                       (prp1, b) => new { prp1.Properties, prp1.Tenants, b = b.FirstOrDefault() }).Join(
                       db.tbl_lease.Where(p => EntityFunctions.TruncateTime(p.Startdate) >= FromDate && EntityFunctions.TruncateTime(p.Startdate) <= ToDate),
                       leaseprp => leaseprp.Properties.Property_Id,
                       lease => lease.Property_Id,
                       (leaseprp, lease) => new { leaseprp.Properties, leaseprp.Tenants, leaseprp.b, lease = lease }
                       )
                   .Select(c => new PropertyDataLease
                   {
                       PropertyName = c.Properties.PropertyName,
                       Address1 = c.Properties.AddressLine1,
                       Address2 = c.Properties.AddressLine2,
                       Size = c.Properties.Size,
                       LeasedTo = c.Tenants.FirstName + " " + c.Tenants.LastName,
                       LeasedEnd = c.Properties.LeasedEnd,
                       BuildingName = c.b.BuildingName,
                       BuildingId = c.b.Building_Id,
                       TenantId = c.Tenants.Tenant_Id,
                       PropertyId = c.Properties.Property_Id
                   }).ToList();
                }
                else
                {
                    property = db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id)
             .Join(
                 db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                 prp => prp.LeasedTo,
                 tnt => tnt.Tenant_Id,
                 (prp, tnt) => new { Properties = prp, Tenants = tnt })
            .GroupJoin(
                 db.Buildings,
                 prp1 => prp1.Properties.BuildingId,
                 b => b.Building_Id,
                 (prp1, b) => new { prp1.Properties, prp1.Tenants, b = b.FirstOrDefault() })
             .Select(c => new PropertyDataLease
             {
                 PropertyName = c.Properties.PropertyName,
                 Address1 = c.Properties.AddressLine1,
                 Address2 = c.Properties.AddressLine2,
                 Size = c.Properties.Size,
                 LeasedTo = c.Tenants.FirstName + " " + c.Tenants.LastName,
                 LeasedEnd = c.Properties.LeasedEnd,
                 BuildingName = c.b.BuildingName,
                 BuildingId = c.b.Building_Id,
                 TenantId = c.Tenants.Tenant_Id,
                 PropertyId = c.Properties.Property_Id
             }).ToList();
                }
            }
            //if (drpBuilding != null)
            //    property = property.Where(p => p.BuildingId == drpBuilding).ToList();
            if (drpProperty != null)
                property = property.Where(p => p.PropertyId == drpProperty).ToList();
            if (drpTenant != null)
                property = property.Where(p => p.TenantId == drpTenant).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\PropertyReportWithLease.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsPropertyOnLease", property);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportPaymentHistories(int? drpval)
        {
            List<paymentHistoryreport> PaymentHistory = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                //      PaymentHistory = db.paymentHistories.Where(p => p.Landlord_id == id).ToList();
                PaymentHistory = db.paymentHistories.Where(p => p.Landlord_id == id)
                     .Join(
                         db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                         paymnt => paymnt.Property_Id,
                         prp => prp.Property_Id,
                         (paymnt, prp) => new { Payments = paymnt, Properties = prp }).Join(
                         db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                         pmt => pmt.Payments.Tenant_Id,
                         tnt => tnt.Tenant_Id,
                         (pmt, tnt) => new { pmt.Payments, pmt.Properties, Tenants = tnt })
                    .GroupJoin(
                         db.Buildings,
                         prp1 => prp1.Properties.BuildingId,
                         b => b.Building_Id,
                         (prp1, b) => new { prp1.Payments, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() }).Select(c => new paymentHistoryreport
                         {
                             Property = c.Properties.PropertyName,
                             Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                             Building = c.b.BuildingName,
                             RecievedOn = c.Payments.RecievedOn,
                             Type = c.Payments.Type,
                             Amount = c.Payments.Amount,
                             DueDate = c.Payments.DueDate,
                             BuildingId = c.b.Building_Id,
                             TenantId = c.Tenants.Tenant_Id,
                             PropertyId = c.Properties.Property_Id
                         }).ToList();

                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
            }
            PaymentHistory = getdrpval(drpval, PaymentHistory);
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\PaymentHistoryReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsPaymentHistory", PaymentHistory);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        private static List<paymentHistoryreport> getdrpval(int? drpval, List<paymentHistoryreport> PaymentHistory)
        {
            if (drpval == 1)
                PaymentHistory = PaymentHistory.Where(p => p.RecievedOn.Value.Month == DateTime.Now.Month).ToList();
            else if (drpval == 2)
                PaymentHistory = PaymentHistory.Where(p => p.RecievedOn == DateTime.Now.AddDays(-7).Date && p.RecievedOn == DateTime.Now.Date).ToList();
            else if (drpval == 3)
                PaymentHistory = PaymentHistory.Where(p => p.RecievedOn.Value.Month == DateTime.Now.AddMonths(-1).Month).ToList();
            else if (drpval == 4)
                PaymentHistory = PaymentHistory.Where(p => p.RecievedOn == DateTime.Now.AddDays(-7).Date).ToList();
            else if (drpval == 5)
                PaymentHistory = PaymentHistory.Where(p => p.RecievedOn.Value.Day == DateTime.Now.Day && p.RecievedOn.Value.Day == DateTime.Now.AddDays(-90).Day).ToList();
            else if (drpval == 6)
                PaymentHistory = PaymentHistory.Where(p => p.RecievedOn.Value.Year == DateTime.Now.Year).ToList();
            else if (drpval == 7)
                PaymentHistory = PaymentHistory.Where(p => p.RecievedOn.Value.Year == DateTime.Now.AddYears(-1).Year).ToList();
            else if (drpval == 8)
                PaymentHistory = PaymentHistory.Where(p => p.DueDate.Value.Month == DateTime.Now.Month).ToList();
            else if (drpval == 9)
                PaymentHistory = PaymentHistory.Where(p => p.DueDate.Value.Month == DateTime.Now.AddMonths(-1).Month).ToList();
            return PaymentHistory;
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult ReportPaymentHistories(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort, int? drpval)
        {

            List<paymentHistoryreport> PaymentHistory = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                //      PaymentHistory = db.paymentHistories.Where(p => p.Landlord_id == id).ToList();
                PaymentHistory = db.paymentHistories.Where(p => p.Landlord_id == id)
                     .Join(
                         db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                         paymnt => paymnt.Property_Id,
                         prp => prp.Property_Id,
                         (paymnt, prp) => new { Payments = paymnt, Properties = prp }).Join(
                         db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                         pmt => pmt.Payments.Tenant_Id,
                         tnt => tnt.Tenant_Id,
                         (pmt, tnt) => new { pmt.Payments, pmt.Properties, Tenants = tnt })
                    .GroupJoin(
                         db.Buildings,
                         prp1 => prp1.Properties.BuildingId,
                         b => b.Building_Id,
                         (prp1, b) => new { prp1.Payments, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() }).Select(c => new paymentHistoryreport
                         {
                             Property = c.Properties.PropertyName,
                             Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                             Building = c.b.BuildingName,
                             RecievedOn = c.Payments.RecievedOn,
                             Type = c.Payments.Type,
                             Amount = c.Payments.Amount,
                             DueDate = c.Payments.DueDate,
                             BuildingId = c.b.Building_Id,
                             TenantId = c.Tenants.Tenant_Id,
                             PropertyId = c.Properties.Property_Id
                         }).ToList();
            }
            PaymentHistory = getdrpval(drpval, PaymentHistory);
            if (drpBuilding != null)
                PaymentHistory = PaymentHistory.Where(p => p.BuildingId == drpBuilding).ToList();
            if (drpProperty != null)
                PaymentHistory = PaymentHistory.Where(p => p.PropertyId == drpProperty).ToList();
            if (drpTenant != null)
                PaymentHistory = PaymentHistory.Where(p => p.TenantId == drpTenant).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\PaymentHistoryReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsPaymentHistory", PaymentHistory);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public JsonResult PropertyList(long? id)
        {
            var data = db.Properties.Where(p => p.LeasedTo == id).ToList();
            return Json(new SelectList(data.ToArray(), "Property_Id", "PropertyName"), JsonRequestBehavior.AllowGet);
        }
        [SessionExpire]

        public JsonResult TenantList(int? id)
        {
            var data = db.Tenants.Where(p => p.Property_Id == id).ToList();
            return Json(new SelectList(data.ToArray(), "Tenant_Id", "FirstName"), JsonRequestBehavior.AllowGet);
        }
        [SessionExpire]
        public ActionResult ReportLease()
        {
            int? id = 0;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
            }
            //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
            ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1).ToList(), "Property_Id", "PropertyName");
            List<ActiveLease> lease = null;
            if (Session["LandlordId"] != null)
            {
                //int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                lease = db.tbl_lease
               .Join(
                   db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                   lse => lse.primaryTenant_Id,
                   tnt => tnt.Tenant_Id,
                   (lse, tnt) => new { Lease = lse, Tenants = tnt })
              .Join(
                   db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                   lse1 => lse1.Lease.Property_Id,
                  prp => prp.Property_Id,
                   (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                     .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
               .Select(c => new ActiveLease
               {
                   Property = c.Properties.PropertyName,
                   Address = c.Properties.AddressLine1,
                   Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   StartDate = c.Lease.Startdate,
                   EndDate = c.Lease.EndDate,
                   Building = c.b.BuildingName,
                   LateFee = c.Lease.MaxLateFee,
                   Rent = c.Lease.Amount
               }).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ReportActiveLease.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsActiveLease", lease);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult ReportLease(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort, DateTime? FromDate, DateTime? ToDate)
        {
            int? id = 0;
            if (Session["LandlordId"] != null)
            { id = Convert.ToInt32(Session["LandlordId"].ToString()); }
            ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1).ToList(), "Property_Id", "PropertyName");
            //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
            List<ActiveLease> lease = null;
            if (Session["LandlordId"] != null)
            {
                lease = db.tbl_lease.Where(p => EntityFunctions.TruncateTime(p.Startdate) >= FromDate && EntityFunctions.TruncateTime(p.Startdate) <= ToDate)
               .Join(
                   db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                   lse => lse.primaryTenant_Id,
                   tnt => tnt.Tenant_Id,
                   (lse, tnt) => new { Lease = lse, Tenants = tnt })
              .Join(
                   db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                   lse1 => lse1.Lease.Property_Id,
                  prp => prp.Property_Id,
                   (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                     .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
               .Select(c => new ActiveLease
               {
                   Property = c.Properties.PropertyName,
                   Address = c.Properties.AddressLine1,
                   Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   StartDate = c.Lease.Startdate,
                   EndDate = c.Lease.EndDate,
                   Building = c.b.BuildingName,
                   LateFee = c.Lease.MaxLateFee,
                   Rent = c.Lease.Amount,
                   BuildingId = c.b.Building_Id,
                   TenantId = c.Tenants.Tenant_Id,
                   PropertyId = c.Properties.Property_Id
               }).ToList();

            }
            if (drpBuilding != null)
                lease = lease.Where(p => p.BuildingId == drpBuilding).ToList();
            if (drpProperty != null)
                lease = lease.Where(p => p.PropertyId == drpProperty).ToList();
            if (drpTenant != null)
                lease = lease.Where(p => p.TenantId == drpTenant).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ReportActiveLease.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsActiveLease", lease);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportAccountinfo()
        {
            int? id = 0;
            List<paymentHistory> ph = new List<paymentHistory>();
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                ph = db.paymentHistories.Where(p => p.Landlord_id == id).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\reportaccount.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsaccountinfo", ph);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult accountingreport()
        {
            int? id = 0;
            double? expanditure = 0.0;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
            }
            ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
            List<Accountingdata> accdata = null;
            if (Session["LandlordId"] != null)
            {
                //int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                accdata = db.paymentHistories
               .Join(
                   db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                   lse => lse.Tenant_Id,
                   tnt => tnt.Tenant_Id,
                   (lse, tnt) => new { Lease = lse, Tenants = tnt })
              .Join(
                   db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                   lse1 => lse1.Lease.Property_Id,
                  prp => prp.Property_Id,
                   (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                     .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
               //.GroupJoin(
               //db.Faults,
               //prp2 => prp2.Properties.Property_Id,
               //flt => flt.PropertyId,
               //(prp2, flt) => new { prp2.Lease, prp2.Tenants, prp2.Properties,prp2.b, flt = flt.FirstOrDefault() })
               .Select(c => new Accountingdata
               {
                   Property = c.Properties.PropertyName,
                   Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   Building = c.b.BuildingName,
                   RecievedOn = c.Lease.RecievedOn,
                   Amount = c.Lease.Amount,
                   Balance = c.Tenants.Balance,
                   Comment = c.Lease.Comment,
                   BuildingId = c.b.Building_Id,
                   TenantId = c.Tenants.Tenant_Id,
                   PropertyId = c.Properties.Property_Id
               }).ToList();
                foreach (var item in accdata)
                {
                    var cnt = RandomFunctions.getExpenditure(item.PropertyId);
                    foreach (var q in cnt)
                    {
                        if (q.TenantMaintainanceCost != null)
                        { expanditure += q.TenantMaintainanceCost; }
                        else if (q.MaintenanceCost != null)
                        { expanditure += q.MaintenanceCost; }
                    }
                    item.Expenditure = expanditure;
                }

            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AccountingReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsAccountingdata", accdata);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult accountingreport(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort)
        {
            int? id = 0;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
            }
            ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
            List<Accountingdata> accdata = null;
            if (Session["LandlordId"] != null)
            {
                //int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                accdata = db.paymentHistories
               .Join(
                   db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                   lse => lse.Tenant_Id,
                   tnt => tnt.Tenant_Id,
                   (lse, tnt) => new { Lease = lse, Tenants = tnt })
              .Join(
                   db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                   lse1 => lse1.Lease.Property_Id,
                  prp => prp.Property_Id,
                   (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                     .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
               .Select(c => new Accountingdata
               {
                   Property = c.Properties.PropertyName,
                   Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   Building = c.b.BuildingName,
                   RecievedOn = c.Lease.RecievedOn,
                   Amount = c.Lease.Amount,
                   Balance = c.Tenants.Balance,
                   Comment = c.Lease.Comment

               }).ToList();
            }
            if (drpBuilding != null)
                accdata = accdata.Where(p => p.BuildingId == drpBuilding).ToList();
            if (drpProperty != null)
                accdata = accdata.Where(p => p.PropertyId == drpProperty).ToList();
            if (drpTenant != null)
                accdata = accdata.Where(p => p.TenantId == drpTenant).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\AccountingReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsAccountingdata", accdata);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportInventory()
        {
            int? id = 0;
            List<InventoryReport> ph = new List<InventoryReport>();
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
                //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1).ToList(), "Property_Id", "PropertyName");
                // ph = db.tbl_inventory.Where(p => p.LandlordId == id).ToList();
            }
            ph = db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id)
               .Join(
                   db.tbl_inventory,
                   prp => prp.Property_Id,
                   inventory => inventory.PropertyId,
                   (prp, inventory) => new { Properties = prp, Inventory = inventory })
              .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Properties, prp1.Inventory, b = b.FirstOrDefault() })
               .Select(c => new InventoryReport
               {
                   Property = c.Properties.PropertyName,
                   Building = c.b.BuildingName,
                   Name = c.Inventory.Name,
                   Quantity = c.Inventory.Quantity,
                   Cost = c.Inventory.Cost
               }).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\InventoryReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsInventory", ph);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult ReportInventory(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort)
        {
            int? id = 0;
            List<InventoryReport> ph = new List<InventoryReport>();
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
                //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1).ToList(), "Property_Id", "PropertyName");
                // ph = db.tbl_inventory.Where(p => p.LandlordId == id).ToList();
            }
            ph = db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id)
               .Join(
                   db.tbl_inventory,
                   prp => prp.Property_Id,
                   inventory => inventory.PropertyId,
                   (prp, inventory) => new { Properties = prp, Inventory = inventory })
              .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Properties, prp1.Inventory, b = b.FirstOrDefault() })
               .Select(c => new InventoryReport
               {
                   Property = c.Properties.PropertyName,
                   Building = c.b.BuildingName,
                   Name = c.Inventory.Name,
                   Quantity = c.Inventory.Quantity,
                   Cost = c.Inventory.Cost,
                   BuildingId = c.b.Building_Id,

                   PropertyId = c.Properties.Property_Id
               }).ToList();
            if (drpBuilding != null)
                ph = ph.Where(p => p.BuildingId == drpBuilding).ToList();
            if (drpProperty != null)
                ph = ph.Where(p => p.PropertyId == drpProperty).ToList();
            //if (drpTenant != null)
            //    ph = ph.Where(p => p.TenantId == drpTenant).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\InventoryReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsInventory", ph);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportWorkOrders()
        {
            int? id = 0;
            List<FaultReport> ph = new List<FaultReport>();
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                // ph = db.tbl_inventory.Where(p => p.LandlordId == id).ToList();
            }
            ph = db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id)
               .Join(
                   db.Faults,
                   prp => prp.Property_Id,
                   faults => faults.PropertyId,
                   (prp, faults) => new { Properties = prp, Fault = faults }).GroupJoin(
                   db.Tenants,
                   prp2 => prp2.Fault.TenantId,
                   tenant => tenant.Tenant_Id,
                   (prp2, tenant) => new { Properties = prp2.Properties, Fault = prp2.Fault, Tenant = tenant.FirstOrDefault() })
                   .GroupJoin(
                   db.Serviceproviders,
                   prp3 => prp3.Fault.ServiceProviderId,
                   provider => provider.ProviderId,
                   (prp3, provider) => new { Properties = prp3.Properties, Fault = prp3.Fault, Tenant = prp3.Tenant, Provider = provider.FirstOrDefault() })
              .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Properties, prp1.Fault, Tenant = prp1.Tenant, Provider = prp1.Provider, b = b.FirstOrDefault() })
               .Select(c => new FaultReport
               {
                   Property = c.Properties.PropertyName,
                   Building = c.b.BuildingName,
                   Tenant = c.Tenant.FirstName + " " + c.Tenant.LastName,
                   Provider = c.Provider.FirstName + " " + c.Provider.LastName,
                   VisitDate = c.Fault.VisitDate,
                   VisitTime = c.Fault.VisitTime,
                   JobStatus = c.Fault.JobStatus,
                   Cost = c.Fault.MaintenanceCost
               }).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\FaultReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsFault", ph);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult ReportWorkOrders(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort)
        {
            int? id = 0;
            List<FaultReport> ph = new List<FaultReport>();
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                // ph = db.tbl_inventory.Where(p => p.LandlordId == id).ToList();
            }
            ph = db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id)
               .Join(
                   db.Faults,
                   prp => prp.Property_Id,
                   faults => faults.PropertyId,
                   (prp, faults) => new { Properties = prp, Fault = faults }).GroupJoin(
                   db.Tenants,
                   prp2 => prp2.Fault.TenantId,
                   tenant => tenant.Tenant_Id,
                   (prp2, tenant) => new { Properties = prp2.Properties, Fault = prp2.Fault, Tenant = tenant.FirstOrDefault() })
                   .GroupJoin(
                   db.Serviceproviders,
                   prp3 => prp3.Fault.ServiceProviderId,
                   provider => provider.ProviderId,
                   (prp3, provider) => new { Properties = prp3.Properties, Fault = prp3.Fault, Tenant = prp3.Tenant, Provider = provider.FirstOrDefault() })
              .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Properties, prp1.Fault, Tenant = prp1.Tenant, Provider = prp1.Provider, b = b.FirstOrDefault() })
               .Select(c => new FaultReport
               {
                   Property = c.Properties.PropertyName,
                   Building = c.b.BuildingName,
                   Tenant = c.Tenant.FirstName + " " + c.Tenant.LastName,
                   Provider = c.Provider.FirstName + " " + c.Provider.LastName,
                   VisitDate = c.Fault.VisitDate,
                   VisitTime = c.Fault.VisitTime,
                   Cost = c.Fault.MaintenanceCost,
                   BuildingId = c.b.Building_Id,
                   TenantId = c.Tenant.Tenant_Id,
                   PropertyId = c.Properties.Property_Id
               }).ToList();
            if (drpBuilding != null)
                ph = ph.Where(p => p.BuildingId == drpBuilding).ToList();
            if (drpProperty != null)
                ph = ph.Where(p => p.PropertyId == drpProperty).ToList();
            if (drpTenant != null)
                ph = ph.Where(p => p.TenantId == drpTenant).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\FaultReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsFault", ph);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        [SessionExpire]

        public ActionResult AcceptApplicationRequest(int? id)
        {
            var data = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
            data.SentStatus = 1;
            data.Status = 1;
            db.SaveChanges();
            string body = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Application Request Has been Aceepted for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            SendEmails.SendNotificationToTenant(data.User.Email, "Application Request Accepted", body);
            string body1 = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Accepted Application Request of " + data.User.FirstName + " " + data.User.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            SendEmails.SendNotificationToLandlord(data.Tenant.Email, "Application Request Accepted", body1);
            return RedirectToAction("TenantApplicationRequests");
        }
        [SessionExpire]
        public ActionResult RejectApplicationRequest(int? id)
        {
            var data = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
            data.SentStatus = 3;
            db.SaveChanges();
            string body = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Application Request Has been Rejected for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            SendEmails.SendNotificationToTenant(data.User.Email, "Application Request Rejected", body);
            string body1 = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Rejected Application Request of " + data.User.FirstName + " " + data.User.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            SendEmails.SendNotificationToLandlord(data.Tenant.Email, "Application Request Rejected", body1);
            return RedirectToAction("TenantApplicationRequests");
        }

        [SessionExpire]
        public ActionResult ApplicationAcceptedByLandlord(int? id)
        {
            var data = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
            data.Status = 2;
            db.SaveChanges();
            Property prp = db.Properties.Where(p => p.Property_Id == data.PropertyId).FirstOrDefault();
            prp.LeasedEnd = data.Moveout.Value.Date;
            db.SaveChanges();
            TempData["AdminApplicationRequests"] = "Application Accepted";
            return RedirectToAction("AdminApplicationRequests");
        }
        [SessionExpire]
        public ActionResult ApplicationRejectedByLandlord(int? id)
        {
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult ApplicationRejectedByLandlord(int? id, string Comments)
        {
            var data = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
            data.Status = 3;
            data.CmtRejected = Comments;
            db.SaveChanges();
            TempData["AdminApplicationRequests"] = "Application Rejected";
            return Json(new { success = true });
        }
        [SessionExpire]
        public ActionResult ApplicationDefferedByLandlord(int? id)
        {
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult ApplicationDefferedByLandlord(int? id, string Comments)
        {
            var data = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
            data.Status = 4;
            data.CmtDeffered = Comments;
            db.SaveChanges();
            TempData["AdminApplicationRequests"] = "Application Deffered";
            return Json(new { success = true });
        }
        [SessionExpire]
        public ActionResult showappdetail(int? id)
        {
            var data = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
            return View(data);
        }
        [SessionExpire]

        public ActionResult ReportIncomeAccounting()
        {
            int? id = 0;
            //   List<FaultReport> ph = new List<FaultReport>();
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                // ph = db.tbl_inventory.Where(p => p.LandlordId == id).ToList();
            }
            var ph = db.paymentHistories.Where(p => p.Landlord_id == id && (p.paymentsectionid == 0 || p.paymentsectionid == 1 || p.paymentsectionid == 2))
               .Join(
                   db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                   lse => lse.Tenant_Id,
                   tnt => tnt.Tenant_Id,
                   (lse, tnt) => new { Lease = lse, Tenants = tnt })
              .Join(
                   db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                   lse1 => lse1.Lease.Property_Id,
                  prp => prp.Property_Id,
                   (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                     .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
               .Select(c => new Accountingdata
               {
                   Property = c.Properties.PropertyName,
                   Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   Building = c.b.BuildingName,
                   RecievedOn = c.Lease.RecievedOn,
                   Amount = c.Lease.Amount,
                   Balance = c.Tenants.Balance,
                   Comment = c.Lease.Comment,
                   BuildingId = c.b.Building_Id,
                   TenantId = c.Tenants.Tenant_Id,
                   PropertyId = c.Properties.Property_Id
               }).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\IncomeAccountingReports.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsIncomeAccounting", ph);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportExpenseAccounting()
        {
            int? id = 0;
            //   List<FaultReport> ph = new List<FaultReport>();
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                // ph = db.tbl_inventory.Where(p => p.LandlordId == id).ToList();
            }
            var ph = db.paymentHistories.Where(p => p.Landlord_id == id && (p.paymentsectionid == 3 || p.paymentsectionid == 4 || p.paymentsectionid == 5))
               .Join(
                   db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                   lse => lse.Tenant_Id,
                   tnt => tnt.Tenant_Id,
                   (lse, tnt) => new { Lease = lse, Tenants = tnt })
              .Join(
                   db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                   lse1 => lse1.Lease.Property_Id,
                  prp => prp.Property_Id,
                   (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                     .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
               .Select(c => new Accountingdata
               {
                   Property = c.Properties.PropertyName,
                   Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   Building = c.b.BuildingName,
                   RecievedOn = c.Lease.RecievedOn,
                   Amount = c.Lease.Amount,
                   Balance = c.Tenants.Balance,
                   Comment = c.Lease.Comment,
                   BuildingId = c.b.Building_Id,
                   TenantId = c.Tenants.Tenant_Id,
                   PropertyId = c.Properties.Property_Id
               }).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\IncomeAccountingReports.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsIncomeAccounting", ph);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportProfitLoss()
        {
            int? id = 0;
            //   List<FaultReport> ph = new List<FaultReport>();
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                // ph = db.tbl_inventory.Where(p => p.LandlordId == id).ToList();
            }
            var ph = db.paymentHistories.Where(p => p.Landlord_id == id && (p.paymentsectionid == 0 || p.paymentsectionid == 1 || p.paymentsectionid == 2))
               .Join(
                   db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                   lse => lse.Tenant_Id,
                   tnt => tnt.Tenant_Id,
                   (lse, tnt) => new { Lease = lse, Tenants = tnt })
              .Join(
                   db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                   lse1 => lse1.Lease.Property_Id,
                  prp => prp.Property_Id,
                   (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                     .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
               .Select(c => new Accountingdata
               {
                   Property = c.Properties.PropertyName,
                   Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   Building = c.b.BuildingName,
                   RecievedOn = c.Lease.RecievedOn,
                   Amount = c.Lease.Amount,
                   Balance = c.Tenants.Balance,
                   Comment = c.Lease.Comment,
                   BuildingId = c.b.Building_Id,
                   TenantId = c.Tenants.Tenant_Id,
                   PropertyId = c.Properties.Property_Id
               }).ToList();
            double? expanditure = 0.0;
            var ph1 = db.paymentHistories.Where(p => p.Landlord_id == id && (p.paymentsectionid == 3 || p.paymentsectionid == 4 || p.paymentsectionid == 5))
           .Join(
               db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
               lse => lse.Tenant_Id,
               tnt => tnt.Tenant_Id,
               (lse, tnt) => new { Lease = lse, Tenants = tnt })
          .Join(
               db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
               lse1 => lse1.Lease.Property_Id,
              prp => prp.Property_Id,
               (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                 .GroupJoin(
               db.Buildings,
               prp1 => prp1.Properties.BuildingId,
               b => b.Building_Id,
               (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
           .Select(c => new Accountingdata
           {
               Property = c.Properties.PropertyName,
               Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
               Building = c.b.BuildingName,
               RecievedOn = c.Lease.RecievedOn,
               Amount = c.Lease.Amount,
               Balance = c.Tenants.Balance,
               Comment = c.Lease.Comment,
               BuildingId = c.b.Building_Id,
               TenantId = c.Tenants.Tenant_Id,
               PropertyId = c.Properties.Property_Id
           }).ToList();
            foreach (var item in ph1)
            {
                var cnt = RandomFunctions.getExpenditure(item.PropertyId);
                foreach (var q in cnt)
                {
                    if (q.TenantMaintainanceCost != null)
                    { expanditure += q.TenantMaintainanceCost; }
                    else if (q.MaintenanceCost != null)
                    { expanditure += q.MaintenanceCost; }
                }
                item.Expenditure = expanditure;
            }

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ProfitLossReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsProfit", ph);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ReportDataSource rdc1 = new ReportDataSource("dsloss", ph1);
            reportViewer.LocalReport.DataSources.Add(rdc1);
            reportViewer.LocalReport.Refresh();
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportExpiringLease(int? daterange)
        {
            int? id = 0;
            DateTime date = DateTime.Now.AddDays(Convert.ToDouble(daterange));
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
            }
            ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1 && p.LeasedEnd <= date).ToList(), "Property_Id", "PropertyName");
            // ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
            List<ActiveLease> lease = null;
            if (Session["LandlordId"] != null)
            {
                //int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                lease = db.tbl_lease
               .Join(
                   db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id && p.LeaseEnds <= date),
                   lse => lse.primaryTenant_Id,
                   tnt => tnt.Tenant_Id,
                   (lse, tnt) => new { Lease = lse, Tenants = tnt })
              .Join(
                   db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id && p.LeasedEnd <= date),
                   lse1 => lse1.Lease.Property_Id,
                  prp => prp.Property_Id,
                   (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                     .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
               .Select(c => new ActiveLease
               {
                   Property = c.Properties.PropertyName,
                   Address = c.Properties.AddressLine1,
                   Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   StartDate = c.Lease.Startdate,
                   EndDate = c.Lease.EndDate,
                   Building = c.b.BuildingName,
                   LateFee = c.Lease.MaxLateFee,
                   Rent = c.Lease.Amount
               }).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ExpireLease.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("ExpiringLease", lease);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult ReportExpiringLease(int? daterange, int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort, DateTime? FromDate, DateTime? ToDate)
        {
            int? id = 0;
            DateTime date = DateTime.Now.AddDays(Convert.ToDouble(daterange));
            if (Session["LandlordId"] != null)
            { id = Convert.ToInt32(Session["LandlordId"].ToString()); }
            //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
            ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1 && p.LeasedEnd <= date).ToList(), "Property_Id", "PropertyName");
            List<ActiveLease> lease = null;
            if (Session["LandlordId"] != null)
            {
                if (FromDate != null && ToDate != null)
                {
                    lease = db.tbl_lease.Where(p => EntityFunctions.TruncateTime(p.Startdate) >= FromDate && EntityFunctions.TruncateTime(p.Startdate) <= ToDate)
                   .Join(
                       db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                       lse => lse.primaryTenant_Id,
                       tnt => tnt.Tenant_Id,
                       (lse, tnt) => new { Lease = lse, Tenants = tnt })
                  .Join(
                       db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                       lse1 => lse1.Lease.Property_Id,
                      prp => prp.Property_Id,
                       (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                         .GroupJoin(
                       db.Buildings,
                       prp1 => prp1.Properties.BuildingId,
                       b => b.Building_Id,
                       (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
                   .Select(c => new ActiveLease
                   {
                       Property = c.Properties.PropertyName,
                       Address = c.Properties.AddressLine1,
                       Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                       StartDate = c.Lease.Startdate,
                       EndDate = c.Lease.EndDate,
                       Building = c.b.BuildingName,
                       LateFee = c.Lease.MaxLateFee,
                       Rent = c.Lease.Amount,
                       BuildingId = c.b.Building_Id,
                       TenantId = c.Tenants.Tenant_Id,
                       PropertyId = c.Properties.Property_Id
                   }).ToList();
                }
                else
                {
                    lease = db.tbl_lease
              .Join(
                  db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id && p.LeaseEnds <= date),
                  lse => lse.primaryTenant_Id,
                  tnt => tnt.Tenant_Id,
                  (lse, tnt) => new { Lease = lse, Tenants = tnt })
             .Join(
                  db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id && p.LeasedEnd <= date),
                  lse1 => lse1.Lease.Property_Id,
                 prp => prp.Property_Id,
                  (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                    .GroupJoin(
                  db.Buildings,
                  prp1 => prp1.Properties.BuildingId,
                  b => b.Building_Id,
                  (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
              .Select(c => new ActiveLease
              {
                  Property = c.Properties.PropertyName,
                  Address = c.Properties.AddressLine1,
                  Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                  StartDate = c.Lease.Startdate,
                  EndDate = c.Lease.EndDate,
                  Building = c.b.BuildingName,
                  LateFee = c.Lease.MaxLateFee,
                  Rent = c.Lease.Amount
              }).ToList();
                }
            }
            if (drpBuilding != null)
                lease = lease.Where(p => p.BuildingId == drpBuilding).ToList();
            if (drpProperty != null)
                lease = lease.Where(p => p.PropertyId == drpProperty).ToList();
            if (drpTenant != null)
                lease = lease.Where(p => p.TenantId == drpTenant).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ExpireLease.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("ExpiringLease", lease);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportExpiredLease()
        {
            int? id = 0;
            DateTime date = DateTime.Now.Date;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt32(Session["LandlordId"].ToString());
            }
            //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);

            ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1 && p.LeasedEnd <= date && p.Archievestatus == 0).ToList(), "Property_Id", "PropertyName");
            List<ActiveLease> lease = null;
            if (Session["LandlordId"] != null)
            {
                //int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                lease = db.tbl_lease
               .Join(
                   db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id && p.LeaseEnds <= date),
                   lse => lse.primaryTenant_Id,
                   tnt => tnt.Tenant_Id,
                   (lse, tnt) => new { Lease = lse, Tenants = tnt })
              .Join(
                   db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id && p.LeasedEnd <= date),
                   lse1 => lse1.Lease.Property_Id,
                  prp => prp.Property_Id,
                   (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                     .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
               .Select(c => new ActiveLease
               {
                   Property = c.Properties.PropertyName,
                   Address = c.Properties.AddressLine1,
                   Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   StartDate = c.Lease.Startdate,
                   EndDate = c.Lease.EndDate,
                   Building = c.b.BuildingName,
                   LateFee = c.Lease.MaxLateFee,
                   Rent = c.Lease.Amount
               }).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ExpiredLease.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("ExpiredLease", lease);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();



        }

        [HttpPost]
        [SessionExpire]
        public ActionResult ReportExpiredLease(int? drpBuilding, int? drpProperty, int? drpTenant, string drpGrouping, string drpSort, DateTime? FromDate, DateTime? ToDate)
        {
            int? id = 0;
            if (Session["LandlordId"] != null)
            { id = Convert.ToInt32(Session["LandlordId"].ToString()); }
            //ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
            ViewBag.property = new SelectList(db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1 && p.LeasedEnd <= ToDate && p.Archievestatus == 0).ToList(), "Property_Id", "PropertyName");
            List<ActiveLease> lease = null;
            if (Session["LandlordId"] != null)
            {
                lease = db.tbl_lease.Where(p => EntityFunctions.TruncateTime(p.Startdate) >= FromDate && EntityFunctions.TruncateTime(p.Startdate) <= ToDate)
               .Join(
                   db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                   lse => lse.primaryTenant_Id,
                   tnt => tnt.Tenant_Id,
                   (lse, tnt) => new { Lease = lse, Tenants = tnt })
              .Join(
                   db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                   lse1 => lse1.Lease.Property_Id,
                  prp => prp.Property_Id,
                   (lse1, prp) => new { lse1.Lease, lse1.Tenants, Properties = prp })
                     .GroupJoin(
                   db.Buildings,
                   prp1 => prp1.Properties.BuildingId,
                   b => b.Building_Id,
                   (prp1, b) => new { prp1.Lease, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
               .Select(c => new ActiveLease
               {
                   Property = c.Properties.PropertyName,
                   Address = c.Properties.AddressLine1,
                   Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                   StartDate = c.Lease.Startdate,
                   EndDate = c.Lease.EndDate,
                   Building = c.b.BuildingName,
                   LateFee = c.Lease.MaxLateFee,
                   Rent = c.Lease.Amount,
                   BuildingId = c.b.Building_Id,
                   TenantId = c.Tenants.Tenant_Id,
                   PropertyId = c.Properties.Property_Id
               }).ToList();

            }
            if (drpBuilding != null)
                lease = lease.Where(p => p.BuildingId == drpBuilding).ToList();
            if (drpProperty != null)
                lease = lease.Where(p => p.PropertyId == drpProperty).ToList();
            if (drpTenant != null)
                lease = lease.Where(p => p.TenantId == drpTenant).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ExpiredLease.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("ExpiredLease", lease);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportCollected()
        {
            List<BalanceReport> PaymentHistory = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);
                var qq = db.paymentHistories.ToList();
                PaymentHistory = db.paymentHistories.Where(p => p.Landlord_id == id)
                     .Join(
                         db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                         paymnt => paymnt.Property_Id,
                         prp => prp.Property_Id,
                         (paymnt, prp) => new { Payments = paymnt, Properties = prp }).Join(
                         db.Tenants.Where(p => p.leasestatus == 1),
                         pmt => pmt.Payments.Tenant_Id,
                         tnt => tnt.Tenant_Id,
                         (pmt, tnt) => new { pmt.Payments, pmt.Properties, Tenants = tnt }).GroupJoin(
                         db.Buildings,
                         prp1 => prp1.Properties.BuildingId,
                         b => b.Building_Id,
                         (prp1, b) => new { prp1.Payments, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
                    .Join(
                         db.tbl_lease,
                         lse => lse.Payments.Property_Id,
                         l => l.Property_Id,
                         (lse, l) => new { lse.Payments, lse.Tenants, lse.Properties, lse.b, l = l })
                    .Select(c => new BalanceReport
                    {
                        Property = c.Properties.PropertyName,
                        Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                        Telephone = c.Tenants.Telephone,
                        Building = c.b.BuildingName,
                        Due = c.l.DueDate,
                        Type = c.Payments.Type,
                        Lease_Period = (EntityFunctions.DiffDays(c.l.Startdate, c.l.EndDate) / (365.65 / 12)),
                        Days_Late = c.l.DaysAfterDueDate,
                        Balance = c.Tenants.Balance,
                        Collected = c.Payments.Amount,
                        BuildingId = c.b.Building_Id,
                        TenantId = c.Tenants.Tenant_Id,
                        PropertyId = c.Properties.Property_Id
                    }).ToList();

            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ReportCollected.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsbalances", PaymentHistory);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult ReportCollected(int? drpBuilding, int? drpProperty, int? drpTenant, DateTime? fromdate, DateTime? todate)
        {
            List<BalanceReport> PaymentHistory = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());

                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", "All Buildings");
                var qq = db.paymentHistories.ToList();
                if (fromdate != null && todate != null)
                {
                    PaymentHistory = db.paymentHistories.Where(p => p.Landlord_id == id && (p.RecievedOn >= fromdate && p.RecievedOn <= todate))
                         .Join(
                             db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                             paymnt => paymnt.Property_Id,
                             prp => prp.Property_Id,
                             (paymnt, prp) => new { Payments = paymnt, Properties = prp }).Join(
                             db.Tenants.Where(p => p.leasestatus == 1),
                             pmt => pmt.Payments.Tenant_Id,
                             tnt => tnt.Tenant_Id,
                             (pmt, tnt) => new { pmt.Payments, pmt.Properties, Tenants = tnt }).GroupJoin(
                             db.Buildings,
                             prp1 => prp1.Properties.BuildingId,
                             b => b.Building_Id,
                             (prp1, b) => new { prp1.Payments, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
                        .Join(
                             db.tbl_lease,
                             lse => lse.Tenants.Tenant_Id,
                             l => l.primaryTenant_Id,
                             (lse, l) => new { lse.Payments, lse.Tenants, lse.Properties, lse.b, l = l }).Select(c => new BalanceReport
                             {
                                 Property = c.Properties.PropertyName,
                                 Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                                 Telephone = c.Tenants.Telephone,
                                 Building = c.b.BuildingName,
                                 Due = c.l.DueDate,
                                 Type = c.Payments.Type,
                                 Lease_Period = (EntityFunctions.DiffDays(c.l.Startdate, c.l.EndDate) / (365.65 / 12)),
                                 Days_Late = c.l.DaysAfterDueDate,
                                 Balance = c.Tenants.Balance,
                                 Collected = c.Payments.Amount,
                                 BuildingId = c.b.Building_Id,
                                 TenantId = c.Tenants.Tenant_Id,
                                 PropertyId = c.Properties.Property_Id
                             }).ToList();
                }
                else
                {
                    PaymentHistory = db.paymentHistories.Where(p => p.Landlord_id == id)
                      .Join(
                          db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id),
                          paymnt => paymnt.Property_Id,
                          prp => prp.Property_Id,
                          (paymnt, prp) => new { Payments = paymnt, Properties = prp }).Join(
                          db.Tenants.Where(p => p.leasestatus == 1),
                          pmt => pmt.Payments.Tenant_Id,
                          tnt => tnt.Tenant_Id,
                          (pmt, tnt) => new { pmt.Payments, pmt.Properties, Tenants = tnt }).GroupJoin(
                          db.Buildings,
                          prp1 => prp1.Properties.BuildingId,
                          b => b.Building_Id,
                          (prp1, b) => new { prp1.Payments, prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
                     .Join(
                          db.tbl_lease,
                          lse => lse.Payments.Property_Id,
                          l => l.Property_Id,
                          (lse, l) => new { lse.Payments, lse.Tenants, lse.Properties, lse.b, l = l })
                     .Select(c => new BalanceReport
                     {
                         Property = c.Properties.PropertyName,
                         Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                         Telephone = c.Tenants.Telephone,
                         Building = c.b.BuildingName,
                         Due = c.l.DueDate,
                         Type = c.Payments.Type,
                         Lease_Period = (EntityFunctions.DiffDays(c.l.Startdate, c.l.EndDate) / (365.65 / 12)),
                         Days_Late = c.l.DaysAfterDueDate,
                         Balance = c.Tenants.Balance,
                         Collected = c.Payments.Amount,
                         BuildingId = c.b.Building_Id,
                         TenantId = c.Tenants.Tenant_Id,
                         PropertyId = c.Properties.Property_Id
                     }).ToList();
                }
            }
            if (PaymentHistory != null)
            {
                if (drpBuilding != null)
                    PaymentHistory = PaymentHistory.Where(p => p.BuildingId == drpBuilding).ToList();
                if (drpProperty != null)
                    PaymentHistory = PaymentHistory.Where(p => p.PropertyId == drpProperty).ToList();
                if (drpTenant != null)
                    PaymentHistory = PaymentHistory.Where(p => p.TenantId == drpTenant).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ReportCollected.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsbalances", PaymentHistory);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult ReportCurrentBalances()
        {
            List<BalanceReport> PaymentHistory = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", 4);

                PaymentHistory = db.tbl_lease.Join(
                    db.Properties.Where(p => p.Landlord_Id == id && p.leasestatus == 1),
                    lse => lse.Property_Id,
                    prp => prp.Property_Id,
                    (lse, prp) => new { Lease = lse, Properties = prp }).GroupJoin(
                     db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id),
                      prp1 => prp1.Properties.LeasedTo,
                       tnt => tnt.Tenant_Id,
                    (prp1, tnt) => new { prp1.Lease, prp1.Properties, Tenants = tnt.FirstOrDefault() }).GroupJoin(
                         db.Buildings,
                         prp2 => prp2.Properties.BuildingId,
                         b => b.Building_Id,
                         (prp2, b) => new { prp2.Lease, prp2.Properties, prp2.Tenants, b = b.FirstOrDefault() }).Select(c => new BalanceReport
                         {
                             Property = c.Properties.PropertyName,
                             Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                             Telephone = c.Tenants.Telephone,
                             Building = c.b.BuildingName,
                             Due = c.Lease.DueDate,
                             Lease_Period = (EntityFunctions.DiffDays(c.Lease.Startdate, c.Lease.EndDate) / (365.65 / 12)),
                             Days_Late = c.Lease.DaysAfterDueDate,
                             Balance = c.Lease.balance,
                             BuildingId = c.b.Building_Id,
                             TenantId = c.Tenants.Tenant_Id,
                             PropertyId = c.Properties.Property_Id
                         }).ToList();

                //PaymentHistory = db.Tenants.Where(p => p.leasestatus == 1 && p.Landlord_id == id).Join(
                //         db.Properties,                        
                //         tnt => tnt.Tenant_Id,
                //          prp => prp.LeasedTo,
                //         (tnt, prp) => new { Tenants = tnt, Properties = prp }).GroupJoin(
                //         db.Buildings,
                //         tnt1 => tnt1.Properties.BuildingId,
                //         b => b.Building_Id,
                //         (prp1, b) => new { prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
                //    //.Join(
                //    //     db.tbl_lease,
                //    //     lse => lse.Tenants.Tenant_Id,
                //    //     l => l.primaryTenant_Id,
                //    //     (lse, l) => new { lse.Tenants, lse.Properties, lse.b, l = l })
                //    .Select(c => new BalanceReport
                //    {
                //        Property = c.Properties.PropertyName,
                //        Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                //        Telephone = c.Tenants.Telephone,
                //        Building = c.b.BuildingName,
                //    //    Due = c.l.DueDate,                       
                //    //    Lease_Period = (EntityFunctions.DiffDays(c.l.Startdate, c.l.EndDate) / (365.65 / 12)),
                //      //  Days_Late = c.l.DaysAfterDueDate,
                //     //   Balance = c.l.balance,                       
                //        BuildingId = c.b.Building_Id,
                //        TenantId = c.Tenants.Tenant_Id,
                //        PropertyId = c.Properties.Property_Id
                //    }).ToList();

            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ReportCurrentBalance.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsbalances", PaymentHistory);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult ReportCurrentBalances(int? drpBuilding, int? drpProperty, int? drpTenant)
        {
            List<BalanceReport> PaymentHistory = null;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());

                ViewBag.building = new SelectList(db.Buildings.Where(p => p.LandlordId == id).ToList(), "Building_Id", "BuildingName", "All Buildings");
                PaymentHistory = db.Properties.Where(p => p.leasestatus == 1 && p.Landlord_Id == id).Join(
                       db.Tenants.Where(p => p.leasestatus == 1),
                       prp => prp.LeasedTo,
                       tnt => tnt.Tenant_Id,
                       (prp, tnt) => new { Properties = prp, Tenants = tnt }).GroupJoin(
                       db.Buildings,
                       prp1 => prp1.Properties.BuildingId,
                       b => b.Building_Id,
                       (prp1, b) => new { prp1.Tenants, prp1.Properties, b = b.FirstOrDefault() })
                  .Join(
                       db.tbl_lease,
                       lse => lse.Properties.Property_Id,
                       l => l.Property_Id,
                       (lse, l) => new { lse.Tenants, lse.Properties, lse.b, l = l })
                  .Select(c => new BalanceReport
                  {
                      Property = c.Properties.PropertyName,
                      Tenant = c.Tenants.FirstName + " " + c.Tenants.LastName,
                      Telephone = c.Tenants.Telephone,
                      Building = c.b.BuildingName,
                      Due = c.l.DueDate,
                      Lease_Period = (EntityFunctions.DiffDays(c.l.Startdate, c.l.EndDate) / (365.65 / 12)),
                      Days_Late = c.l.DaysAfterDueDate,
                      Balance = c.Tenants.Balance,
                      BuildingId = c.b.Building_Id,
                      TenantId = c.Tenants.Tenant_Id,
                      PropertyId = c.Properties.Property_Id
                  }).ToList();
            }
            if (PaymentHistory != null)
            {
                if (drpBuilding != null)
                    PaymentHistory = PaymentHistory.Where(p => p.BuildingId == drpBuilding).ToList();
                if (drpProperty != null)
                    PaymentHistory = PaymentHistory.Where(p => p.PropertyId == drpProperty).ToList();
                if (drpTenant != null)
                    PaymentHistory = PaymentHistory.Where(p => p.TenantId == drpTenant).ToList();
            }
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            //tbBuilding ds = new tbBuilding();
            //ds.Tables[0] = building.;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\Reports\ReportCurrentBalance.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportDataSource rdc = new ReportDataSource("dsbalances", PaymentHistory);
            reportViewer.LocalReport.DataSources.Add(rdc);
            ViewBag.ReportViewer = reportViewer;
            return View();
        }
        [SessionExpire]
        public ActionResult lease(long? id)
        {
            int term = 1; string duration = "0"; DateTime StartDate = DateTime.Now.Date;
            if (duration == "0")
            {
                ViewBag.EndDateValue = StartDate.AddMonths(term).ToShortDateString();
            }
            else if (duration == "1")
            {
                int days = term * 7;
                ViewBag.EndDateValue = StartDate.AddDays(days).ToShortDateString();
            }
            else if (duration == "2")
            {
                ViewBag.EndDateValue = StartDate.AddDays(term).ToShortDateString();
            }
            GetleaseFormData(id);
            ViewBag.propid = id;
            var leasedetails = db.tbl_lease.Where(p => p.Property_Id == id).FirstOrDefault();
            if (leasedetails != null)
            {
                return View(leasedetails);
            }
            return View();
        }

        public JsonResult GetPropertyDet(int? id)
        {
            var rnt = db.Properties.Where(p => p.Property_Id == id).FirstOrDefault();
            return Json(rnt.Price, JsonRequestBehavior.AllowGet);

        }

        [SessionExpire]
        private void GetleaseFormData(long? id)
        {
            long? lid = 0;
            if (Session["LandlordId"] != null)
                lid = Convert.ToInt64(Session["LandlordId"]);
            double? prc = db.Properties.Where(p => p.Property_Id == id).Select(p => p.Price).FirstOrDefault();
            ViewBag.actualrent = prc;
            var ddd = db.tbl_lease.Where(p => p.Property_Id == id).FirstOrDefault();
            var data = new List<Property>();

            data = db.Properties.Where(p => p.leasestatus == 0).ToList();
            if (ddd != null)
            {
                var prpdata = db.Properties.Where(p => p.Property_Id == id).FirstOrDefault();

                data.Add(prpdata);
            }
            Session["test"] = data.Count();
            var data1 = db.Properties.Where(p => p.Property_Id == id).FirstOrDefault();
            var lstTenants = db.Tenants.Where(p => p.Landlord_id == lid && p.leasestatus == 0).ToList();
            if (Session["Check"] != null)
            {
                ViewBag.tenant = new SelectList(lstTenants, "Tenant_Id", "FirstName", id);
                ViewBag.lease = new SelectList(data, "Property_Id", "DataTextFieldLabel");
            }
            else
            {
                ViewBag.lease = new SelectList(data, "Property_Id", "DataTextFieldLabel", id);
                ViewBag.tenant = new SelectList(lstTenants, "Tenant_Id", "FirstName");
            }

            ViewBag.Sectenant = new SelectList(lstTenants, "Tenant_Id", "FirstName");
            ViewBag.DescriptionId = new SelectList(db.Descriptions.OrderBy(p => p.Description1).ToList(), "DescriptionId", "Description1");
            var selectedrentdue = 1;
            ViewBag.rentisdue = new SelectList(db.RentDues.ToList(), "RentdueId", "RentDueValue", selectedrentdue);

            //var ddd = db.tbl_lease.Where(p => p.Property_Id == id).FirstOrDefault();
            if (ddd != null)
            {
                var appdetails = db.application_details.Where(p => p.PropertyId == id).FirstOrDefault();
                if (appdetails != null)
                {
                    DateTime dt = appdetails.Moveindate.Value.Date;
                    DateTime dt1 = appdetails.Moveout.Value.Date;
                    TimeSpan gettotaldate1 = dt1.Date.Subtract(dt);
                    int dur = gettotaldate1.Days / 30;
                    ViewBag.moveindt = dt;
                    ViewBag.moveoutdt = dt1;
                    ViewBag.dur = dur;
                    double? da = db.paymentHistories.Where(p => p.Property_Id == id).Select(p => p.Amount).FirstOrDefault();
                    ViewBag.amount = da;
                }
            }
        }
        //Lease POST
        [HttpPost]
        [SessionExpire]
        public ActionResult lease(List<tbl_othercharges> otherchargesmodel, string Command, tbl_lease ls, int? id, long? propid, long? Property_Id, long? primaryTenant_Id, long? sectenant_Id, DateTime StartDate, int term, string duration, float? Rent, int? rentisdue, DateTime firstrentalpay, DateTime EndDate, string DescriptionId, float? Amount, string Due, DateTime? ValidFrom, DateTime? ValidUntil, string Reason, string Type, double? CreditAmount, DateTime? CreditedOn, bool? LatefeeEnabled, int? DaysAfterDueDate, bool? FlatFeeOrBaseAmountOrPercentRent, double? FlatFee, double? percentRent, double? baseAmount, bool? ExcludeGracePeriod, double? MaxLateFee, int? Bi_firstdate, int? Bi_Lastdate, string paymentSchedule , string paymentmode , long? lease_id , string description , string due)
        {
            GetleaseFormData(id);
            if (Property_Id == null)
                Property_Id = propid;
            TimeSpan getdatediff = EndDate.Date.Subtract(StartDate.Date);
            double? balance = 0.0;

            balance = getdatediff.Days / 30 * Rent;

            if (Command == "save lease")
            {
                if (Session["Applicants"] != null)
                {
                    var applicantdata = db.Applicants.Where(p => p.Applicant_Id == id).FirstOrDefault();
                    long? lid = 0;
                    if (Session["LandlordId"] != null)
                        lid = Convert.ToInt64(Session["LandlordId"]);
                    Tenant tnt = new Tenant();
                    tnt.Landlord_id = lid;
                    tnt.Address1 = applicantdata.Address1;
                    tnt.Address2 = applicantdata.Address2;
                    tnt.Cellular = applicantdata.Mobile;
                    tnt.City = applicantdata.City;
                    tnt.Comment = applicantdata.Comment;
                    tnt.Dateofbirth = applicantdata.Dateofbirth;
                    tnt.DriverLicense = applicantdata.DriverLicense;
                    tnt.Email = applicantdata.Email;
                    tnt.FirstName = applicantdata.FirstName;
                    tnt.Gender = applicantdata.Gender;
                    tnt.LastName = applicantdata.LastName;
                    tnt.PostCode = applicantdata.PostCode;
                    tnt.Profile = applicantdata.Profile;
                    tnt.Smoker = applicantdata.Smoker;
                    tnt.SocialSecurity = applicantdata.SocialSecurity;
                    tnt.State = applicantdata.State;
                    tnt.Telephone = applicantdata.Telephone;                    
                    db.Tenants.Add(tnt);
                    db.SaveChanges();
                    primaryTenant_Id = db.Tenants.OrderByDescending(p => p.Tenant_Id).Select(p => p.Tenant_Id).FirstOrDefault();
                    Applicant app = db.Applicants.Find(id);
                    db.Applicants.Remove(app);
                    db.SaveChanges();

                    return RedirectToAction("tenant_Index");
                }
                var cheeckData = db.tbl_lease.Where(p => p.Property_Id == Property_Id).FirstOrDefault();
                if (cheeckData != null)
                {
                    tbl_lease lss = db.tbl_lease.Where(p => p.Property_Id == Property_Id).FirstOrDefault();
                    lss.duration = duration;
                    lss.EndDate = EndDate;
                    Session["leaseend"] = lss.EndDate;
                    lss.firstrentalpay = firstrentalpay;
                    lss.primaryTenant_Id = primaryTenant_Id;
                    Session["tenantlease"] = lss.primaryTenant_Id;
                    lss.Rent = Rent;
                    lss.balance = balance;
                    lss.sectenant_Id = sectenant_Id;
                    lss.Startdate = StartDate;
                    lss.term = ls.term;
                    lss.Property_Id = Property_Id;
                    lss.rentisdue = rentisdue;
                    //lss.DescriptionId = DescriptionId;
                    lss.Amount = Amount;
                    lss.Due = Due;
                    lss.ValidFrom = ValidFrom;
                    lss.ValidUntil = ValidUntil;
                    lss.Reason = Reason;
                    lss.Type = Type;
                    lss.CreditAmount = CreditAmount;
                    lss.CreditedOn = CreditedOn;
                    lss.LatefeeEnabled = LatefeeEnabled;
                    lss.DaysAfterDueDate = DaysAfterDueDate;
                    lss.FlatFeeOrBaseAmountOrPercentRent = FlatFeeOrBaseAmountOrPercentRent;
                    lss.FlatFee = FlatFee;
                    lss.percentRent = percentRent;
                    lss.baseAmount = baseAmount;
                    lss.ExcludeGracePeriod = ExcludeGracePeriod;
                    lss.MaxLateFee = MaxLateFee;
                    lss.paymentmode = paymentmode;
                    lss.PaymentStatus = 0;
                    lss.paymentSchedule = paymentSchedule;                  
                    lss.updatedOn = DateTime.Now.Date;
                    db.SaveChanges();
                    var property = db.Properties.Where(p => p.Property_Id == Property_Id).FirstOrDefault();
                    property.leasestatus = 1;
                    property.LeasedTo = lss.primaryTenant_Id;
                    property.LeasedEnd = EndDate;
                    var tenant = db.Tenants.Where(p => p.Tenant_Id == lss.primaryTenant_Id).FirstOrDefault();
                    tenant.LeaseEnds = EndDate;
                    tenant.Property_Id = lss.Property_Id;
                    tenant.Balance = balance;
                    tenant.leasestatus = 1;
                    db.SaveChanges();
                    var othercharges = db.tbl_othercharges.Where(l => l.lease_id == lease_id).FirstOrDefault();
                    othercharges.lease_id = othercharges.lease_id;
                    othercharges.description = description;
                    othercharges.due = due;
                    othercharges.validfrom = ValidFrom;
                    othercharges.validuntil = ValidUntil;
                    db.SaveChanges();

                    if (Session["Check"] != null)
                    {
                        var tenantdata = db.Tenants.Where(p => p.Tenant_Id == id).FirstOrDefault();
                        tenant.LeaseEnds = EndDate;
                        tenant.Property_Id = lss.Property_Id;
                        tenant.leasestatus = 1;
                        db.SaveChanges();
                        Session["Check"] = null;
                        return RedirectToAction("tenant_Index");
                    }
                    if (Session["Applicants"] != null)
                    {
                        var tenantdata = db.Tenants.Where(p => p.Tenant_Id == id).FirstOrDefault();
                        tenantdata.LeaseEnds = EndDate;
                        tenantdata.Property_Id = lss.Property_Id;
                        tenantdata.Balance = balance;
                        tenantdata.leasestatus = 1;
                        db.SaveChanges();
                        Session["Applicants"] = null;
                        return RedirectToAction("tenant_Index");
                    }
                }
                else
                {
                    tbl_lease lss = new tbl_lease();
                    lss.duration = duration;
                    lss.EndDate = EndDate;
                    Session["leaseend"] = lss.EndDate;
                    lss.firstrentalpay = firstrentalpay;
                    lss.primaryTenant_Id = primaryTenant_Id;
                    Session["tenantlease"] = lss.primaryTenant_Id;
                    lss.Rent = Rent;
                    lss.sectenant_Id = sectenant_Id;
                    lss.Startdate = StartDate;
                    lss.term = ls.term;
                    lss.Property_Id = Property_Id;
                    lss.rentisdue = rentisdue;
                   // lss.DescriptionId = DescriptionId;
                    lss.Amount = Amount;
                    lss.Due = Due;
                    lss.ValidFrom = ValidFrom;
                    lss.ValidUntil = ValidUntil;
                    lss.Reason = Reason;
                    lss.Type = Type;
                    lss.CreditAmount = CreditAmount;
                    lss.CreditedOn = CreditedOn;
                    lss.LatefeeEnabled = LatefeeEnabled;
                    lss.DaysAfterDueDate = DaysAfterDueDate;
                    lss.FlatFeeOrBaseAmountOrPercentRent = FlatFeeOrBaseAmountOrPercentRent;
                    lss.FlatFee = FlatFee;
                    lss.percentRent = percentRent;
                    lss.baseAmount = baseAmount;
                    lss.ExcludeGracePeriod = ExcludeGracePeriod;
                    lss.MaxLateFee = MaxLateFee;
                    lss.PaymentStatus = 0;
                    lss.paymentmode = paymentmode;
                    lss.paymentSchedule = paymentSchedule;
                    lss.createdOn = DateTime.Now.Date;
                    lss.updatedOn = DateTime.Now.Date;
                    db.tbl_lease.Add(lss);
                    tbl_othercharges othercharges = new tbl_othercharges();
                    othercharges.lease_id = othercharges.lease_id;
                    othercharges.description = description;
                    othercharges.due = due;
                    othercharges.validfrom = ValidFrom;
                    othercharges.validuntil = ValidUntil;
                    db.tbl_othercharges.Add(othercharges);
                    var property = db.Properties.Where(p => p.Property_Id == Property_Id).FirstOrDefault();
                    property.leasestatus = 1;
                    property.LeasedTo = lss.primaryTenant_Id;
                    property.LeasedEnd = EndDate;
                    var tenant = db.Tenants.Where(p => p.Tenant_Id == lss.primaryTenant_Id).FirstOrDefault();
                    tenant.LeaseEnds = EndDate;
                    tenant.Property_Id = lss.Property_Id;
                    tenant.Balance = balance;
                    tenant.leasestatus = 1;
                    db.SaveChanges();
                    if (Session["Check"] != null)
                    {
                        var tenantdata = db.Tenants.Where(p => p.Tenant_Id == id).FirstOrDefault();
                        tenantdata.LeaseEnds = EndDate;
                        tenantdata.Property_Id = lss.Property_Id;
                        tenantdata.Balance = balance;
                        tenantdata.leasestatus = 1;
                        db.SaveChanges();
                        Session["Check"] = null;
                        return RedirectToAction("tenant_Index");
                    }
                    if (Session["Applicants"] != null)
                    {
                        var tenantdata = db.Tenants.Where(p => p.Tenant_Id == id).FirstOrDefault();
                        tenantdata.LeaseEnds = EndDate;
                        tenantdata.Property_Id = lss.Property_Id;
                        tenantdata.Balance = balance;
                        tenantdata.leasestatus = 1;
                        db.SaveChanges();
                        Session["Applicants"] = null;
                        return RedirectToAction("tenant_Index");
                    }
                }
            }
            else if (Command == "Refresh")
            {
                TimeSpan gettotaldate = EndDate.Date.Subtract(StartDate.Date);
                List<DateTime> obj = new List<DateTime>();
                if (rentisdue == 1)
                {
                    int days = gettotaldate.Days / 30;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddMonths(i));
                    }
                }
                else if (rentisdue == 2)
                {
                    int days = gettotaldate.Days / 30;

                    for (int i = 1; i <= days; i++)
                    {
                        for (int j = 1; j <= 30; j++)
                        {
                            if (j == Bi_firstdate)
                            {
                                obj.Add(StartDate.Date.AddDays(Convert.ToDouble(Bi_firstdate)));
                                obj.Add(StartDate.Date.AddDays(Convert.ToDouble(Bi_Lastdate)));
                                break;
                            }
                        }
                    }
                }
                else if (rentisdue == 3)
                {
                    int days = gettotaldate.Days / 7;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddDays(i * 7));
                    }
                }
                else if (rentisdue == 4)
                {
                    int days = gettotaldate.Days / 14;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddDays(i * 14));
                    }
                }
                else if (rentisdue == 5)
                {
                    int days = gettotaldate.Days;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddDays(i));
                    }
                }
                else if (rentisdue == 6)
                {
                    int days = gettotaldate.Days / 90;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddMonths(i * 3));
                    }
                }
                else if (rentisdue == 7)
                {
                    int days = gettotaldate.Days / 182;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddMonths(i * 6));
                    }
                }
                else if (rentisdue == 8)
                {
                    int days = gettotaldate.Days / 365;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddYears(i));
                    }
                }
                GetDatesTerm(StartDate, term, duration);
                ViewBag.Refresh = obj;
                ViewBag.actualrent = null;
                ViewBag.RefreshRent = Rent;
                return View();
            }
            else
            {
                GetDatesTerm(StartDate, term, duration);
                return View();
            }
            return RedirectToAction("PropertyIndex");
        }

        [SessionExpire]
        public ActionResult acceptrequest(int id)
        {
            var data = db.RequestViews.Where(p => p.ReqviewId == id && p.RequestStatus == 1).OrderByDescending(p => p.ReqviewId).FirstOrDefault();
            data.Approve = 1;
            db.SaveChanges();//commented
            //string body = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Viewing Request Has been Aceepted for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            //SendEmails.SendNotificationToTenant(data.Tenant.Email, "Viewing Request Accepted", body);
            //string body1 = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Accepted Viewing Request of " + data.Tenant.FirstName + " " + data.Tenant.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            //SendEmails.SendNotificationToLandlord(data.User.Email, "Viewing Request Accepted", body1);
            return RedirectToAction("landlordviewingrequest");

        }
        [SessionExpire]
        public ActionResult rejectrequest(int id)
        {
            var data = db.RequestViews.Where(p => p.ReqviewId == id && p.RequestStatus == 1).OrderByDescending(p => p.ReqviewId).FirstOrDefault();
            data.Approve = 2;
            db.SaveChanges();//Commnetd
            //string body = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Viewing Request Has been Rejected for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            //SendEmails.SendNotificationToTenant(data.Tenant.Email, "Viewing Request Rejected", body);
            //string body1 = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Rejected Viewing Request of " + data.Tenant.FirstName + " " + data.Tenant.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            //SendEmails.SendNotificationToLandlord(data.User.Email, "Viewing Request Rejected", body1);
            return RedirectToAction("landlordviewingrequest");
        }
        [SessionExpire]
        
        public ActionResult DepositRequest(int? id)
        {
            if (Session["LandlordId"] != null)
            {
                var s = db.RequestViews.Where(p => p.ReqviewId == id && p.Approve == 1).OrderByDescending(p => p.ReqviewId).FirstOrDefault();
                //var q = db.tbl_offer.Where(p => p.OfferId == id).FirstOrDefault();
                //s.Approve = 6;
                s.Status = 2;
                db.SaveChanges();
                var amount = db.Properties.Where(p => p.Property_Id == s.PropertyId).Select(p => p.HoldingFee).FirstOrDefault();
                Deposit Depositdata = new Deposit();
                Depositdata.DepositApproval = 0;
                Depositdata.DepositAmount = amount;
                Depositdata.DateCreated = DateTime.UtcNow.Date;
                Depositdata.LandlordId = Convert.ToInt64(Session["LandlordId"]);
                Depositdata.PropertyId = s.PropertyId;
                Depositdata.TenantId = s.TenantId;
                Depositdata.DepositStatus = s.Status;
                Depositdata.DepositStatus = 0;
                //Depositdata.OfferId = s.OfferId;
                Depositdata.ReqviewId = s.ReqviewId;
                //Depositdata.DepositApproval = s.Status;
                db.Deposits.Add(Depositdata);
                db.SaveChanges();
                var rd = db.Deposits.OrderByDescending(p => p.DepositId).Take(1).FirstOrDefault();
                TenancyDetail td = db.TenancyDetails.Where(p => p.TenantId == s.TenantId && p.LandlordId == s.LandlordId && p.PropertyId == s.PropertyId ).FirstOrDefault();
                td.DepositId = rd.DepositId;
                db.SaveChanges();
                return RedirectToAction("Landlordviewingrequest");
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult landlordviewingrequest()
        {
            long? id = 0;

            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt64(Session["LandlordId"]);
                var data = db.RequestViews.Where(p => p.LandlordId == id && p.Approve == 0).OrderByDescending(p => p.ReqviewId).ToList();
                ViewBag.Pending = data;
                var data1 = db.RequestViews.Where(p => p.LandlordId == id && p.Approve == 1 ).OrderByDescending(p => p.ReqviewId).ToList();
                ViewBag.Accepted = data1;
                var data2 = db.RequestViews.Where(p => p.LandlordId == id && p.Approve == 2 ).OrderByDescending(p => p.ReqviewId).ToList();
                ViewBag.rejected = data2;
                var data3 = db.RequestViews.Where(p => p.LandlordId == id && p.Approve == 3 && p.RequestStatus == 1).OrderByDescending(p => p.ReqviewId).ToList();
                ViewBag.purposal = data3;
                //var data6 = db.RequestViews.Where(p => p.LandlordId == id && p.Approve == 6 && p.RequestStatus == 1 && p.Status==1).OrderByDescending(p => p.ReqviewId).ToList();
                //ViewBag.DepositRequest = data6;
                //var data3 = db.ViewingPurposals.Where(p => p.LandlordId == id && p.Approve == 0).ToList();
                //ViewBag.Pendingprop = data3;
                //var data4 = db.ViewingPurposals.Where(p => p.LandlordId == id && p.Approve == 1).ToList();
                //ViewBag.acceptedprop = data4;
                //var data5 = db.ViewingPurposals.Where(p => p.LandlordId == id && p.Approve == 2).ToList();
                //ViewBag.rejectedprop = data5;
                return View();
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult AdminApplicationRequests()
        {
            long? id = 0;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt64(Session["LandlordId"]);
                var data = db.application_details.Where(p => p.LandlordId == id && p.Status == 0).ToList();
                ViewBag.Pending = data;
                var data1 = db.application_details.Where(p => p.LandlordId == id && (p.Status == 1 || p.Status == 5)).ToList();
                ViewBag.accepted = data1;
                var data5 = db.application_details.Where(p => p.LandlordId == id && p.Status == 2).ToList();
                ViewBag.acceptedLandlord = data5;
                var data2 = db.application_details.Where(p => p.LandlordId == id && p.Status == 3).ToList();
                ViewBag.Rejected = data2;
                var data3 = db.application_details.Where(p => p.LandlordId == id && p.Status == 4).ToList();
                ViewBag.Deffered = data3;
                var data4 = db.application_details.Where(p => p.LandlordId == id && p.FillStatus == 1).ToList();
                ViewBag.FillStatus = data4;
                return View();
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult ApplicationRequest(int? id)
        {
            if (Session["LandlordId"] != null)
            {
                var deposit = db.Deposits.Where(p => p.DepositId == id).FirstOrDefault();
                deposit.DepositApproval = 4;
                db.SaveChanges();
                application_details app = new application_details();
                app.LandlordId = deposit.LandlordId;
                app.TenantId = deposit.TenantId;
                app.Status = 0;
                app.SentStatus = 0;
                app.FillStatus = 0;
                app.DateCreated = DateTime.UtcNow.Date;
                app.PropertyId = deposit.PropertyId;
                db.application_details.Add(app);
                db.SaveChanges();
                TempData["AdminDepositRequests"] = "Application Request Sent Successfully";
                return RedirectToAction("AdminDepositRequests");
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult AdminDepositRequests()
        {
            long? id = 0;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt64(Session["LandlordId"]);
                var data = db.Deposits.Where(p => p.LandlordId == id && p.DepositApproval == 0).ToList();
                ViewBag.Pending = data;
                var data1 = db.Deposits.Where(p => p.LandlordId == id && p.DepositApproval == 1).ToList();
                ViewBag.accepted = data1;
                var data2 = db.Deposits.Where(p => p.LandlordId == id && p.DepositApproval == 2).ToList();
                ViewBag.Rejected = data2;
                var data3 = db.Deposits.Where(p => p.LandlordId == id && p.DepositApproval == 3).ToList();
                ViewBag.DepositStatus = data3;
                return View();
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult TenancyAgreement(int? id)
        {
            var app = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
            var prp = db.Properties.Where(p => p.Property_Id == app.PropertyId).FirstOrDefault();
            Session["propdata"] = prp;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TenancyAgreement(TenancyAgreement ta, int? id, HttpPostedFileBase TenancyAgreementData, HttpPostedFileBase Signatures)
        {
            if (TenancyAgreementData != null && Signatures != null)
            {
                var app = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
                app.SentStatus = 2;
                db.SaveChanges();
                int Years = app.Moveout.Value.Date.Year - app.Moveindate.Value.Date.Year;
                int month = app.Moveout.Value.Date.Month - app.Moveindate.Value.Date.Month;
                string duration = "";
                if (month > 0)
                    duration = Years.ToString() + "Year " + month.ToString();
                else
                    duration = Years.ToString() + "Year";
                TenancyAgreement agreement = new TenancyAgreement();
                agreement.FromAgreementDate = app.Moveindate.Value.Date;
                agreement.ToAgreementDate = app.Moveout.Value.Date;
                agreement.TenantId = app.TenantId;
                agreement.LandlordId = app.LandlordId;
                agreement.PropertyId = app.PropertyId;
                string pic = "", pic1 = "";
                if (TenancyAgreementData != null)
                {
                    TenancyAgreementData.SaveAs(Server.MapPath("~/TenancyAgreementDoc/" + TenancyAgreementData.FileName));
                    pic = TenancyAgreementData.FileName;
                }
                else
                    pic = "";
                agreement.TenancyAgreementData = pic;
                if (Signatures != null)
                {
                    Signatures.SaveAs(Server.MapPath("~/TenancyAgreementDoc/" + Signatures.FileName));
                    pic1 = Signatures.FileName;
                }
                else
                    pic1 = "";
                agreement.Signatures = pic1;
                agreement.RenewalDate = app.Moveout.Value.Date;
                agreement.Duration = duration;
                agreement.ApprovalStatus = 0;
                agreement.DateAdded = DateTime.UtcNow.Date;
                agreement.RenewStatus = 0;
                db.TenancyAgreements.Add(agreement);
                db.SaveChanges();
                var rd = db.TenancyAgreements.OrderByDescending(p => p.TenancyAgreementId).Take(1).FirstOrDefault();
                TenancyDetail td = db.TenancyDetails.Where(p => p.TenantId == app.TenantId && p.LandlordId == app.LandlordId && p.PropertyId == app.PropertyId).FirstOrDefault();
                td.TenancyAgrrementId = rd.TenancyAgreementId;
                db.SaveChanges();
                TempData["AdminApplicationRequests"] = "Agreement Sent Successfully";
            }
            else
            {
                TempData["AdminApplicationRequests"] = "Upload Agreement & Signatures first";
            }
            // return Json(new { success = true });
            return RedirectToAction("AdminApplicationRequests");
        }

        [SessionExpire]
        public ActionResult AdminTenancyAgreement()
        {
            long? id = 0;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt64(Session["LandlordId"]);
                var data = db.TenancyAgreements.Where(p => p.LandlordId == id && p.ApprovalStatus == 0).ToList();
                ViewBag.Pending = data;
                var data1 = db.TenancyAgreements.Where(p => p.LandlordId == id && p.ApprovalStatus == 1).ToList();
                ViewBag.accepted = data1;
                var data2 = db.TenancyAgreements.Where(p => p.LandlordId == id && p.ApprovalStatus == 2).ToList();
                ViewBag.Rejected = data2;

                return View();
            }
            else
                return RedirectToAction("Login");
        }

        //public ActionResult TenantTenancyAgreement()
        //{
        //    int? id = 0;
        //    if (Session["TenantId"] != null)
        //    {
        //        id = Convert.ToInt32(Session["TenantId"]);
        //        var data = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 0).ToList();
        //        ViewBag.Pending = data;
        //        var data1 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 1).ToList();
        //        ViewBag.accepted = data1;
        //        var data2 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 2).ToList();
        //        ViewBag.Rejected = data2;               
        //        return View();
        //    }
        //    else
        //        return RedirectToAction("Login");
        //}
        [SessionExpire]
        //public ActionResult TenantTenancyAgreement()
        //{
        //    Session["TenantId"] = 1021;
        //    int? id = 0;
        //    if (Session["TenantId"] != null)
        //    {
        //        id = Convert.ToInt32(Session["TenantId"]);
        //        var data = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 0 && p.RenewStatus == 0).ToList();
        //        ViewBag.Pendingold = data;
        //        var data1 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 1 && p.RenewStatus == 0).ToList();
        //        ViewBag.acceptedold = data1;
        //        var data2 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 2 && p.RenewStatus == 0).ToList();
        //        ViewBag.Rejectedold = data2;
        //        var data3 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 0 && p.RenewStatus == 1).ToList();
        //        ViewBag.Pendingrenew = data3;
        //        var data4 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 1 && p.RenewStatus == 1).ToList();
        //        ViewBag.acceptedrenew = data4;
        //        var data5 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 2 && p.RenewStatus == 1).ToList();
        //        ViewBag.Rejectedrenew = data5;
        //        var data6 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 0 && p.RenewStatus == 2).ToList();
        //        ViewBag.Pendingcancel = data6;
        //        var data7 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 1 && p.RenewStatus == 2).ToList();
        //        ViewBag.acceptedcancel = data7;
        //        var data8 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 2 && p.RenewStatus == 2).ToList();
        //        ViewBag.Rejectedcancel = data8;
        //        return View();
        //    }
        //    else
        //        return RedirectToAction("Login");
        //}

        //[SessionExpire]
        //public ActionResult AcceptTenancyAgreement(int? id)
        //{
        //    var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
        //    long? id1 = data.PropertyId;
        //    ViewBag.TAID = data.TenancyAgreementData;
        //    var prp = db.Properties.Where(p => p.Property_Id == id1).FirstOrDefault();
        //    Session["propdata"] = prp;
        //    return View();
        //}
        //[HttpPost]
        //[SessionExpire]
        //public ActionResult AcceptTenancyAgreement(int? id, HttpPostedFileBase SignatureTenant, TenancyAgreement ta, bool? IsAccepted)
        //{
        //    if (IsAccepted == true)
        //    {
        //        if (SignatureTenant != null)
        //        {
        //            var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
        //            data.ApprovalStatus = 1;
        //            data.DateApproved = DateTime.UtcNow.Date;
        //            string pic = "";
        //            if (SignatureTenant != null)
        //            {
        //                SignatureTenant.SaveAs(Server.MapPath("~/TenancyAgreementDoc/" + SignatureTenant.FileName));
        //                pic = SignatureTenant.FileName;
        //            }
        //            else
        //                pic = "";
        //            data.SignatureTenant = pic;
        //            db.SaveChanges();
        //            var prp = db.Properties.Where(p => p.Property_Id == data.PropertyId).FirstOrDefault();
        //            Session["propdata"] = prp;
        //            TempData["TenantTenancyAgreement"] = "Agreement Accepted Successfully";
        //            string body = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Tenancy Agreement Has been Aceepted for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
        //            SendEmails.SendNotificationToTenant(data.User.Email, "Tenancy Agreement Accepted", body);
        //            string body1 = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Accepted Tenancy Agreement of " + data.User.FirstName + " " + data.User.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
        //            SendEmails.SendNotificationToLandlord(data.Tenant.Email, "Tenancy Agreement Accepted", body1);
        //        }
        //        else
        //            TempData["TenantTenancyAgreement"] = "Upload Signature First";
        //    }
        //    else
        //    {
        //        TempData["TenantTenancyAgreement"] = "Accept Agreement Conditions to Continue";
        //    }
        //    return RedirectToAction("TenantTenancyAgreement");
        //}
        //[SessionExpire]
        //public ActionResult RejectTenancyAgreement(int? id)
        //{
        //    var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
        //    data.ApprovalStatus = 2;
        //    data.DateApproved = DateTime.UtcNow.Date;
        //    db.SaveChanges();
        //    string body = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Tenancy Agreement Has been Rejected for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
        //    SendEmails.SendNotificationToTenant(data.User.Email, "Tenancy Agreement Rejected", body);
        //    string body1 = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Rejected Tenancy Agreement of " + data.User.FirstName + " " + data.User.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
        //    SendEmails.SendNotificationToLandlord(data.Tenant.Email, "Tenancy Agreement Rejected", body1);
        //    TempData["TenantTenancyAgreement"] = "Agreement Rejected";
        //    return RedirectToAction("TenantTenancyAgreement");
        //}
        //public FileStreamResult GetPDF(int ? id)
        //{
        //    var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
        //    FileStream fs = new FileStream(Server.MapPath("/TenancyAgreementDoc/"+data.TenancyAgreementData), FileMode.Open, FileAccess.Read);
        //    return File(fs, "application/pdf");
        //}
        //[SessionExpire]
        public ActionResult RejectDepositRequest(int? id)
        {
            var data = db.Deposits.Where(p => p.DepositId == id).FirstOrDefault();
            data.DepositApproval = 2;
            db.SaveChanges();
            string body = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Deposit Request Has been Rejected for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            SendEmails.SendNotificationToTenant(data.User.Email, "Deposit Request Rejected", body);
            string body1 = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Rejected Deposit Request of " + data.User.FirstName + " " + data.User.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            SendEmails.SendNotificationToLandlord(data.Tenant.Email, "Deposit Request Rejected", body1);
            return RedirectToAction("TenantDepositRequests");
        }
        [SessionExpire]
        public ActionResult GetDocuments()
        {
            int? cntid = 0;
            if (Session["LandlordId"] != null)
            {
                int? id = Convert.ToInt32(Session["LandlordId"].ToString());
                cntid = db.Users.Where(p => p.UserID == id).Select(p => p.Country).FirstOrDefault();
                ViewBag.UserDocuments = db.UserDocuments.Where(p => p.LandlordId == id).ToList();
            }
            var docc = db.tblDocuments.Where(p => p.Country == cntid).ToList();
            return View(docc);
        }
        [SessionExpire]
        public ActionResult DocDetails(int? id)
        {
            var docc = db.tblDocuments.Where(p => p.DocumentId == id).FirstOrDefault();
            return View(docc);
        }
        [SessionExpire]
        public ActionResult UserDocDetails(int? id)
        {
            var docc = db.UserDocuments.Where(p => p.UserDocumentId == id).FirstOrDefault();
            return View(docc);
        }
        [SessionExpire]
        public ActionResult AddDoc(int? id)
        {
            int? lid = 0;
            if (Session["LandlordId"] != null)
                lid = Convert.ToInt32(Session["LandlordId"].ToString());
            var docc = db.tblDocuments.Where(p => p.DocumentId == id).FirstOrDefault();
            UserDocument usrd = new UserDocument();
            usrd.DocumentId = id;
            usrd.DocumentTypeId = docc.DocumentTypeId;
            usrd.LandlordId = lid;
            usrd.ContentDetails = docc.DocumentDetails;
            usrd.Status = docc.Status;
            usrd.DateCreated = DateTime.Now.Date;
            string s = docc.Title.ToString() + "(Copy)";
            usrd.DocTitle = s;
            db.UserDocuments.Add(usrd);
            db.SaveChanges();
            return RedirectToAction("GetDocuments");
        }
        [SessionExpire]
        public ActionResult AddNewDocType()
        {
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult AddNewDocType(DocumentType usd)
        {
            int? lid = 0;
            if (Session["LandlordId"] != null)
                lid = Convert.ToInt32(Session["LandlordId"].ToString());
            DocumentType usrd = new DocumentType();
            usrd.DocumentTypeName = usd.DocumentTypeName;
            db.DocumentTypes.Add(usrd);
            db.SaveChanges();
            return Json(new { success = true });
            //  return RedirectToAction("AddNewDoc");
        }
        [SessionExpire]
        public ActionResult AddNewDoc()
        {
            ViewBag.columnTenants = from t in typeof(Tenant).GetProperties() select t.Name;
            ViewBag.columnLandlord = from t in typeof(User).GetProperties() select t.Name;
            ViewBag.columnProperty = from t in typeof(Property).GetProperties() select t.Name;
            ViewBag.columnLease = from t in typeof(tbl_lease).GetProperties() select t.Name;
            ViewBag.DocumentType = new SelectList(db.DocumentTypes.ToList(), "DocumentTypeId", "DocumentTypeName");
            return View("AddNewDoc");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult AddNewDoc(UserDocument usd)
        {
            int? lid = 0;
            if (Session["LandlordId"] != null)
                lid = Convert.ToInt32(Session["LandlordId"].ToString());
            UserDocument usrd = new UserDocument();
            usrd.ContentDetails = usd.ContentDetails;
            usrd.DocumentTypeId = usd.DocumentTypeId;
            usrd.DocTitle = usd.DocTitle;
            usrd.DateCreated = DateTime.Now.Date;
            db.UserDocuments.Add(usrd);
            db.SaveChanges();
            return Json(new { success = true });
            //return RedirectToAction("GetDocuments");
        }
        [SessionExpire]
        public ActionResult EditUserDoc(int? id)
        {
            ViewBag.columnTenants = from t in typeof(Tenant).GetProperties() select t.Name;
            ViewBag.columnLandlord = from t in typeof(User).GetProperties() select t.Name;
            ViewBag.columnProperty = from t in typeof(Property).GetProperties() select t.Name;
            ViewBag.columnLease = from t in typeof(tbl_lease).GetProperties() select t.Name;
            var docc = db.UserDocuments.Where(p => p.UserDocumentId == id).FirstOrDefault();
            ViewBag.DocumentType = new SelectList(db.DocumentTypes.ToList(), "DocumentTypeId", "DocumentTypeName");
            return View("EditUserDoc", docc);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult EditUserDoc(int? id, UserDocument usd)
        {
            int? lid = 0;
            if (Session["LandlordId"] != null)
                lid = Convert.ToInt32(Session["LandlordId"].ToString());
            var usrd = db.UserDocuments.Where(p => p.UserDocumentId == id).FirstOrDefault();
            usrd.ContentDetails = usd.ContentDetails;
            usrd.DocumentTypeId = usd.DocumentTypeId;
            usrd.DocTitle = usd.DocTitle;
            usrd.DateCreated = DateTime.Now.Date;
            db.SaveChanges();
            return Json(new { success = true });
            //   return RedirectToAction("GetDocuments");
        }
        [SessionExpire]
        public ActionResult DeleteUserDoc(int? id)
        {
            var docc = db.UserDocuments.Find(id);
            db.UserDocuments.Remove(docc);
            db.SaveChanges();
            return RedirectToAction("GetDocuments");
        }
        [SessionExpire]
        public ActionResult GenerateDocuments(int? id)
        {
            var q = db.UserDocuments.Find(id);
            if (q.DocumentTypeId == 4)
                ViewBag.TenantList = new SelectList(db.Tenants.Where(p => p.leasestatus == 1).ToList(), "Tenant_Id", "FirstName");
            else
                ViewBag.TenantList = new SelectList(db.Tenants.ToList(), "Tenant_Id", "FirstName");
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult GenerateDocuments(int? id, string doc, int? TenantId)
        {
            DocumentPdfModel ppp = ConvertToPdf1(id, TenantId);
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{DateAdded}", DateTime.UtcNow.ToShortDateString());
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantFirstName}", ppp.tenant.FirstName != null ? ppp.tenant.FirstName : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantLastName}", ppp.tenant.LastName != null ? ppp.tenant.LastName : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantTelephone}", ppp.tenant.Telephone != null ? ppp.tenant.Telephone : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantCellular}", ppp.tenant.Cellular != null ? ppp.tenant.Cellular : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantEmail}", ppp.tenant.Email != null ? ppp.tenant.Email : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantGender}", ppp.tenant.Gender != null ? ppp.tenant.Gender : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantAddress1}", ppp.tenant.Address1 != null ? ppp.tenant.Address1 : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantAddress2}", ppp.tenant.Address2 != null ? ppp.tenant.Address2 : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantCity}", ppp.tenant.City != null ? ppp.tenant.City : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantState}", ppp.tenant.State != null ? ppp.tenant.State : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantPostCode}", ppp.tenant.PostCode != null ? ppp.tenant.PostCode : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantBalance}", ppp.tenant.Balance.ToString() != null ? ppp.tenant.Balance.ToString() : "");
            if (ppp.tenant.LastPmtOn != null)
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantLastPmtOn}", ppp.tenant.LastPmtOn.Value.ToShortDateString() != null ? ppp.tenant.LastPmtOn.Value.ToShortDateString() : "");
            if (ppp.tenant.Nextpmtdue != null)
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantNextpmtdue}", ppp.tenant.Nextpmtdue.Value.ToShortDateString() != null ? ppp.tenant.Nextpmtdue.Value.ToShortDateString() : "");
            if (ppp.tenant.LeaseEnds != null)
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantLeaseEnds}", ppp.tenant.LeaseEnds.Value.ToShortDateString() != null ? ppp.tenant.LeaseEnds.Value.ToShortDateString() : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{Tenantleasestatus}", ppp.tenant.leasestatus.ToString() != null ? ppp.tenant.leasestatus.ToString() : "");
            if (ppp.tenant.Property_Id != null)
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{TenantProperty}", ppp.tenant.Property.PropertyName != null ? ppp.tenant.Property.PropertyName : "");
            if (ppp.tenant.Property_Id != null)
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{Tenanttbl_lease}", ppp.tenant.Property.PropertyName != null ? ppp.tenant.Property.PropertyName : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordFirstName}", ppp.user.FirstName != null ? ppp.user.FirstName : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordLastName}", ppp.user.LastName != null ? ppp.user.LastName : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordCompanyName}", ppp.user.CompanyName != null ? ppp.user.CompanyName : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordEmail}", ppp.user.Email != null ? ppp.user.Email : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordAddress1}", ppp.user.Address1 != null ? ppp.user.Address1 : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordAddress2}", ppp.user.Address2 != null ? ppp.user.Address2 : "");
            //if (ppp.user.City != null)
            //    ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordCity}", ppp.user.City1.CityName != null ? ppp.user.City1.CityName : "");
            //if (ppp.user.State != null)
            //    ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordState}", ppp.user.State1.StateName != null ? ppp.user.State1.StateName : "");
            if (ppp.user.Country != null)
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordCountry}", ppp.user.Country1.countryName != null ? ppp.user.Country1.countryName : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordZip}", ppp.user.Zip != null ? ppp.user.Zip : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordPhone}", ppp.user.Phone != null ? ppp.user.Phone : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordNameOnAccount}", ppp.user.NameOnAccount != null ? ppp.user.NameOnAccount : "");
            ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LandlordUserRole1}", ppp.user.UserRole1.RoleName != null ? ppp.user.UserRole1.RoleName : "");
            if (ppp.property != null)
            {
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyPropertyTypeName}", ppp.property.PropertyStyle.Style_Name != null ? ppp.property.PropertyStyle.Style_Name : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyPropertyName}", ppp.property.PropertyName != null ? ppp.property.PropertyName : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertySize}", ppp.property.Size != null ? ppp.property.Size : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyAddress1}", ppp.property.AddressLine1 != null ? ppp.property.AddressLine1 : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyAddress2}", ppp.property.AddressLine2 != null ? ppp.property.AddressLine2 : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyCity}", ppp.property.City != null ? ppp.property.City : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyState}", ppp.property.State != null ? ppp.property.State : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyPostCode}", ppp.property.PostCode != null ? ppp.property.PostCode : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertySqFt}", ppp.property.SqFt.ToString() != null ? ppp.property.SqFt.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyCreated}", ppp.property.Created.Value.ToShortDateString() != null ? ppp.property.Created.Value.ToShortDateString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyYearBuilt}", ppp.property.YearBuilt.ToString() != null ? ppp.property.YearBuilt.ToString() : "");
                // ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyDateAvailable}", ppp.property.DateAvailable.Value.ToShortDateString());
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyPrice}", ppp.property.Price.ToString() != null ? ppp.property.Price.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyBedroom}", ppp.property.Bedroom.ToString() != null ? ppp.property.Bedroom.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyFurnished}", ppp.property.Furnished.ToString() != null ? ppp.property.Furnished.ToString() : "");
                //ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyDateAdded}", ppp.property.DateAdded.Value.ToShortDateString());
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyBathroom}", ppp.property.Bathroom.ToString() != null ? ppp.property.Bathroom.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertySmokersAllowed}", ppp.property.SmokersAllowed.ToString() != null ? ppp.property.SmokersAllowed.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyPetsAllowed}", ppp.property.PetsAllowed.ToString() != null ? ppp.property.PetsAllowed.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyParking}", ppp.property.Parking.ToString() != null ? ppp.property.Parking.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyGarden}", ppp.property.Garden.ToString() != null ? ppp.property.Garden.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyDescription}", ppp.property.Description != null ? ppp.property.Description : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyLeasedTo}", ppp.property.LeasedTo.ToString() != null ? ppp.property.LeasedTo.ToString() : "");
                //ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyLeasedEnd}", ppp.property.LeasedEnd.Value.ToShortDateString());
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{PropertyComment}", ppp.property.Comment != null ? ppp.property.Comment : "");
            }
            if (ppp.tbllease != null)
            {
                //ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseStartdate}", ppp.tbllease.Startdate.Value.ToShortDateString());
                // ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseEndDate}", ppp.tbllease.EndDate.Value.ToShortDateString());
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{Leasesectenant_Id}", ppp.tbllease.Tenant.FirstName + " " + ppp.tbllease.Tenant.LastName + " ");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{Leaseterm}", ppp.tbllease.term.ToString() != null ? ppp.tbllease.term.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{Leaseduration}", ppp.tbllease.duration.ToString() != null ? ppp.tbllease.duration.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseRent}", ppp.tbllease.Rent.ToString() != null ? ppp.tbllease.Rent.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{Leasebalance}", ppp.tbllease.balance.ToString() != null ? ppp.tbllease.balance.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{Leaserentisdue}", ppp.tbllease.RentDue.RentDueValue.ToString() != null ? ppp.tbllease.RentDue.RentDueValue.ToString() : "");
                //ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{Leasefirstrentalpay}", ppp.tbllease.firstrentalpay.Value.ToShortDateString());
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseAmount}", ppp.tbllease.Amount.ToString() != null ? ppp.tbllease.Amount.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseDue}", ppp.tbllease.Due.ToString() != null ? ppp.tbllease.Due.ToString() : "");
                //ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseValidFrom}", ppp.tbllease.ValidFrom.Value.ToShortDateString());
                //ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseValidUntil}", ppp.tbllease.ValidUntil.Value.ToShortDateString());
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseReason}", ppp.tbllease.Reason != null ? ppp.tbllease.Reason : "");
                //ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseType}", ppp.tbllease.Type.ToString());
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseCreditAmount}", ppp.tbllease.CreditAmount.ToString() != null ? ppp.tbllease.CreditAmount.ToString() : "");
                //ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseCreditedOn}", ppp.tbllease.CreditedOn.Value.ToShortDateString());
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseLatefeeEnabled}", ppp.tbllease.LatefeeEnabled.ToString() != null ? ppp.tbllease.LatefeeEnabled.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseDaysAfterDueDate}", ppp.tbllease.DaysAfterDueDate.ToString() != null ? ppp.tbllease.DaysAfterDueDate.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseFlatFeeOrBaseAmountOrPercentRent}", ppp.tbllease.FlatFeeOrBaseAmountOrPercentRent.ToString() != null ? ppp.tbllease.FlatFeeOrBaseAmountOrPercentRent.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseFlatFee}", ppp.tbllease.FlatFee.ToString() != null ? ppp.tbllease.FlatFee.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeasepercentRent}", ppp.tbllease.percentRent.ToString() != null ? ppp.tbllease.percentRent.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeasebaseAmount}", ppp.tbllease.baseAmount.ToString() != null ? ppp.tbllease.baseAmount.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseExcludeGracePeriod}", ppp.tbllease.ExcludeGracePeriod.ToString() != null ? ppp.tbllease.ExcludeGracePeriod.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseMaxLateFee}", ppp.tbllease.MaxLateFee.ToString() != null ? ppp.tbllease.MaxLateFee.ToString() : "");
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeasePaymentStatus}", ppp.tbllease.PaymentStatus.ToString() != null ? ppp.tbllease.PaymentStatus.ToString() : "");
                //ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeaseComment}", ppp.tbllease.Comment.ToString());
                ppp.userdocument.ContentDetails = ppp.userdocument.ContentDetails.ToString().Replace("{LeasepaymentSchedule}", ppp.tbllease.paymentSchedule.ToString() != null ? ppp.tbllease.paymentSchedule.ToString() : "");
            }
            ppp.userdocument.ContentDetails = HtmlRemoval.StripTagsCharArray(ppp.userdocument.ContentDetails);
            var builder = new PdfBuilderDoc(ppp, Server.MapPath("/Views/Front/ConvertToPdf1.cshtml"), Server.MapPath("/css/style.css"));
            return builder.GetPdf1();

            //   return View();
        }
        [SessionExpire]
        public DocumentPdfModel ConvertToPdf1(int? id, int? TenantId)
        {
            DocumentPdfModel ppp = new DocumentPdfModel();
            var q = db.UserDocuments.Find(id);
            ppp.userdocument = q;
            var t = db.Tenants.Find(TenantId);
            ppp.tenant = t;
            var u = db.Users.Find(t.Landlord_id);
            ppp.user = u;
            var l = db.tbl_lease.Where(p => p.primaryTenant_Id == t.Tenant_Id).FirstOrDefault();
            ppp.tbllease = l;
            var p1 = db.Properties.Where(p => p.Property_Id == t.Property_Id).FirstOrDefault();
            ppp.property = p1;
            return ppp;
        }
        [SessionExpire]
        public ActionResult GetCommonDoc()
        {
            if (Session["LandlordId"] != null)
            {
                ViewBag.CommonDocuments = db.CommonDocuments.ToList();
                return View();
            }
            else
                return RedirectToAction("Login");
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult GetCommonDoc(CommonDocument cmd, IEnumerable<HttpPostedFileBase> DocName, string Command)
        {
            int? id = 0;
            if (Session["LandlordId"] != null)
                id = Convert.ToInt32(Session["LandlordId"].ToString());
            string path = Server.MapPath("~/Content/CommonDoc/");
            if (Command == "Upload")
            {
                foreach (var images in DocName)
                {
                    if (images.ContentLength > 0)
                    {
                        string pic = "";
                        if (images != null)
                        {
                            images.SaveAs(path + images.FileName);
                            pic = images.FileName;
                        }
                        CommonDocument cd = new CommonDocument();
                        cd.DateAdded = DateTime.UtcNow.Date;
                        cd.LandLordId = id;
                        cd.DocName = pic;
                        db.CommonDocuments.Add(cd);
                        db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("GetCommonDoc");
        }
        [SessionExpire]
        public ActionResult DeleteCommonDoc(int? id)
        {
            var docc = db.CommonDocuments.Find(id);
            db.CommonDocuments.Remove(docc);
            db.SaveChanges();
            return RedirectToAction("GetCommonDoc");
        }
        [SessionExpire]
        public ActionResult EditCommonDoc()
        {
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult EditCommonDoc(int? id, CommonDocument doc)
        {
            var docc = db.CommonDocuments.Find(id);
            docc.DocComments = doc.DocComments;
            docc.DateAdded = DateTime.Now.Date;
            db.SaveChanges();
            return Json(new { success = true });
            //  return RedirectToAction("GetCommonDoc");
        }

        public ActionResult bloghome()
        {
            var q = db.Blogs.ToList();
            return View(q);
        }
        public ActionResult AllArticle()
        {
            return View();
        }
        public ActionResult detailarticle(int? id)
        {
            var data = db.Blogs.Where(p => p.BlogId == id).FirstOrDefault();
            return View(data);
        }
        [SessionExpire]
        public ActionResult showrenew()
        {
            long id = Convert.ToInt64(Session["LandlordId"]);
            DateTime dt = DateTime.Now.Date.AddMonths(2);
            var dt1 = db.TenancyAgreements.Where(p => EntityFunctions.TruncateTime(p.ToAgreementDate) <= dt && p.RenewStatus == 0).ToList();
            var data = db.TenancyAgreements.Where(p => p.LandlordId == id && p.RenewStatus == 1).ToList();
            Session["acceptedreq"] = data;
            var data1 = db.TenancyAgreements.Where(p => p.LandlordId == id && p.RenewStatus == 2).ToList();
            Session["rejectedreq"] = data1;
            var data2 = db.damage_intventory.Where(p => p.Status == 1 || p.Status == 2 && p.Landlord_Id == id).ToList();
            Session["acceptbalance"] = data2;
            //  var dt2=dt1.Where(p=>p.ToAgreementDa)

            //foreach (var item in dt1)
            //{
            //    var data = db.TenancyAgreements.Where(p => p.LandlordId == id && item <= dt && p.RenewStatus == 0).ToList();
            //}
            return View(dt1);
        }
        [SessionExpire]
        public ActionResult sendrenew(int? id)
        {
            var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
            return View(data);
        }
        [SessionExpire]
        public ActionResult showdamageinvent()
        {
            int id = Convert.ToInt32(Session["TenantId"]);
            var data = db.damage_intventory.Where(p => p.Tenant_Id == id && p.Status == 0).ToList();
            TempData["pendinginvent"] = data;
            var data1 = db.damage_intventory.Where(p => p.Tenant_Id == id && p.Status == 1).ToList();
            TempData["acceptinvent"] = data1;
            var data2 = db.damage_intventory.Where(p => p.Tenant_Id == id && p.Status == 2).ToList();
            TempData["rejectinvent"] = data2;
            return View();
        }
        [SessionExpire]

        public ActionResult offerrequest()
        {
            long? id = 0;
            if (Session["LandlordId"] != null)
            {
                id = Convert.ToInt64(Session["LandlordId"]);
                var data = db.tbl_offer.Where(p => p.LandlordId == id && p.Approval == 0).OrderByDescending(p => p.OfferId).ToList();
                ViewBag.Pending = data;
                var data1 = db.tbl_offer.Where(p => p.LandlordId == id && p.Approval == 1).OrderByDescending(p => p.OfferId).ToList();
                ViewBag.accepted = data1;
                var data2 = db.tbl_offer.Where(p => p.LandlordId == id && p.Approval == 2).OrderByDescending(p => p.OfferId).ToList();
                ViewBag.Rejected = data2;
                var data6 = db.RequestViews.Where(p => p.LandlordId == id && p.Approve == 6 && p.RequestStatus == 1).OrderByDescending(p => p.ReqviewId).ToList();
                ViewBag.DepositRequest = data6;
                //var data3 = db.tbl_offer.Where(p => p.LandlordId == id && p.PurposalStatus == 1 && p.Approval == 3).ToList();
                //ViewBag.purposal = data3;
                //var data4 = db.tbl_offer.Where(p => p.LandlordId == id && p.Approval == 4).ToList();
                //ViewBag.DepositReq = data4;
                //var data5 = db.Purposals.Where(p => p.LandlordId == id && p.Approval == 0).ToList();
                //ViewBag.PendingPurposal = data5;
                //var data6 = db.Purposals.Where(p => p.LandlordId == id && p.Approval == 1).ToList();
                //ViewBag.acceptedPurposal = data6;
                //var data7 = db.Purposals.Where(p => p.LandlordId == id && p.Approval == 2).ToList();
                //ViewBag.RejectedPurposal = data7;
                return View();
            }
            else
                return RedirectToAction("Login");
        }
        public ActionResult SaveList(long? id)
        {
            if (Session["TenantId"] != null)
            {
                cmd = 1;           
                //Session["TenantId"] = 1021;
                long? tid = Convert.ToInt32(Session["TenantId"]);
                var data = db.Properties.Where(p => p.Property_Id == id).FirstOrDefault();
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                string jsonresult = JsonConvert.SerializeObject(data);
                Mylist mylist = new Mylist();
                mylist.ListName = "";
                mylist.ListDetails = jsonresult;
                mylist.TenantId = tid;
                mylist.PropertyId = id;
                mylist.UpdatedOn = DateTime.UtcNow.Date;
                db.Mylists.Add(mylist);
                db.SaveChanges();

                return RedirectToAction("PropertyListing");
            }
            else
                return RedirectToAction("Login");
        }

        [SessionExpire]
        private void GetDatesTerm(DateTime StartDate, int term, string duration)
        {
            if (duration == "0")
            {
                ViewBag.EndDateValue = StartDate.AddMonths(term).ToShortDateString();
            }
            else if (duration == "1")
            {
                int days = term * 7;
                ViewBag.EndDateValue = StartDate.AddDays(days).ToShortDateString();
            }
            else if (duration == "2")
            {
                ViewBag.EndDateValue = StartDate.AddDays(term).ToShortDateString();
            }
            ViewBag.payduedate = StartDate.ToShortDateString();
            ViewBag.moveindt = null;
            ViewBag.updatedstartdate = StartDate;
        }

        [SessionExpire]
        public ActionResult createinvoice(int? id)
        {
            var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
            long? id1 = data.PropertyId;
            long? idd = data.TenantId;

            var prp = db.Properties.Where(p => p.Property_Id == id1).FirstOrDefault();
            var tenant = db.Tenants.Where(p => p.Tenant_Id == idd).FirstOrDefault();
            var tenancyDetails = db.TenancyDetails.Where(p => p.TenancyAgrrementId == id).FirstOrDefault();
            if (tenancyDetails != null)
            {
               // var offerdata = db.RequestViews.Where(p => p.ReqviewId == tenancyDetails.OfferId).FirstOrDefault();
                var offerdata = db.tbl_offer.Where(p => p.OfferId == tenancyDetails.OfferId).FirstOrDefault();
                if (offerdata != null)
                    ViewBag.offerdata = offerdata;
                else
                {
                    var proposaldata = db.Purposals.Where(p => p.PurposalId == tenancyDetails.ProposalOfferId).FirstOrDefault();
                    if (proposaldata != null)
                        ViewBag.proposaldata = proposaldata;
                }
            }
            Session["invoicedata"] = prp;
            Session["Tenantinvoicedata"] = tenant;
            Session["agreedata"] = data;

            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult createinvoice(int? id, float Amount, float Amountpay, string Comment)
        {
            var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
            long? id1 = data.PropertyId;
            long? idd = data.TenantId;
            long? iddd = data.LandlordId;
            tbl_invoice invoice = new tbl_invoice();
            invoice.AgreementId = id;
            invoice.Amount = Amount;
            invoice.Amountpay = Amountpay;
            invoice.Comment = Comment;
            invoice.Created = DateTime.Now;
            invoice.LandlordId = iddd;
            invoice.PaymentStatus = 0;
            invoice.SendStatus = 1;
            invoice.TenantId = idd;
            invoice.PropertyId = id1;
            db.tbl_invoice.Add(invoice);
            db.SaveChanges();
            var rd = db.tbl_invoice.OrderByDescending(p => p.InvoiceId).Take(1).FirstOrDefault();
            TenancyDetail td = db.TenancyDetails.Where(p => p.TenantId == rd.TenantId && p.LandlordId == rd.LandlordId && p.PropertyId == rd.PropertyId).FirstOrDefault();
            td.InvoiceId = rd.InvoiceId;
            db.SaveChanges();
            return Json(new { success = true });
        }
        [SessionExpire]
        public ActionResult invoicelist()
        {
            var data = db.tbl_invoice.ToList();
            return View(data);
        }
        [SessionExpire]
        public ActionResult editlease(int id)
        {
            var data = db.tbl_lease.Where(p => p.LeaseId == id).FirstOrDefault();
            var data1 = db.Properties.Where(p => p.leasestatus == 0).ToList();
            //var q = db.Tenants.Where(p => p.Landlord_id == data.Property.Landlord_Id).ToList();
            var q = db.Tenants.Where(p => p.Landlord_id == data.Property.Landlord_Id).ToList();
            ViewBag.tenant = new SelectList(q, "Tenant_Id", "FirstName", data.primaryTenant_Id);
            ViewBag.lease = new SelectList(data1, "Property_Id", "PropertyName");
            //  ViewBag.Sectenant = new SelectList(db.Tenants.ToList(), "Tenant_Id", "FirstName", data.primaryTenant_Id);
            ViewBag.rentisdue = new SelectList(db.RentDues.ToList(), "RentdueId", "RentDueValue", 1);
            ViewBag.paymntschule = data.paymentSchedule;
            return View(data);
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult editlease(tbl_lease lease, int id, int? Property_Id, string Command, int? primaryTenant_Id, int? sectenant_Id, DateTime StartDate, int term, string duration, float? Rent, int? rentisdue, DateTime firstrentalpay, DateTime EndDate, float? Amount, string Due, DateTime? ValidFrom, DateTime? ValidUntil, string Reason, string Type, double? CreditAmount, DateTime? CreditedOn, bool? LatefeeEnabled, int? DaysAfterDueDate, bool? FlatFeeOrBaseAmountOrPercentRent, double? FlatFee, double? percentRent, double? baseAmount, bool? ExcludeGracePeriod, double? MaxLateFee, int? Bi_firstdate, int? Bi_Lastdate, string paymentSchedule)
        {
            var data = db.tbl_lease.Where(p => p.LeaseId == id).FirstOrDefault();
            var data1 = db.Properties.Where(p => p.leasestatus == 0).ToList();
            ViewBag.tenant = new SelectList(db.Tenants.ToList(), "Tenant_Id", "FirstName", data.primaryTenant_Id);
            ViewBag.lease = new SelectList(data1, "Property_Id", "PropertyName");
            ViewBag.Sectenant = new SelectList(db.Tenants.ToList(), "Tenant_Id", "FirstName", data.primaryTenant_Id);
            ViewBag.rentisdue = new SelectList(db.RentDues.ToList(), "RentdueId", "RentDueValue", 1);
            ViewBag.paymntschule = data.paymentSchedule;
            TimeSpan getdatediff = EndDate.Date.Subtract(StartDate.Date);
            double? balance = 0.0;
            balance = getdatediff.Days / 30 * Rent;
            if (Command == "save lease")
            {
                var leasedata = db.tbl_lease.Where(p => p.LeaseId == id).FirstOrDefault();
                leasedata.duration = duration;
                leasedata.EndDate = EndDate;

                leasedata.firstrentalpay = firstrentalpay;
                //leasedata.primaryTenant_Id = primaryTenant_Id;

                leasedata.Rent = Rent;
                leasedata.balance = balance;
                leasedata.sectenant_Id = sectenant_Id;
                leasedata.Startdate = StartDate;
                leasedata.term = lease.term;
                //leasedata.Property_Id = Property_Id;
                leasedata.rentisdue = rentisdue;
                leasedata.Amount = Amount;
                leasedata.Due = Due;
                leasedata.ValidFrom = ValidFrom;
                leasedata.ValidUntil = ValidUntil;
                leasedata.Reason = Reason;
                leasedata.Type = Type;
                leasedata.CreditAmount = CreditAmount;
                leasedata.CreditedOn = CreditedOn;
                leasedata.LatefeeEnabled = LatefeeEnabled;
                leasedata.DaysAfterDueDate = DaysAfterDueDate;
                leasedata.FlatFeeOrBaseAmountOrPercentRent = FlatFeeOrBaseAmountOrPercentRent;
                leasedata.FlatFee = FlatFee;
                leasedata.percentRent = percentRent;
                leasedata.baseAmount = baseAmount;
                leasedata.ExcludeGracePeriod = ExcludeGracePeriod;
                leasedata.MaxLateFee = MaxLateFee;
                leasedata.paymentSchedule = paymentSchedule;
                db.SaveChanges();
                Tenant tnt = db.Tenants.Where(p => p.Tenant_Id == primaryTenant_Id).FirstOrDefault();
                tnt.Balance = balance;
                db.SaveChanges();
            }
            if (Command == "Refresh")
            {
                TimeSpan gettotaldate = EndDate.Date.Subtract(StartDate.Date);
                List<DateTime> obj = new List<DateTime>();
                if (rentisdue == 1)
                {   
                    int days = gettotaldate.Days / 30;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddMonths(i));
                    }
                }
                else if (rentisdue == 2)
                {
                    int days = gettotaldate.Days / 30;
                    for (int i = 1; i <= days; i++)
                    {
                        for (int j = 1; j <= 30; j++)
                        {
                            if (j == Bi_firstdate)
                            {
                                obj.Add(StartDate.Date.AddDays(Convert.ToDouble(Bi_firstdate)));
                                obj.Add(StartDate.Date.AddDays(Convert.ToDouble(Bi_Lastdate)));
                                break;
                            }
                        }
                    }
                }
                else if (rentisdue == 3)
                {
                    int days = gettotaldate.Days / 7;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddDays(i * 7));
                    }
                }
                else if (rentisdue == 4)
                {
                    int days = gettotaldate.Days / 14;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddDays(i * 14));
                    }
                }
                else if (rentisdue == 5)
                {
                    int days = gettotaldate.Days;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddDays(i));
                    }
                }
                else if (rentisdue == 6)
                {
                    int days = gettotaldate.Days / 90;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddMonths(i * 3));
                    }
                }
                else if (rentisdue == 7)
                {
                    int days = gettotaldate.Days / 182;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddMonths(i * 6));
                    }
                }
                else if (rentisdue == 8)
                {
                    int days = gettotaldate.Days / 365;
                    for (int i = 1; i <= days; i++)
                    {
                        obj.Add(StartDate.Date.AddYears(i));
                    }
                }
                GetDatesTerm(StartDate, term, duration);
                ViewBag.Refresh = obj;
                ViewBag.RefreshRent = Rent;
                return View(data);
            }
            else
            {
                GetDatesTerm(StartDate, term, duration);
                return View(data);
            }
        }

        //Landlord Messages

        [SessionExpire]
 
        public ActionResult Messages(int? page, string tab)
        {
            if (Session["EmailId1"] != null)
            {
                int pageSize = 10;
                int pageIndex = 1;

                string email = Session["EmailId1"].ToString();
                var inbox1 = db.tbMessages.Where(p => p.emailto == email).OrderByDescending(p => p.SentDate).ToPagedList(pageIndex, pageSize);
                var sent1 = db.tbMessages.Where(p => p.emailfrom == email).OrderByDescending(p => p.SentDate).ToPagedList(pageIndex, pageSize);
                ViewBag.sent = sent1;
                ViewBag.inbox = inbox1;
                if (tab == "tab-1" || tab == null)
                {
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    var inbox = db.tbMessages.Where(p => p.emailto == email).OrderByDescending(p => p.SentDate).ToPagedList(pageIndex, pageSize);
                    ViewBag.inbox = inbox;
                }
                if (tab == "tab-2" || tab == null)
                {
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    var sent = db.tbMessages.Where(p => p.emailfrom == email).OrderByDescending(p => p.SentDate).ToPagedList(pageIndex, pageSize);
                    ViewBag.sent = sent;
                }
                ViewBag.tab = tab;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [SessionExpire]
        public ActionResult Message()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Message(tbMessage msg)
        {
            if (Session["EmailId1"] != null)
            {
                tbMessage _message = new tbMessage();
                _message.emailfrom = Session["EmailId1"].ToString();
                _message.Message = msg.Message;
                _message.emailto = msg.emailto;
                _message.Subject = msg.Subject;
                _message.Status = 0;
                _message.SentDate = DateTime.UtcNow.Date;
                db.tbMessages.Add(_message);
                db.SaveChanges();
                return RedirectToAction("Messages");
                //return Json(new { success = true });
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        private void SortMessages()
        {
            var slectdsent = Request.Form["SortBySent"];
            var slectdInbox = Request.Form["SortByInbox"];
            var SortBy = new List<SelectListItem>
            {
                  new SelectListItem{ Text="Select", Value = "0" },
                 new SelectListItem{ Text="Date (ASC)", Value = "1" },
                new SelectListItem{ Text="Date (DESC)", Value = "2" }
            };
            ViewBag.SortByMessagesSent = new SelectList(SortBy, "Value", "Text", slectdsent);
            ViewBag.SortByMessagesInbox = new SelectList(SortBy, "Value", "Text", slectdInbox);
        }

        [SessionExpire]
        public ActionResult _MessagesInbox()
        {
            return View();
        }
        [SessionExpire]
        public ActionResult _MessagesSent()
        {
            return View();
        }

        [SessionExpire]
        public ActionResult ViewMessageAdmin(int id)
        {
            var q = db.tbMessages.Where(p => p.MessageId == id).FirstOrDefault();
            q.Status = 1;
            db.SaveChanges();
            return View(q);
        }
        [SessionExpire]
        public ActionResult ViewMessageAdmin1(int id)
        {
            var q = db.tbMessages.Where(p => p.MessageId == id).FirstOrDefault();
            return View("ViewMessageAdmin", q);
        }
        [SessionExpire]
        public ActionResult ReplyMessageAdmin(int id)
        {
            return View();
        }
        [SessionExpire]
        [HttpPost]
        public ActionResult ReplyMessageAdmin(int id, tbMessage msg)
        {
            var q = db.tbMessages.Where(p => p.MessageId == id).FirstOrDefault();
            tbMessage _message = new tbMessage();
            _message.emailfrom = q.emailto;
            _message.Message = msg.Message;
            _message.emailto = q.emailfrom;
            _message.Subject = msg.Subject;
            _message.Status = 0;
            _message.SentDate = DateTime.UtcNow.Date;
            db.tbMessages.Add(_message);
            db.SaveChanges();
            return RedirectToAction("Messages");
            //return Json(new { success = true });
        }
    }
}