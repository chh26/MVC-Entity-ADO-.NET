using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XQLiteMgm.Models
{
    public class ResultViewModels<T>
    {
        public Status ResultStatus { get; set; }
        public string Msg { get; set; }

        public T Result { get; set; }
    }

    public enum Status { success = 1, fail = 0 };
}