using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proptiwise.Models
{
    public static class CalendarHelpers
    {
        //Google Calender

        public static string GoogleCalendar(this HtmlHelper helper, string linkText, string what, DateTime start, DateTime? end, string description, string location, string websiteName, string websiteAddress, string attributes)
        {
            //parse dates
            var dates = start.ToString("yyyyMMddTHHmmssZ");
            if (end.HasValue && end > start)
            {
                dates += "/" + end.Value.ToString("yyyyMMddTHHmmssZ");
            }
            else
            {
                dates += "/" + start.ToString("yyyyMMddTHHmmssZ");
            }

            var path = string.Format("http://www.google.com/calendar/event?action=TEMPLATE&text={0}&dates={1}&details={2}&location={3}&trp=false&sprop={4}&sprop=name:{5}",
                                            what,
                                            dates,
                                            description,
                                            location,
                                            websiteName,
                                            websiteAddress);

            var calendar = string.Format("<a href='{0}' target='_blank' {1}>{2}</a>",
                                            HttpUtility.UrlPathEncode(path),
                                            helper.AttributeEncode(attributes),
                                            linkText);

            return calendar;
        }

        public static string GoogleCalendar(this HtmlHelper helper, string linkText, string what, DateTime start, DateTime? end, string description, string location, string websiteName, string websiteAddress)
        {
            return GoogleCalendar(helper, linkText, what, start, end, description, location, websiteName, websiteAddress, "");
        }

        public static string GoogleCalendar(this HtmlHelper helper, string linkText, string what, DateTime start, string description)
        {
            return GoogleCalendar(helper, linkText, what, start, null, description, "", "", "", "");
        }
    }
}