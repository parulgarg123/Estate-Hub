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
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Property
    {
        public Property()
        {
            this.application_details = new HashSet<application_details>();
            this.damage_intventory = new HashSet<damage_intventory>();
            this.Deposits = new HashSet<Deposit>();
            this.Faults = new HashSet<Fault>();
            this.Inspections = new HashSet<Inspection>();
            this.InventoryImages = new HashSet<InventoryImage>();
            this.MovingIns = new HashSet<MovingIn>();
            this.Mylists = new HashSet<Mylist>();
            this.paymentHistories = new HashSet<paymentHistory>();
            this.Purposals = new HashSet<Purposal>();
            this.tb_PropertyNotes = new HashSet<tb_PropertyNotes>();
            this.tbl_inventory = new HashSet<tbl_inventory>();
            this.tbl_invoice = new HashSet<tbl_invoice>();
            this.tbl_offer = new HashSet<tbl_offer>();
            this.TenancyAgreements = new HashSet<TenancyAgreement>();
            this.TenancyDetails = new HashSet<TenancyDetail>();
            this.Tenants = new HashSet<Tenant>();
            this.ViewingPurposals = new HashSet<ViewingPurposal>();
            this.tbl_lease = new HashSet<tbl_lease>();
            this.RequestViews = new HashSet<RequestView>();
            this.tbl_LeaseCredits = new HashSet<tbl_LeaseCredits>();
            this.tbl_othercharges = new HashSet<tbl_othercharges>();
        }

        public long Property_Id { get; set; }
        public string PropertyName { get; set; }
        public Nullable<int> PropertyStyleId { get; set; }
        public Nullable<bool> Parking_Id { get; set; }
        public Nullable<bool> Heating_Id { get; set; }
        public Nullable<bool> AirConditioning_Id { get; set; }
        public Nullable<bool> Flooring_Id { get; set; }
        public Nullable<int> BuildingId { get; set; }
        public string Size { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public Nullable<int> PropertyTypeId { get; set; }
        public Nullable<int> SqFt { get; set; }
        public string FeaturesAndAmenities { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string Apt_Suite_Unit { get; set; }
        public string Street { get; set; }
        public string street_name { get; set; }
        public string street_type { get; set; }
        public string street_direction { get; set; }
        public Nullable<short> YearBuilt { get; set; }
        public Nullable<System.DateTime> DateAvailable { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<short> Bedroom { get; set; }
        public string Furnished { get; set; }
        public Nullable<System.DateTime> DateAdded { get; set; }
        public Nullable<short> Bathroom { get; set; }
        public Nullable<short> toilet { get; set; }
        public Nullable<short> studyRoom { get; set; }
        public Nullable<short> UtilityRoom { get; set; }
        public Nullable<short> Kitchen { get; set; }
        public Nullable<short> Garage { get; set; }
        public Nullable<bool> SmokersAllowed { get; set; }
        public Nullable<bool> PetsAllowed { get; set; }
        public Nullable<bool> Parking { get; set; }
        public Nullable<bool> Garden { get; set; }
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Nullable<long> Landlord_Id { get; set; }
        public Nullable<int> leasestatus { get; set; }
        public Nullable<int> Archievestatus { get; set; }
        public Nullable<long> LeasedTo { get; set; }
        public Nullable<System.DateTime> LeasedEnd { get; set; }
        public string Comment { get; set; }
        public string listtitle { get; set; }
        public string lsContactName { get; set; }
        public string lsContactPhone { get; set; }
        public string lsContactEmail { get; set; }
        public string VirtualTourUrl { get; set; }
        public Nullable<double> DepositReq { get; set; }
        public Nullable<double> ApplicationFee { get; set; }
        public Nullable<short> RentFreq { get; set; }
        public Nullable<System.DateTime> AvailableOn { get; set; }
        public Nullable<System.DateTime> VacancyDate { get; set; }
        public Nullable<double> LateFee { get; set; }
        public Nullable<int> Days { get; set; }
        public Nullable<int> HoldingFee { get; set; }
        public Nullable<double> IfHoldingFee { get; set; }
        public Nullable<double> MinimumRent { get; set; }
        public string PropertyPhotos { get; set; }
        public string BuildingName { get; set; }
        public string Zip { get; set; }
        public string Town { get; set; }
        public string UnitNumber { get; set; }
        public string Suberb { get; set; }
        public string Province { get; set; }
        public string StreetAddress { get; set; }
        public string County { get; set; }
        [NotMapped]
        public string Propertystylename { get; set; }
        public string DataTextFieldLabel
        {
            get
            {
                return string.Format("{0} ({1})", AddressLine1, Country);
            }
        }
        public virtual ICollection<application_details> application_details { get; set; }
        public virtual Building Building { get; set; }
        public virtual ICollection<damage_intventory> damage_intventory { get; set; }
        public virtual ICollection<Deposit> Deposits { get; set; }
        public virtual ICollection<Fault> Faults { get; set; }
        public virtual ICollection<Inspection> Inspections { get; set; }
        public virtual ICollection<InventoryImage> InventoryImages { get; set; }
        public virtual ICollection<MovingIn> MovingIns { get; set; }
        public virtual ICollection<Mylist> Mylists { get; set; }
        public virtual ICollection<paymentHistory> paymentHistories { get; set; }
        public virtual PropertyStyle PropertyStyle { get; set; }
        public virtual PropertyType PropertyType { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Purposal> Purposals { get; set; }
        public virtual ICollection<tb_PropertyNotes> tb_PropertyNotes { get; set; }
        public virtual ICollection<tbl_inventory> tbl_inventory { get; set; }
        public virtual ICollection<tbl_invoice> tbl_invoice { get; set; }
        public virtual ICollection<tbl_offer> tbl_offer { get; set; }
        public virtual ICollection<TenancyAgreement> TenancyAgreements { get; set; }
        public virtual ICollection<TenancyDetail> TenancyDetails { get; set; }
        public virtual ICollection<Tenant> Tenants { get; set; }
        public virtual ICollection<ViewingPurposal> ViewingPurposals { get; set; }
        public virtual ICollection<tbl_lease> tbl_lease { get; set; }
        public virtual ICollection<RequestView> RequestViews { get; set; }
        public virtual ICollection<tbl_LeaseCredits> tbl_LeaseCredits { get; set; }
        public virtual ICollection<tbl_othercharges> tbl_othercharges { get; set; }
    }
}
