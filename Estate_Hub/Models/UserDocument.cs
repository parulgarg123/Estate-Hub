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
    
    public partial class UserDocument
    {
        public int UserDocumentId { get; set; }
        public Nullable<int> DocumentId { get; set; }
        public Nullable<int> Status { get; set; }
        public string ContentDetails { get; set; }
        public Nullable<long> LandlordId { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string DocTitle { get; set; }
        public Nullable<int> Country { get; set; }
    
        public virtual Country Country1 { get; set; }
        public virtual DocumentType DocumentType { get; set; }
        public virtual User User { get; set; }
    }
}
