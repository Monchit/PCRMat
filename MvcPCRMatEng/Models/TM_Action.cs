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
    
    public partial class TM_Action
    {
        public TM_Action()
        {
            this.TD_Transaction = new HashSet<TD_Transaction>();
        }
    
        public byte action_id { get; set; }
        public string action_name { get; set; }
        public System.DateTime update_dt { get; set; }
        public string update_by { get; set; }
        public bool del_flag { get; set; }
    
        public virtual ICollection<TD_Transaction> TD_Transaction { get; set; }
    }
}
