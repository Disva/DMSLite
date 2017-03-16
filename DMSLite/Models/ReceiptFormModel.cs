using DMSLite.Entities;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class ReceiptFormModel
    {
        [Display(Name = "Donors", ResourceType = typeof(Resources.Resources))]
        public List<Donor> donors { get; set; }

        [Display(Name = "Batches", ResourceType = typeof(Resources.Resources))]
        public List<Batch> batches { get; set; }
    }
}