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
    
    public partial class Purposal
    {
        public Purposal()
        {
            this.TenancyDetails = new HashSet<TenancyDetail>();
        }
    
        public int PurposalId { get; set; }
        public Nullable<long> LandlordId { get; set; }
        public Nullable<long> TenantId { get; set; }
        public Nullable<long> PropertyId { get; set; }
        public Nullable<double> Offerprice { get; set; }
        public Nullable<double> purposal1 { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Approval { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> offerid { get; set; }
    
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<TenancyDetail> TenancyDetails { get; set; }
        public virtual User User { get; set; }
        public virtual Property Property { get; set; }
    }
}