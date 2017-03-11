using DMSLite.DataContexts;
using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMSLite.Models
{
    public class DonationFormViewModel
    {
        [Required]
        public Donation donation {get; set;}

        [Required]
        public int donationDonorId { get; set; }

        [Required]
        public int donationBatchId { get; set; }

        public int donationAccountId { get; set; }

        public List<Donor> donors { get; set; }
        public List<Batch> batches { get; set; }
        public List<Account> accounts { get; set; }

        public DonationFormViewModel()
        {

        }

        public DonationFormViewModel(Donation donation, OrganizationDb db)
        {
            this.donation = donation;

            donors = db.Donors.ToList();

            // Only populate open batches
            batches = db.Batches.Where(x => x.CloseDate == null).ToList();

            accounts = db.Accounts.ToList();
        }
    }
}