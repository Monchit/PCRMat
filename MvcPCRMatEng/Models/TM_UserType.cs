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
    
    public partial class TM_UserType
    {
        public TM_UserType()
        {
            this.TM_User = new HashSet<TM_User>();
        }
    
        public byte utype_id { get; set; }
        public string utype_name { get; set; }
        public System.DateTime update_dt { get; set; }
        public string update_by { get; set; }
        public bool del_flag { get; set; }
    
        public virtual ICollection<TM_User> TM_User { get; set; }
    }
}
