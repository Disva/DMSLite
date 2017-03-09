using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class BatchViewModel
    {
        [Required]
        [Display(Name = "Batch")]
        public Batch batch { get; set; }

        [Required]
        [Display(Name = "Number Of Donations")]
        public int count { get; set; }

        [Required]
        [Display(Name = "Total Amount Donated")]
        public double sum { get; set; }
    }
}