using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proptiwise.ViewModels
{
    public class TenantModel
    {
        public long Tenant_Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public string Cellular { get; set; }
        [Required(ErrorMessage = "Enter Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Email address must contain at least @ and dot")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter Confirm Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Enter atleast 6 characters with one capital ,one small letter and one numeric value")]
        [Compare("Password", ErrorMessage = "Confirm Password doesn't matched.")]
        [NotMapped]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Enter atleast 6 characters with one capital ,one small letter and one numeric value")]
        public string Password { get; set; }
        public string Gender { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Nullable<int> Country { get; set; }
        public string PostCode { get; set; }
        public Nullable<System.DateTime> Dateofbirth { get; set; }
        public string DriverLicense { get; set; }
        public string PassportNo { get; set; }
        public string SocialSecurity { get; set; }
        public Nullable<int> Profile { get; set; }
        public Nullable<bool> Smoker { get; set; }
        public Nullable<double> Balance { get; set; }
        public Nullable<System.DateTime> LastPmtOn { get; set; }
        public Nullable<System.DateTime> Nextpmtdue { get; set; }
        public Nullable<System.DateTime> LeaseEnds { get; set; }
        public Nullable<long> Property_Id { get; set; }
        public Nullable<int> leasestatus { get; set; }
        public string Comment { get; set; }
        public string Signature { get; set; }
        public Nullable<long> Landlord_id { get; set; }
        public string ProfilePhoto { get; set; }
        public Nullable<int> MaritalStatus { get; set; }
        public string IfVisa { get; set; }
        public Nullable<short> Pets { get; set; }
        public string ifPets { get; set; }
        public Nullable<short> Children { get; set; }
        public string IfChildren { get; set; }
        public string BuildingName { get; set; }
        public string Town { get; set; }
        public string UnitNumber { get; set; }
        public string Suberb { get; set; }
        public string Province { get; set; }
        public string StreetAddress { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }
        public string paymentmode { get; set; }
        public long lease_id { get; set; }
        public Nullable<long> Property_id { get; set; }
        public Nullable<long> tenant_id { get; set; }
        public string description { get; set; }
        public Nullable<double> amount { get; set; }
        public string due { get; set; }
        public Nullable<System.DateTime> validfrom { get; set; }
        public Nullable<System.DateTime> validuntil { get; set; }
    }
}