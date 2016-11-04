using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class ResponseModel
    {
        public string Speech { get; set; }
        public Tuple<string, string> Instructions { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}