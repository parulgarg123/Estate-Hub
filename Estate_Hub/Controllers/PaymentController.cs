using Proptiwise.Models;
using Proptiwise.ViewModels.Stripe;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
namespace Proptiwise.Controllers
{
    [RouteArea("Payment")]
    [Route("{action=Buy}")]
    [Route("{action}/{id=value}")]
    public class PaymentController : Controller
    {
        //
        // GET: /Payment/Registration
        dbrealestateEntities db = new dbrealestateEntities();
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
        public ActionResult Buy(int? id)
        {
            ViewBag.planid = id;
            return View();
        }
        // GET: Payment
        [HttpPost]

        public ActionResult Buy(FormCollection form, int? id)
        {
            int? planid = 0;
            if (Session["Plan"] != null)
                planid = Convert.ToInt32(Session["Plan"]);
            var plandata = db.Plans.Where(p => p.PlanID == planid).FirstOrDefault();
            string selectedPlan = "";
            if (plandata.PlanName == "Starter")
                selectedPlan = "30 Days";
            else if (plandata.PlanName == "Standard")
                selectedPlan = "60 Days";
            else if (plandata.PlanName == "Main")
                selectedPlan = "90 Days";
            long? idd = Convert.ToInt64(Session["LandlordId"]);
            var data = db.Users.Where(p => p.UserID == idd).FirstOrDefault();
            var customerToken = form["stripeToken"];
            var buyerModel = new BuyerViewModel();
            buyerModel.Email = data.Email;
            buyerModel.FullName = data.FirstName + " " + data.LastName;
            //  UpdateModel(buyerModel, form.ToValueProvider());
            if (data.StripeCustomerId != "")
            {
                // create customer
                var customerOptions = new StripeCustomerCreateOptions
                {
                    Email = buyerModel.Email,
                    Description = buyerModel.FullName,
                    SourceToken = customerToken
                };
                var service = new StripeCustomerService();
                var customer = service.Create(customerOptions);
                Decimal? amt = db.Plans.Where(p => p.PlanID == id).Select(p => p.PriceMonthly).FirstOrDefault();

                var myCharge = new StripeChargeCreateOptions
                {
                    Amount = Convert.ToInt32(amt) * 100,
                    Currency = "usd",
                    CustomerId = customer.Id
                };
                //var chargeService = new StripeChargeService();
                //var stripeCharge = chargeService.Create(myCharge);   
                var subscriptionService = new StripeSubscriptionService();
                StripeSubscription stripeSubscription = subscriptionService.Create(myCharge.CustomerId, selectedPlan);
                Transaction td = new Transaction();
                td.TransId = stripeSubscription.Id;
                td.AmountPaid = ((stripeSubscription.StripePlan.Amount) / 100).ToString();
                td.DateAdded = DateTime.UtcNow.Date;
                td.LandlordId = idd;
                db.Transactions.Add(td);
                db.SaveChanges();
                data.UserStatusID = 2;
                data.StripeCustomerId = customer.Id;
                data.RoleId = 1;
                db.SaveChanges();
                Session["UserRoleId"] = data.RoleId;
                Session["UserRole"] = data.UserRole1.RoleName;
                Session["EmailId"] = data.Email;
                Session["EmailId1"] = data.Email;
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
            return RedirectToAction("Dashboard", "Front", new { area = "" });
        }
	}
}