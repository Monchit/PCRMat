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
    
    public partial class V_Transaction
    {
        public string lv_name { get; set; }
        public string action_name { get; set; }
        public string emp_fname { get; set; }
        public string emp_lname { get; set; }
        public string status_name { get; set; }
        public string group_code { get; set; }
        public string year { get; set; }
        public int run_no { get; set; }
        public byte round_no { get; set; }
        public byte status_id { get; set; }
        public byte lv_id { get; set; }
        public int org_id { get; set; }
        public byte action_id { get; set; }
        public string actor { get; set; }
        public System.DateTime act_dt { get; set; }
        public bool active { get; set; }
        public string comment { get; set; }
        public string email { get; set; }
    }
}
