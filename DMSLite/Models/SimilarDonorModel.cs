using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class SimilarDonorModel
    {
        public Donor newDonor { get; set; }
        public List<Donor> similarDonors { get; set; }
    }
}