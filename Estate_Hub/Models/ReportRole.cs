//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proptiwise.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReportRole
    {
        public ReportRole()
        {
            this.UserReportsPermissions = new HashSet<UserReportsPermission>();
        }
    
        public int ReportRoleId { get; set; }
        public string ReportRoleName { get; set; }
        public string ReportRoleUrl { get; set; }
        public string ReportDescription { get; set; }
    
        public virtual ICollection<UserReportsPermission> UserReportsPermissions { get; set; }
    }
}
