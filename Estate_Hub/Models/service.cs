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
    
    public partial class service
    {
        public service()
        {
            this.Faults = new HashSet<Fault>();
            this.Serviceproviders = new HashSet<Serviceprovider>();
        }
    
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public Nullable<long> LandlordId { get; set; }
    
        public virtual ICollection<Fault> Faults { get; set; }
        public virtual ICollection<Serviceprovider> Serviceproviders { get; set; }
        public virtual User User { get; set; }
    }
}
