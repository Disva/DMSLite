using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class AddDonationViewModel
    {
        public List<Donor> SimilarDonors { get; set; }
        public List<Batch> SimilarBatches { get; set; }
        public Donation Donation { get; set; }
    }
}