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
    
    public partial class gettenantdetails_Result
    {
        public int TenancyAgreementId { get; set; }
        public Nullable<System.DateTime> FromAgreementDate { get; set; }
        public Nullable<System.DateTime> ToAgreementDate { get; set; }
        public string Duration { get; set; }
        public string TenancyAgreementData { get; set; }
        public string Signatures { get; set; }
        public string SignatureTenant { get; set; }
        public Nullable<System.DateTime> RenewalDate { get; set; }
        public Nullable<long> TenantId { get; set; }
        public Nullable<long> LandlordId { get; set; }
        public Nullable<long> PropertyId { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> ApprovalStatus { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<System.DateTime> DateApproved { get; set; }
        public Nullable<System.DateTime> lastmonthdate { get; set; }
        public Nullable<int> RenewStatus { get; set; }
        public Nullable<System.DateTime> Checkoutdate { get; set; }
        public Nullable<int> Paymentstatus { get; set; }
        public Nullable<int> Sendstatus { get; set; }
        public string Cancelcomment { get; set; }
    }
}