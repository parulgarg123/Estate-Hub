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
    
    public partial class UserReportsPermission
    {
        public int UserReportsPermissionsId { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<int> ReportsRoleId { get; set; }
        public Nullable<int> ReportPermissionStatus { get; set; }
    
        public virtual ReportRole ReportRole { get; set; }
        public virtual User User { get; set; }
    }
}