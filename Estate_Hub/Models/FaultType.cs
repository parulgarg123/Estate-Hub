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
    
    public partial class FaultType
    {
        public FaultType()
        {
            this.Faults = new HashSet<Fault>();
        }
    
        public int FaultTypeId { get; set; }
        public string FaultTypeName { get; set; }
    
        public virtual ICollection<Fault> Faults { get; set; }
    }
}