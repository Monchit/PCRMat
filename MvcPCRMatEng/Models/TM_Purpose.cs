//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcPCRMatEng.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TM_Purpose
    {
        public TM_Purpose()
        {
            this.TD_PCR = new HashSet<TD_PCR>();
        }
    
        public byte purpose_id { get; set; }
        public string purpose_name { get; set; }
        public System.DateTime update_dt { get; set; }
        public string update_by { get; set; }
        public bool del_flag { get; set; }
    
        public virtual ICollection<TD_PCR> TD_PCR { get; set; }
    }
}
