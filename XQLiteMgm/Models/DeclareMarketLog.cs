//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace XQLiteMgm.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DeclareMarketLog
    {
        public string userid { get; set; }
        public string appid { get; set; }
        public string market { get; set; }
        public int seq { get; set; }
        public Nullable<System.DateTime> startdate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<System.DateTime> lastupdate { get; set; }
        public string isdeclare { get; set; }
        public string syscheck { get; set; }
        public string @operator { get; set; }
    }
}
