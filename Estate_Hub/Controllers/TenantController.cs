using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proptiwise.Models;
using Proptiwise.ViewModels;
using PagedList;
using System.Net.Mail;
using System.Net;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Web.UI;
using System.Text;
using iTextSharp.text.html.simpleparser;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Microsoft.Reporting.WebForms;
using System.Data.Entity.Core.Objects;
using System.Configuration;
using System.Threading;

namespace Proptiwise.Controllers
{
    [RouteArea("Tenant")]
    [Route("{action=TenantHome}")]
    [Route("{action}/{id=value}")]
    public class TenantController : Controller
    {
        dbrealestateEntities db = new dbrealestateEntities();
        static int cmd = 0;

        //
        // GET: /Tenant/TenantHome
        public ActionResult TenantHome()
        {
            return View();
        }
        // GET: /Tenant/MyAccountTenant
        [SessionExpire]
        public ActionResult MyAccountTenant()
        {                     
            Int64 id = 0;
            if (Session["TenantId"] != null)
            {
                id = Convert.ToInt64(Session["TenantId"]);
                var data = db.Tenants.Where(t => t.Tenant_Id == id).FirstOrDefault();
                return View(data);
            }
            else
                return RedirectToAction("Login");
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult MyAccountTenant(Int64? id, TenantModel tenant, string Command, HttpPostedFileBase Signature, HttpPostedFileBase Image)
        {
            id = 0;
            //if (ModelState.IsValid)
            //{
                if (Session["TenantId"] != null)
                {
                    id = Convert.ToInt64(Session["TenantId"]);
                    var data = db.Tenants.Where(t => t.Tenant_Id == id).FirstOrDefault();
                    if (Command == "ChangePassword")
                    {
                        if (ModelState.IsValid)
                        {
                        if (tenant.Password == tenant.ConfirmPassword)
                        {                           
                            data.Password = tenant.Password;
                            db.SaveChanges();
                            Session["Tenant"] = id;
                            Session["TenantId"] = data.Tenant_Id;
                            TempData["MyAccountMessage"] = "Password Changed Successfully";
                        }
                        else
                        {
                            TempData["MyAccountMessage"] = "Password and ConfirmPassword must be Equal";
                        }
                    }
                    else
                        return View(data);
                    }
                    else if (Command == "uploadsign")
                    {

                        string pic = "";
                        if (tenant.Signature != "" && tenant.Signature != null)
                        {                           
                            if (Signature != null)
                            {
                                Signature.SaveAs(Server.MapPath("~/TenancyAgreementDoc/" + Signature.FileName));
                                pic = Signature.FileName;
                            }
                            else
                                pic = "";
                            data.Signature = pic;
                            db.SaveChanges();
                        }
                    }
                    string photo = "";
                    if (tenant.ProfilePhoto != "" && tenant.ProfilePhoto != null)
                    {                       
                        if (Image != null)
                        {
                            Image.SaveAs(Server.MapPath("~/Folders/ProfileImages/" + Image.FileName));
                            photo = Image.FileName;
                        }
                        else
                            photo = "";
                        data.ProfilePhoto = photo;
                        db.SaveChanges();
                    }
                    return RedirectToAction("MyAccountTenant");
                }
                else
                    return RedirectToAction("Login");
            //}
            //    else
            //    return View();
           
        }
	    
        //Edit Profile
        [SessionExpire]
        public ActionResult EditProfileTenant()
        {
            if (Session["TenantId"] != null)
            {
                Int64 id = Convert.ToInt64(Session["TenantId"]);
                var data = db.Tenants.Where(t => t.Tenant_Id == id).FirstOrDefault();
                ViewBag.email = data.Email;
                ViewBag.country = data.Country;               
                return View(data);
            }
            else
                return RedirectToAction("Login");
        }

        [HttpPost]
        [SessionExpire]
        public ActionResult EditProfileTenant(User t1, Tenant tnt, int? MaritalStatus, short? pets, short? children, HttpPostedFileBase ImageName, HttpPostedFileBase Signature)
        {           
            string profileImage = "";
            string signature = "";

            if (Session["TenantId"] != null)
            {
                if (ImageName != null)
                {
                    string path = Server.MapPath("~/Folders/ProfileImages/");

                    if (ImageName.ContentLength > 0)
                    {
                        ImageName.SaveAs(path + ImageName.FileName);
                    }
                    profileImage += ImageName.FileName + ",";

                }
                if (Signature != null)
                {
                    string path = Server.MapPath("~/Folders/ProfileImages/");

                    if (Signature.ContentLength > 0)
                    {
                        Signature.SaveAs(path + Signature.FileName);
                    }
                    signature += Signature.FileName + ",";
                }

                Int64 id = Convert.ToInt64(Session["TenantId"]);
                var data = db.Tenants.Where(t => t.Tenant_Id == id).FirstOrDefault();
                if (ImageName != null)
                    data.ProfilePhoto = profileImage.TrimEnd(',');
                else
                    data.ProfilePhoto = data.ProfilePhoto;
                if (Signature != null)
                    data.Signature = signature.TrimEnd(',');
                else
                    data.Signature = data.Signature;
                data.Password = data.Password;
                data.Email = data.Email;
                data.FirstName = data.FirstName;
                data.LastName = data.LastName;
                data.MiddleName = data.MiddleName;
                data.Cellular = tnt.Cellular;
                data.Gender = tnt.Gender;
                data.Dateofbirth = tnt.Dateofbirth;
                data.Telephone = tnt.Telephone;
                data.Cellular = tnt.Cellular;
                data.Email = data.Email;
                data.BuildingName = tnt.BuildingName;
                data.UnitNumber = tnt.UnitNumber;
                data.Suberb = tnt.Suberb;
                data.Province = tnt.Province;
                data.StreetAddress = tnt.StreetAddress;
                data.Town = tnt.Town;
                data.City = tnt.City;
                //  data.Country = tnt.;
                data.Zip = tnt.Zip;
                data.State = tnt.State;
                data.Telephone = tnt.Telephone;
                data.MaritalStatus = MaritalStatus;
                data.PostCode = tnt.PostCode;
                data.IfVisa = tnt.IfVisa;
                data.Pets = pets;
                data.ifPets = tnt.ifPets;
                data.Children = children;
                data.IfChildren = tnt.IfChildren;
                data.DriverLicense = tnt.DriverLicense;
                data.PassportNo = tnt.PassportNo;
                data.Dateofbirth = tnt.Dateofbirth;
                data.SocialSecurity = tnt.SocialSecurity;
                // data.leasestatus = 0;
                //  data.Landlord_id = lid;
                db.SaveChanges();
                return RedirectToAction("MyAccountTenant");
            }
            else
                return RedirectToAction("Login","Front",new { area = "" });
        }
       
        // Tenant/Logout
        public ActionResult TenantLogout()
        {
            Session.Remove("TenantId");
            Session.RemoveAll();
            return RedirectToAction("Login", "Front", new { area = "" });
        }

        // Tenant/Messages

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
        public ActionResult TMessages(int? page, string tab)
        {
            SortMessages();

            if (Session["EmailId1"] != null)
            {
                int pageSize = 2;
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
        [HttpPost]
        public ActionResult TMessages(int? page, string tab, string SortByInbox, string SortBySent)
        {
            SortMessages();
            ViewBag.SortByInbox = SortByInbox;
            ViewBag.SortBySent = SortBySent;
            if (Session["EmailId1"] != null)
            {
                int pageSize = 2;
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
        public ActionResult _TMessagesInbox()
        {
            return View();
        }
        [SessionExpire]
        public ActionResult _TMessagesSent()
        {
            return View();
        }
        [SessionExpire]
        public ActionResult TMessage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TMessage(tbMessage msg)
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
                return RedirectToAction("TMessages");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [SessionExpire]
        public ActionResult request(int? id)
        {
            //Session["TenantId"] = 1021;
            if (Session["TenantId"] != null)
            {
                long? tid = Convert.ToInt32(Session["TenantId"]);
                var tnt = db.Tenants.Where(p => p.Tenant_Id == tid ).FirstOrDefault();
                RequestView rv = new RequestView();
                rv.FirstName = tnt.FirstName;
                rv.LastName = tnt.LastName;
                rv.Phone = tnt.Cellular;
                var prp = db.Properties.Where(p => p.Property_Id == id ).FirstOrDefault();
                ViewBag.prp = prp;
                return View(rv);
            }
            else
                return View("Login");
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult request(int? id, RequestView rv)
        {
            //Session["TenantId"] = 1021;
            if (Session["TenantId"] != null)
            {
                long? ladloradid = db.Properties.Where(p => p.Property_Id == id).Select(p => p.Landlord_Id ).FirstOrDefault();
                RequestView rvs = new RequestView();
                rvs.FirstName = rv.FirstName;
                rvs.LastName = rv.LastName;
                rvs.Phone = rv.Phone;
                rvs.PreferredDate = rv.PreferredDate;
                rvs.PreferredTime = rv.PreferredTime;
                rvs.text = rv.text;
                rvs.LandlordId = ladloradid;
                rvs.PropertyId = id;
                rvs.TenantId = Convert.ToInt32(Session["TenantId"]);
                rvs.Approve = 0;
                rvs.RequestStatus = 1;
                db.RequestViews.Add(rvs);
                db.SaveChanges();
                var rd = db.RequestViews.OrderByDescending(p => p.ReqviewId).Take(1).FirstOrDefault();
                TenancyDetail td = new TenancyDetail();
                td.RequestViewingId = rd.ReqviewId;
                td.LandlordId = ladloradid;
                td.PropertyId = id;
                td.TenantId = Convert.ToInt32(Session["TenantId"]);
                db.TenancyDetails.Add(td);
                db.SaveChanges();
                var prp = GetPropertyDetails(id);
                TempData["RequestMessage"] = "Request Sent Successfully";               
                return View();
                // return Json(new { success = true });
            }
            else
                return View("Login");
        }
        [SessionExpire]

        public ActionResult RequestViewTenants()
        {
            //Session["TenantId"] = 1021;
            long? id = 0;
            if (Session["TenantId"] != null)
            {
                
                id = Convert.ToInt64(Session["TenantId"]);
                var data = db.RequestViews.Where(p => p.TenantId == id && p.Approve==0).OrderByDescending(p => p.ReqviewId).ToList();
                ViewBag.Pending = data;
                var data1 = db.RequestViews.Where(p => p.TenantId == id && p.Approve == 1  ).OrderByDescending(p => p.ReqviewId).ToList();
                ViewBag.accepted = data1;
                var data2 = db.RequestViews.Where(p => p.TenantId == id && p.Approve == 2 ).OrderByDescending(p => p.ReqviewId).ToList();
                ViewBag.rejected = data2;
                //var data3 = db.ViewingPurposals.Where(p => p.TenantId == id && p.Approve == 0).ToList();
                //ViewBag.PendingProposal = data3;
                //var data4 = db.RequestViews.Where(p => p.TenantId == id && p.Approve == 4).ToList();
                //ViewBag.acceptoffer = data4;
                //var data5 = db.RequestViews.Where(p => p.TenantId == id && p.Approve == 5).ToList();
                //ViewBag.rejectoffer = data5;
                return View();
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult AcceptTenancyAgreement(int? id)
        {
            var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
            long? id1 = data.PropertyId;
            ViewBag.TAID = data.TenancyAgreementData;
            var prp = db.Properties.Where(p => p.Property_Id == id1).FirstOrDefault();
            Session["propdata"] = prp;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult AcceptTenancyAgreement(int? id, HttpPostedFileBase SignatureTenant, TenancyAgreement ta, bool? IsAccepted)
        {
            if (IsAccepted == true)
            {
                if (SignatureTenant != null)
                {
                    var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
                    data.ApprovalStatus = 1;
                    data.DateApproved = DateTime.UtcNow.Date;
                    string pic = "";
                    if (SignatureTenant != null)
                    {
                        SignatureTenant.SaveAs(Server.MapPath("~/TenancyAgreementDoc/" + SignatureTenant.FileName));
                        pic = SignatureTenant.FileName;
                    }
                    else
                        pic = "";
                    data.SignatureTenant = pic;
                    db.SaveChanges();
                    var prp = db.Properties.Where(p => p.Property_Id == data.PropertyId).FirstOrDefault();
                    Session["propdata"] = prp;
                    TempData["TenantTenancyAgreement"] = "Agreement Accepted Successfully";
                    string body = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Tenancy Agreement Has been Aceepted for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
                    SendEmails.SendNotificationToTenant(data.User.Email, "Tenancy Agreement Accepted", body);
                    string body1 = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Accepted Tenancy Agreement of " + data.User.FirstName + " " + data.User.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
                    SendEmails.SendNotificationToLandlord(data.Tenant.Email, "Tenancy Agreement Accepted", body1);
                }
                else
                    TempData["TenantTenancyAgreement"] = "Upload Signature First";
            }
            else
            {
                TempData["TenantTenancyAgreement"] = "Accept Agreement Conditions to Continue";
            }
            return RedirectToAction("TenantTenancyAgreement");
        }
        [SessionExpire]
        public ActionResult RejectTenancyAgreement(int? id)
        {
            var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
            data.ApprovalStatus = 2;
            data.DateApproved = DateTime.UtcNow.Date;
            db.SaveChanges();
            string body = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Tenancy Agreement Has been Rejected for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            SendEmails.SendNotificationToTenant(data.User.Email, "Tenancy Agreement Rejected", body);
            string body1 = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Rejected Tenancy Agreement of " + data.User.FirstName + " " + data.User.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            SendEmails.SendNotificationToLandlord(data.Tenant.Email, "Tenancy Agreement Rejected", body1);
            TempData["TenantTenancyAgreement"] = "Agreement Rejected";
            return RedirectToAction("TenantTenancyAgreement");
        }
        //public FileStreamResult GetPDF(int ? id)
        //{
        //    var data = db.TenancyAgreements.Where(p => p.TenancyAgreementId == id).FirstOrDefault();
        //    FileStream fs = new FileStream(Server.MapPath("/TenancyAgreementDoc/"+data.TenancyAgreementData), FileMode.Open, FileAccess.Read);
        //    return File(fs, "application/pdf");
        //}
        [SessionExpire]
        public ActionResult CreateOffer(int? id)
        {
            long? id1 = db.RequestViews.Where(p => p.ReqviewId == id).Select(p => p.PropertyId).FirstOrDefault();
            var prp = db.Properties.Where(p => p.Property_Id == id1).FirstOrDefault();
            Session["propdata"] = prp;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult CreateOffer(int? id, tbl_offer tblofr)
        {
            ///Session["TenantId"] = 1021;
            if (Session["TenantId"] != null)
            {
                var q = db.RequestViews.Where(p => p.ReqviewId == id).FirstOrDefault();
                q.Status = 1;
                db.SaveChanges();
                tbl_offer tboffr = new tbl_offer();
                tboffr.Approval = 0;
                tboffr.PurposalStatus = 0;
                tboffr.OfferStatus = 1;
                tboffr.Askingprice = tblofr.Askingprice;
                tboffr.Date = DateTime.UtcNow.Date;
                tboffr.LandlordId = q.LandlordId;
                tboffr.PropertyId = q.PropertyId;
                tboffr.TenantId = Convert.ToInt32(Session["TenantId"]);
                tboffr.offer = tblofr.offer;
                db.tbl_offer.Add(tboffr);
                db.SaveChanges();
                var rd = db.tbl_offer.OrderByDescending(p => p.OfferId).Take(1).FirstOrDefault();
                TenancyDetail td = db.TenancyDetails.Where(p => p.TenantId == q.TenantId && p.LandlordId == q.LandlordId && p.PropertyId == q.PropertyId).FirstOrDefault();
                td.OfferId = rd.OfferId;
                db.SaveChanges();
                TempData["RequestViewTenantsMessage"] = "Congratulations! Your offer has been submitted successfully." + Environment.NewLine + " We hope that the landlord accepts your offer!";
                // return Json(new { success = true });
                return RedirectToAction("RequestViewTenants");
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult acceptoffer(int id)
        {
            var data = db.RequestViews.Where(p => p.ReqviewId == id).FirstOrDefault();
            data.Approve = 1;
            db.SaveChanges();//commented
            //string body = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Viewing Request Has been Aceepted for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            //SendEmails.SendNotificationToTenant(data.Tenant.Email, "Viewing Request Accepted", body);
            //string body1 = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Accepted Viewing Request of " + data.Tenant.FirstName + " " + data.Tenant.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            //SendEmails.SendNotificationToLandlord(data.User.Email, "Viewing Request Accepted", body1);
            return RedirectToAction("RequestViewTenants");

        }
        [SessionExpire]
        public ActionResult rejectoffer(int id)
        {
            var data = db.RequestViews.Where(p => p.ReqviewId == id).FirstOrDefault();
            data.Approve = 2;
            db.SaveChanges();//Commnetd
            //string body = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Viewing Request Has been Rejected for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            //SendEmails.SendNotificationToTenant(data.Tenant.Email, "Viewing Request Rejected", body);
            //string body1 = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Rejected Viewing Request of " + data.Tenant.FirstName + " " + data.Tenant.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            //SendEmails.SendNotificationToLandlord(data.User.Email, "Viewing Request Rejected", body1);
            return RedirectToAction("RequestViewTenants");
        }
        [SessionExpire]
        public ActionResult AcceptDepositRequest(int? id)
        {
            var data = db.Deposits.Where(p => p.DepositId == id).FirstOrDefault();
            data.DepositApproval = 1;
            db.SaveChanges();
            string body = "Hi " + data.User.FirstName + " " + data.User.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "Your Deposit Request Has been Aceepted for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            SendEmails.SendNotificationToTenant(data.User.Email, "Deposit Request Accepted", body);
            string body1 = "Hi " + data.Tenant.FirstName + " " + data.Tenant.LastName + "" + Environment.NewLine + "" + Environment.NewLine + "You Have Accepted Deposit Request of " + data.User.FirstName + " " + data.User.LastName + " for " + data.Property.PropertyName + " Property" + Environment.NewLine + "" + Environment.NewLine + "Thanks & Regards" + Environment.NewLine + "Proptiwise Team";
            SendEmails.SendNotificationToLandlord(data.Tenant.Email, "Deposit Request Accepted", body1);
            return RedirectToAction("TenantDepositRequests");
        }
        [SessionExpire]
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

        public ActionResult Onboarding()
        {
            return View();
        }
        //public ActionResult OffersTenant()
        //{
        //    //Session["TenantId"] = 1021;
        //    int? id = 0;
        //    if (Session["TenantId"] != null)
        //    {
        //        id = Convert.ToInt32(Session["TenantId"]);
        //        var data0 = db.tbl_offer.Where(p => p.TenantId == id && p.Approval != 4).ToList();
        //        ViewBag.AllOffers = data0;
        //        var data = db.tbl_offer.Where(p => p.TenantId == id && p.Approval == 0).ToList();
        //        ViewBag.Pending = data;
        //        var data1 = db.tbl_offer.Where(p => p.TenantId == id && p.Approval == 1).ToList();
        //        ViewBag.accepted = data1;
        //        var data2 = db.tbl_offer.Where(p => p.TenantId == id && p.Approval == 2).ToList();
        //        ViewBag.Rejected = data2;
        //        //var data3 = db.tbl_offer.Where(p => p.TenantId == id && p.PurposalStatus == 1 && p.Approval == 3).ToList();
        //        //ViewBag.purposal = data3;
        //        var data3 = db.Purposals.Where(p => p.TenantId == id && p.Approval == 0).ToList();
        //        ViewBag.PendingPurposal = data3;
        //        var data4 = db.Purposals.Where(p => p.TenantId == id && p.Approval == 1).ToList();
        //        ViewBag.acceptedPurposal = data4;
        //        var data5 = db.Purposals.Where(p => p.TenantId == id && p.Approval == 2).ToList();
        //        ViewBag.RejectedPurposal = data5;
        //        var data00 = db.Purposals.Where(p => p.TenantId == id).ToList();
        //        ViewBag.AllPurposal = data00;
        //        return View();
        //    }
        //    else
        //        return RedirectToAction("Login");
        //}
        //[SessionExpire]
        public ActionResult TenantTenancyAgreement()
        {
            //Session["TenantId"] = 1021;
            int? id = 0;
            if (Session["TenantId"] != null)
            {
                id = Convert.ToInt32(Session["TenantId"]);
                var data = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 0 && p.RenewStatus == 0).ToList();
                ViewBag.Pendingold = data;
                var data1 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 1 && p.RenewStatus == 0).ToList();
                ViewBag.acceptedold = data1;
                var data2 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 2 && p.RenewStatus == 0).ToList();
                ViewBag.Rejectedold = data2;
                var data3 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 0 && p.RenewStatus == 1).ToList();
                ViewBag.Pendingrenew = data3;
                var data4 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 1 && p.RenewStatus == 1).ToList();
                ViewBag.acceptedrenew = data4;
                var data5 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 2 && p.RenewStatus == 1).ToList();
                ViewBag.Rejectedrenew = data5;
                var data6 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 0 && p.RenewStatus == 2).ToList();
                ViewBag.Pendingcancel = data6;
                var data7 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 1 && p.RenewStatus == 2).ToList();
                ViewBag.acceptedcancel = data7;
                var data8 = db.TenancyAgreements.Where(p => p.TenantId == id && p.ApprovalStatus == 2 && p.RenewStatus == 2).ToList();
                ViewBag.Rejectedcancel = data8;
                return View();
            }
            else
                return RedirectToAction("Login");
        }

        [SessionExpire]
        public ActionResult applicationdetail(int? id)
        {
            int tid = Convert.ToInt32(Session["TenantId"]);
            var tnt = db.Tenants.Where(p => p.Tenant_Id == tid).FirstOrDefault();
            ViewBag.tnt = tnt;
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult applicationdetail(int? id, string Noage18, application_details ad, DateTime? date, string MaritalStatus, int? Children, int? Notolive)
        {
            string comma = "";
            Session["appid"] = id;
            for (int i = 1; i <= Convert.ToInt32(Noage18); i++)
            {
                string idd = "txtCustomer" + i;
                string customer = Request.Form[idd];
                comma = comma + customer + ",";

            }
            application_details app = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
            app.FullName = ad.FullName;
            app.FormerName = ad.FormerName;
            app.PhoneNumber = ad.PhoneNumber;
            app.MobileNumber = ad.MobileNumber;
            app.Email = ad.Email;
            app.DOB = date;
            app.MaritalStatus = MaritalStatus;
            app.Children = Children;
            app.Notolive = Notolive;
            app.Noage18 = Convert.ToInt32(Noage18);
            app.PeopleNames = comma.TrimEnd(',');
            app.Pets = ad.Pets;
            app.Smoker = ad.Smoker;
            app.FillStatus = 1;
            db.SaveChanges();
            var rd = db.application_details.OrderByDescending(p => p.ApplicationId).Take(1).FirstOrDefault();
            TenancyDetail td = db.TenancyDetails.Where(p => p.TenantId == app.TenantId && p.LandlordId == app.LandlordId && p.PropertyId == app.PropertyId).FirstOrDefault();
            td.ApplicationId = rd.ApplicationId;
            db.SaveChanges();

            TempData["applicationdetailMessage"] = "Application Submitted Successfully";
            return RedirectToAction("appdetailaddress", id);

        }
        [SessionExpire]
        public ActionResult appdetailaddress(int? id)
        {
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult appdetailaddress(application_details ad, DateTime? date1, DateTime? date2)
        {
            int id = Convert.ToInt32(Session["appid"]);            
                application_details app = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
                app.HouseNumber = ad.HouseNumber;
                app.StreetName = ad.StreetName;
                app.Town = ad.Town;
                app.City = ad.City;
                app.Country = ad.Country;
                app.PostCode = ad.PostCode;
                app.ReferenceName = ad.ReferenceName;
                app.ReferenceEmail = ad.ReferenceEmail;
                app.ReferneceNumber = ad.ReferneceNumber;
                app.Moveindate = date1;
                app.Moveout = date2;
                app.Address = ad.Address;
                db.SaveChanges();
                return RedirectToAction("appdetailEmployment", id);
                       
        }
        public ActionResult appdetailEmployment()
        {
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult appdetailEmployment(application_details ad, string Employmntstatus, string netduration, string additionalduration, string grossduration)
        {
            int id = Convert.ToInt32(Session["appid"]);
            application_details app = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
            app.Employmntstatus = Employmntstatus;
            app.EmployerName = ad.EmployerName;
            app.Phone = ad.Phone;
            app.EmpEmail = ad.EmpEmail;
            app.Position = ad.Position;
            app.LengthEmp = ad.LengthEmp;
            app.Emphousenum = ad.Emphousenum;
            app.Empstreetname = ad.Empstreetname;
            app.Emptown = ad.Emptown;
            app.EmpCity = ad.EmpCity;
            app.EmpCountry = ad.EmpCountry;
            app.EmpPostCode = ad.EmpPostCode;
            app.CPName = ad.CPName;
            app.CPNumber = ad.CPNumber;
            app.CPemail = ad.CPemail;
            app.NetIncome = ad.NetIncome;
            app.AddIncome = ad.AddIncome;
            app.GrossIncome = ad.GrossIncome;
            app.netduration = netduration;
            app.additionalduration = additionalduration;
            app.grossduration = grossduration;
            db.SaveChanges();
            return RedirectToAction("appdetailother", id);
        }
        [SessionExpire]
        public ActionResult appdetailother()
        {
            return View();
        }
        [HttpPost]
        [SessionExpire]
        public ActionResult appdetailother(application_details ad)
        {
            int id = Convert.ToInt32(Session["appid"]);
            application_details app = db.application_details.Where(p => p.ApplicationId == id).FirstOrDefault();
            app.countryagainst = ad.countryagainst;
            app.bankruptcy = ad.bankruptcy;
            app.criminalactivity = ad.criminalactivity;
            app.terroristactivity = ad.terroristactivity;
            app.Status = 1;
            db.SaveChanges();
            return RedirectToAction("TenantApplicationRequests");
        }
        [SessionExpire]
        public ActionResult TenantApplicationRequests()
        {
            //Session["TenantId"] = 1021;
            int? id = 0;
            if (Session["TenantId"] != null)
            {
                id = Convert.ToInt32(Session["TenantId"]);
                var data = db.application_details.Where(p => p.TenantId == id && p.Status == 0).ToList();
                ViewBag.Pending = data;
                var data1 = db.application_details.Where(p => p.TenantId == id && p.Status == 1).ToList();
                ViewBag.accepted = data1;
                var data4 = db.application_details.Where(p => p.TenantId == id && p.Status == 2).ToList();
                ViewBag.acceptedLandlord = data4;
                var data2 = db.application_details.Where(p => p.TenantId == id && p.Status == 3).ToList();
                ViewBag.Rejected = data2;
                var data3 = db.application_details.Where(p => p.TenantId == id && p.Status == 4).ToList();
                ViewBag.Deffered = data3;
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
      
        public ActionResult TenantDepositRequests()
        {
            //Session["TenantId"] = 1021;
            int? id = 0;
            if (Session["TenantId"] != null)
            {
                id = Convert.ToInt32(Session["TenantId"]);
                var data = db.Deposits.Where(p => p.TenantId == id && p.DepositApproval == 0).ToList();
                ViewBag.Pending = data;
                var data1 = db.Deposits.Where(p => p.TenantId == id && p.DepositApproval == 1).ToList();
                ViewBag.accepted = data1;
                var data2 = db.Deposits.Where(p => p.TenantId == id && p.DepositApproval == 2).ToList();
                ViewBag.Rejected = data2;
                return View();
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        //public ActionResult DepositRequest(int? id)
        //{
        //    if (Session["LandlordId"] != null)
        //    {
        //        var q = db.tbl_offer.Where(p => p.OfferId == id).FirstOrDefault();
        //        q.Approval = 4;
        //        db.SaveChanges();
        //        var amount = db.Properties.Where(p => p.Property_Id == q.PropertyId).Select(p => p.HoldingFee).FirstOrDefault();
        //        Deposit Depositdata = new Deposit();
        //        Depositdata.DepositApproval = 0;
        //        Depositdata.DepositAmount = amount;
        //        Depositdata.DateCreated = DateTime.UtcNow.Date;
        //        Depositdata.LandlordId = Convert.ToInt64(Session["LandlordId"]);
        //        Depositdata.PropertyId = q.PropertyId;
        //        Depositdata.TenantId = q.TenantId;
        //        Depositdata.DepositStatus = 0;
        //        Depositdata.OfferId = q.OfferId;
        //        db.Deposits.Add(Depositdata);
        //        db.SaveChanges();
        //        var rd = db.Deposits.OrderByDescending(p => p.DepositId).Take(1).FirstOrDefault();
        //        TenancyDetail td = db.TenancyDetails.Where(p => p.TenantId == q.TenantId && p.LandlordId == q.LandlordId && p.PropertyId == q.PropertyId && p.OfferId == q.OfferId).FirstOrDefault();
        //        td.DepositId = rd.DepositId;
        //        db.SaveChanges();
        //        return RedirectToAction("offerrequest");
        //    }
        //    else
        //        return RedirectToAction("Login");
        //}
        //[SessionExpire]
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
        public ActionResult showreq()
        {
            int? id = 0;
            if (Session["TenantId"] != null)
                id = Convert.ToInt32(Session["TenantId"]);
            var data = db.TenancyAgreements.Where(p => p.RenewStatus == 1 && p.TenantId == id && p.Status == 0 && p.ApprovalStatus == 0).FirstOrDefault();
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
        public ActionResult TenantMovingInRequests()
        {
            int? id = 0;
            if (Session["TenantId"] != null)
            {
                id = Convert.ToInt32(Session["TenantId"]);
                var data = db.MovingIns.Where(p => p.TenantId == id && p.ApprovalStatus == 0).ToList();
                ViewBag.Pending = data;
                var data1 = db.MovingIns.Where(p => p.TenantId == id && p.ApprovalStatus == 1).ToList();
                ViewBag.accepted = data1;
                var data2 = db.MovingIns.Where(p => p.TenantId == id && p.ApprovalStatus == 2).ToList();
                ViewBag.Rejected = data2;
                return View();
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult TenantInvoiceRequests()
        {
            int? id = 0;
            if (Session["TenantId"] != null)
            {
                id = Convert.ToInt32(Session["TenantId"]);
                var data = db.tbl_invoice.Where(p => p.TenantId == id && p.PaymentStatus == 0).ToList();
                ViewBag.Pending = data;
                var data1 = db.tbl_invoice.Where(p => p.TenantId == id && p.PaymentStatus == 1).ToList();
                ViewBag.accepted = data1;

                return View();
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult EditViewingRequest(int? id)
        {
            RequestView rv = db.RequestViews.Find(id);
            ViewBag.prp = db.Properties.Where(p => p.Property_Id == rv.PropertyId).FirstOrDefault();
            return View(rv);
        }
        [HttpPost]
        
        public ActionResult EditViewingRequest(int? id, RequestView rv)
        {
            //Session["TenantId"] = 1021;
            if (Session["TenantId"] != null)
            {
                // int? ladloradid = db.Properties.Where(p => p.Property_Id == id).Select(p => p.Landlord_Id).FirstOrDefault();
                RequestView rvs = db.RequestViews.Find(rv.ReqviewId);
                rvs.FirstName = rv.FirstName;
                rvs.LastName = rv.LastName;
                rvs.Phone = rv.Phone;
                rvs.PreferredDate = rv.PreferredDate;
                rvs.PreferredTime = rv.PreferredTime;
                rvs.text = rv.text;
                rvs.LandlordId = rvs.LandlordId;
                rvs.PropertyId = rvs.PropertyId;
                rvs.TenantId = Convert.ToInt32(Session["TenantId"]);
                rvs.Approve = 0;
                db.SaveChanges();
                TempData["RequestViewTenantsMessage"] = "Request Updated Successfully";
                return RedirectToAction("RequestViewTenants");
            }
            else
                return RedirectToAction("Login");
        }
        [SessionExpire]
        public ActionResult MyListProperties()
        {
            SortMylist();
            GetPropertiesList();
            return View();
        }
        //private void GetPropertiesList()
        //{
        //    Session["TenantId"] = 1021;
        //    long? id = Convert.ToInt32(Session["TenantId"]);
        //    var q = db.Mylists.Where(p => p.TenantId == id).ToList();
        //    List<Property> prplst = new List<Property>();

        //    foreach (var data in q)
        //    {
        //        Property prp = new Property();
        //        var pn = db.tb_PropertyNotes.Where(p => p.PropertyId == data.PropertyId).ToList();
        //        prp = new JavaScriptSerializer().Deserialize<Property>(data.ListDetails.ToString());
        //        //string prptypename = db.PropertyTypes.Where(p => p.PropertyType_Id == prp.PropertyTypeId).Select(p => p.PropertyTypeName).FirstOrDefault();
        //        //prp.PropertyTypeName = prptypename;
        //        string prpstylename = db.PropertyStyles.Where(p => p.Propertystyle_Id == prp.PropertyStyleId).Select(p => p.Style_Name).FirstOrDefault();
        //        prp.PropertyStyle.Style_Name = prpstylename;                
        //        prp.tb_PropertyNotes = pn;
        //        prplst.Add(prp);
        //    }
        //    Session["ListData"] = prplst;
        //}
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
                prp.Propertystylename = prpStyle_Name;
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
       
        //public ActionResult SaveList(long? id)
        //{
        //    cmd = 1;
        //    Session["TenantId"] = 1021;
        //    long? tid = Convert.ToInt32(Session["TenantId"]);
        //    var data = db.Properties.Where(p => p.Property_Id == id).FirstOrDefault();
        //    db.Configuration.LazyLoadingEnabled = false;
        //    db.Configuration.ProxyCreationEnabled = false;
        //    string jsonresult = JsonConvert.SerializeObject(data);
        //    Mylist mylist = new Mylist();
        //    mylist.ListName = "";
        //    mylist.ListDetails = jsonresult;
        //    mylist.TenantId = tid;
        //    mylist.PropertyId = id;
        //    mylist.UpdatedOn = DateTime.UtcNow.Date;
        //    db.Mylists.Add(mylist);
        //    db.SaveChanges();

        //    return RedirectToAction("PropertyListing");
        //}

        [SessionExpire]
        private Property GetPropertyDetails(int? id)
        {
            var prp = db.Properties.Where(p => p.Property_Id == id).FirstOrDefault();
            var prpImages = db.PropertyImages.Where(p => p.PropertyId == id).ToList();
            ViewBag.prpImages = prpImages;
            ViewBag.prp = prp;
            return prp;
        }

        [SessionExpire]
        public ActionResult ViewMessage(int id)
        {
            var q = db.tbMessages.Where(p => p.MessageId == id).FirstOrDefault();
            q.Status = 1;
            db.SaveChanges();
            return View(q);
        }
        [SessionExpire]
        public ActionResult ViewMessage1(int id)
        {
            var q = db.tbMessages.Where(p => p.MessageId == id).FirstOrDefault();
            return View("ViewMessage", q);
        }
        [SessionExpire]
        public ActionResult ReplyMessage(int id)
        {
            return View();
        }
        [SessionExpire]
        [HttpPost]
        public ActionResult ReplyMessage(int id, tbMessage msg)
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
            return RedirectToAction("TMessages");
            //return Json(new { success = true });
        }

    }
}