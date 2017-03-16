using DMSLite.Entities;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class SimilarDonorModel
    {
        [Display(Name = "NewDonor", ResourceType = typeof(Resources.Resources))]
        public Donor newDonor { get; set; }

        [Display(Name = "SimilarDonors", ResourceType = typeof(Resources.Resources))]
        public List<Donor> similarDonors { get; set; }
    }
}