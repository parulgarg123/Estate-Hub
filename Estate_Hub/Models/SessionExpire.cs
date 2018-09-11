using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Proptiwise.Models
{
    public class SessionExpire : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;
            HttpCookie UserDetailsCookie = HttpContext.Current.Request.Cookies["EmailIdCookies"];
            if (UserDetailsCookie != null)
            {
                HttpContext.Current.Session["EmailId1"] = UserDetailsCookie["EmailIdCookies"];
            }
            if (HttpContext.Current.Session["EmailId1"] == null)
            {
                FormsAuthentication.SignOut();
                string redirectTo = "/Login";
                if (!string.IsNullOrEmpty(context.Request.RawUrl))
                {
                    //  redirectTo = string.Format("~/Front/Login");
                    redirectTo = string.Format("~/Login?ReturnUrl={0}", HttpUtility.UrlEncode(context.Request.RawUrl));
                }
                filterContext.HttpContext.Response.Redirect(redirectTo, true);
            }
            else
            {
                TimeSpan ts = new TimeSpan(0, 30, 0);
                FormsAuthentication.Timeout.Add(ts);
            }
        }
    }   
}