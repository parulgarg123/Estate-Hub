using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proptiwise.Models
{
    public class DocumentPdfModel
    {
     public    UserDocument userdocument { get; set; }
     public  Tenant tenant { get; set; }
     public User user { get; set; }
     public Property property { get; set; }
     public tbl_lease tbllease { get; set; }
    }
}