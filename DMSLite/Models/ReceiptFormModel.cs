using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class ReceiptFormModel
    {
        public List<Donor> donors { get; set; }
        public List<Batch> batches { get; set; }
    }
}