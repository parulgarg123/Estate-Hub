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
    
    public partial class Country
    {
        public Country()
        {
            this.Cities = new HashSet<City>();
            this.States = new HashSet<State>();
            this.tblDocuments = new HashSet<tblDocument>();
            this.UserDocuments = new HashSet<UserDocument>();
            this.Users = new HashSet<User>();
        }
    
        public int countryId { get; set; }
        public string countryName { get; set; }
    
        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<State> States { get; set; }
        public virtual ICollection<tblDocument> tblDocuments { get; set; }
        public virtual ICollection<UserDocument> UserDocuments { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
        public string keyId { get; set; }
        public string value { get; set; }
        public string flagimg { get; set; }
    }
}
