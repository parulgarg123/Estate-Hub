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
    
    public partial class TenancyDetail
    {
        public int TenancyDetailId { get; set; }
        public Nullable<long> TenantId { get; set; }
        public Nullable<long> LandlordId { get; set; }
        public Nullable<long> PropertyId { get; set; }
        public Nullable<int> RequestViewingId { get; set; }
        public Nullable<int> ProposalViewingId { get; set; }
        public Nullable<int> OfferId { get; set; }
        public Nullable<int> ProposalOfferId { get; set; }
        public Nullable<int> DepositId { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> TenancyAgrrementId { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<int> transactionId { get; set; }
        public Nullable<int> PaymentId { get; set; }
        public Nullable<long> LeaseId { get; set; }
        public Nullable<int> MovingInId { get; set; }
    
        public virtual Deposit Deposit { get; set; }
        public virtual Purposal Purposal { get; set; }
        public virtual tbl_offer tbl_offer { get; set; }
        public virtual TenancyAgreement TenancyAgreement { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ViewingPurposal ViewingPurposal { get; set; }
        public virtual User User { get; set; }
        public virtual Property Property { get; set; }
        public virtual RequestView RequestView { get; set; }
        public virtual application_details application_details { get; set; }
    }
}
