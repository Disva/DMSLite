using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class BatchViewModel
    {
        public Batch batch { get; set; }
        public int count { get; set; }
        public double sum { get; set; }
    }
}