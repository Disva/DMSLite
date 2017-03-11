using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class DonationFormViewModel
    {
        public Donation donation {get; set;}

        public int donationDonor { get; set; }
        public int donationBatch { get; set; }

        public List<Batch> batches { get; set; }
        public List<Donor> donors { get; set; }
    }
}