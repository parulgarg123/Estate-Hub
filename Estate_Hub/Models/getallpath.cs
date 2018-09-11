using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Proptiwise.Models
{
    public class getallpath
    {
        public static string getpath = ConfigurationManager.AppSettings["pathurl"];
        public static string currency { get; set; }
    }
}