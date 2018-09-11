using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proptiwise.ViewModels
{
    public class UserModel
    {
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Enter email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Email address must contain at least @ and dot")]
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public Nullable<int> Country { get; set; }
        public string County { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "Enter mobile number")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Enter numeric numbers only")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Enter confirm password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Enter atleast 6 characters with one capital ,one small letter and one numeric value")]
        [Compare("Password", ErrorMessage = "Confirm Password doesn't matched.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Enter atleast 6 characters with one capital ,one small letter and one numeric value")]
        public string Password { get; set; }
        public Nullable<int> UserTypeID { get; set; }
        public Nullable<int> UserStatusID { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string TimeZone { get; set; }
        public Nullable<bool> AgreedToTerms { get; set; }
        public string Terms { get; set; }
        public Nullable<int> CCType { get; set; }
        public string CCNo { get; set; }
        public string CCExpDate { get; set; }
        public string CCSecurityNo { get; set; }
        public Nullable<bool> BillingAddressSameAsCase { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string NameOnAccount { get; set; }
        public string UserName { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public Nullable<int> UserRole { get; set; }
        public string Signature { get; set; }
        public string photo { get; set; }
        public Nullable<int> RoleId { get; set; }
        public string ProfilePhoto { get; set; }
        public string Town { get; set; }
        public string BuildingName { get; set; }
        public string UnitNumber { get; set; }
        public string Suberb { get; set; }
        public string Province { get; set; }
        public string StreetAddress { get; set; }
        public string PostCode { get; set; }
        public string StripeCustomerId { get; set; }
        public Nullable<long> UserLandlordId { get; set; }
    }
}