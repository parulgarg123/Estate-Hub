using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proptiwise.Models
{
    public class TenantEmailData
    {

        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Cellular { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PropertyName { get; set; }

        public Nullable<double> Balance { get; set; }
        public Nullable<System.DateTime> LastPmtOn { get; set; }
        public Nullable<System.DateTime> Nextpmtdue { get; set; }
        public Nullable<System.DateTime> LeaseEnds { get; set; }
        public int? leasestatus { get; set; }
        public int? BuildingId { get; set; }
        public int? PropertyId { get; set; }
        public int? TenantId { get; set; }
    }

    public class TenantEmailDataOthers
    {
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Cellular { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PropertyName { get; set; }
        public string BuildingName { get; set; }
        public Nullable<double> Balance { get; set; }
        public Nullable<System.DateTime> LastPmtOn { get; set; }
        public Nullable<System.DateTime> Nextpmtdue { get; set; }
        public int? BuildingId { get; set; }
        public long? PropertyId { get; set; }
        public long? TenantId { get; set; }

    }
    public class ApplicantEmailData
    {
        public string Name { get; set; }
        public Nullable<System.DateTime> Received_On { get; set; }
        public string DesiredProperty { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Status { get; set; }
        public int? BuildingId { get; set; }
        public int? PropertyId { get; set; }
        public int? TenantId { get; set; }

    }
    public class PropertyDataLease
    {
        public string PropertyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Size { get; set; }
        public string LeasedTo { get; set; }
        //public string FirstName { get; set; }    
        //public Nullable<int> LeasedTo { get; set; }
        public Nullable<System.DateTime> LeasedEnd { get; set; }
        public string BuildingName { get; set; }
        public int? BuildingId { get; set; }
        public long? PropertyId { get; set; }
        public long? TenantId { get; set; }
    }

    public class PropertyDataVacant
    {
        public string PropertyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Size { get; set; }
        public string PropertyType { get; set; }
        public string PropertyStyle { get; set; }
        public Nullable<short> Bedroom { get; set; }
        public Nullable<short> Bathroom { get; set; }
        public string BuildingName { get; set; }
        public int? BuildingId { get; set; }
        public long? PropertyId { get; set; }
        public long? TenantId { get; set; }
    }

    public class BuildingData
    {
        public string BuildingName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Telephone { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public string ManagerName { get; set; }
        public string ManagerTelephone { get; set; }
        public int? BuildingId { get; set; }
        public int? PropertyId { get; set; }
        public int? TenantId { get; set; }
    }
    public class AllProperties
    {
        public string PropertyName { get; set; }
        public string BuildingName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string LeasedTo { get; set; }
        public int? BuildingId { get; set; }
        public long? PropertyId { get; set; }
        public long? TenantId { get; set; }
    }
    public class ActiveLease
    {
        public string Building { get; set; }
        public string Property { get; set; }
        public string Address { get; set; }
        public string Tenant { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public double? LateFee { get; set; }
        public double? Rent { get; set; }
        public int? BuildingId { get; set; }
        public long? PropertyId { get; set; }
        public long? TenantId { get; set; }
    }
    public class Accountingdata
    {
        public string Tenant { get; set; }
        public string Property { get; set; }
        public string Building { get; set; }
        public string Comment { get; set; }

        public Nullable<System.DateTime> RecievedOn { get; set; }
        public Nullable<double> Balance { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<double> Expenditure { get; set; }
        public int? BuildingId { get; set; }
        public long? PropertyId { get; set; }
        public long? TenantId { get; set; }
    }
    public class paymentHistorytable
    {
        public string Property { get; set; }
        public string Tenant { get; set; }
        public string Building { get; set; }
        public string ServiceProvider { get; set; }
        public DateTime? RecievedOn { get; set; }
        public string Type { get; set; }
        public double? Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public int? BuildingId { get; set; }
        public int? PropertyId { get; set; }
        public int? TenantId { get; set; }
    }
    public class paymentHistorydetails
    {
        public string Property { get; set; }
        public string Tenant { get; set; }
        public string Building { get; set; }
        public DateTime? RecievedOn { get; set; }
        public string Type { get; set; }
        public double? Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public int? BuildingId { get; set; }
        public int? PropertyId { get; set; }
        public int? TenantId { get; set; }
    }
    public class paymentHistoryreport
    {
        public string Property { get; set; }
        public string Tenant { get; set; }
        public string Building { get; set; }
        public DateTime? RecievedOn { get; set; }
        public string Type { get; set; }
        public double? Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public int? BuildingId { get; set; }
        public long? PropertyId { get; set; }
        public long? TenantId { get; set; }
    }
    public class InventoryReport
    {
        public string Property { get; set; }
        public string Building { get; set; }
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public double? Cost { get; set; }
        public int? BuildingId { get; set; }
        public long? PropertyId { get; set; }
        public long? TenantId { get; set; }
    }
    public class FaultReport
    {
        public string Property { get; set; }
        public string Building { get; set; }
        public string Tenant { get; set; }
        public string Provider { get; set; }
        public double? Cost { get; set; }
        public string VisitTime { get; set; }
        public DateTime? VisitDate { get; set; }
        public int? JobStatus { get; set; }
        public int? BuildingId { get; set; }
        public long? PropertyId { get; set; }
        public long? TenantId { get; set; }
    }

    public class BalanceReport
    {
        public string Property { get; set; }
        public string Tenant { get; set; }
        public string Telephone { get; set; }
        public string Building { get; set; }
        public DateTime? Due { get; set; }
        public string Type { get; set; }
        public int? Days_Late { get; set; }
        public double? Collected { get; set; }
        public double? Balance { get; set; }
        public double? Lease_Period { get; set; }
        public int? BuildingId { get; set; }
        public long? PropertyId { get; set; }
        public long? TenantId { get; set; }
    }
}