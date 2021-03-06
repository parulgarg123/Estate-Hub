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
    
    public partial class Fault
    {
        public int FaultId { get; set; }
        public string FaultDescription { get; set; }
        public Nullable<int> FaultTypeId { get; set; }
        public Nullable<System.DateTime> FaultDate { get; set; }
        public Nullable<int> Priority { get; set; }
        public string Notes { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<long> LandlordId { get; set; }
        public Nullable<long> PropertyId { get; set; }
        public Nullable<long> TenantId { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> RepairStatus { get; set; }
        public Nullable<double> RepairAmount { get; set; }
        public Nullable<int> RepairPaymentStatus { get; set; }
        public Nullable<int> AgreeWithAmount { get; set; }
        public Nullable<System.DateTime> VisitDate { get; set; }
        public string VisitTime { get; set; }
        public Nullable<double> MaintenanceCost { get; set; }
        public Nullable<short> AcceptCost { get; set; }
        public Nullable<short> AcceptTime { get; set; }
        public string SupplierNotes { get; set; }
        public Nullable<int> JobStatus { get; set; }
        public string JobStatusReason { get; set; }
        public Nullable<System.DateTime> VisitDateOffer { get; set; }
        public string VisitTimeOffer { get; set; }
        public Nullable<int> AcceptOfferTime { get; set; }
        public string RejectCommentByProvider { get; set; }
        public string RejectCommentByTenant { get; set; }
        public Nullable<double> TenantMaintainanceCost { get; set; }
        public string ProposalComments { get; set; }
        public string ProposalProviderComments { get; set; }
        public Nullable<int> AcceptSchedule { get; set; }
        public Nullable<int> ServiceProviderId { get; set; }
        public string photos { get; set; }
    
        public virtual FaultType FaultType { get; set; }
        public virtual Serviceprovider Serviceprovider { get; set; }
        public virtual service service { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual User User { get; set; }
        public virtual Property Property { get; set; }
    }
}
