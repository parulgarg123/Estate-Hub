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
    
    public partial class MovingIn
    {
        public int MovingInId { get; set; }
        public Nullable<System.DateTime> MoveInDate { get; set; }
        public Nullable<bool> WelcomePack { get; set; }
        public Nullable<bool> Keys { get; set; }
        public Nullable<short> SetsofKeys { get; set; }
        public Nullable<bool> PropertyManual { get; set; }
        public Nullable<bool> WhiteGoodManual { get; set; }
        public Nullable<bool> BoilerManual { get; set; }
        public Nullable<bool> UtilitiesInformation { get; set; }
        public Nullable<bool> SubmitMeterReading { get; set; }
        public Nullable<bool> Others { get; set; }
        public Nullable<long> PropertyId { get; set; }
        public Nullable<long> TenantId { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<long> LandlordId { get; set; }
        public Nullable<short> Status { get; set; }
        public Nullable<short> ApprovalStatus { get; set; }
        public string Comments { get; set; }
    
        public virtual Tenant Tenant { get; set; }
        public virtual User User { get; set; }
        public virtual Property Property { get; set; }
    }
}
