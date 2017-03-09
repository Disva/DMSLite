using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class ReceiptFormModel
    {
        [Display(Name = "Donors")]
        public List<Donor> donors { get; set; }

        [Display(Name = "Batches")]
        public List<Batch> batches { get; set; }
    }
}