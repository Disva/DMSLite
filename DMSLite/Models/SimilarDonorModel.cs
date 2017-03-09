using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class SimilarDonorModel
    {
        [Display(Name = "New Donor")]
        public Donor newDonor { get; set; }

        [Display(Name = "Similar Donors")]
        public List<Donor> similarDonors { get; set; }
    }
}