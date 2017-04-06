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
        [Display(Name = "Batch", ResourceType = typeof(Resources.Resources))]
        public Batch batch { get; set; }

        [Required]
        [Display(Name = "Count", ResourceType = typeof(Resources.Resources))]
        public int count { get; set; }

        [Required]
        [Display(Name = "Sum", ResourceType = typeof(Resources.Resources))]
        public double sum { get; set; }
    }
}