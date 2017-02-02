using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Entities
{
    public class Donation : IHaveTenant
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Donor")]
        public Donor DonationDonor { get; set; }

        [ForeignKey("DonationDonor")]        
        public int DonationDonor_Id { get; set; }

        [StringLength(255)]
        [Display(Name = "Description")]
        public string ObjectDescription { get; set; }

        [Required]
        public double Value { get; set; }

        [Required]
        [Display(Name = "Batch")]
        public Batch DonationBatch { get; set; }

        [Display(Name = "Account")]
        public Account DonationAccount { get; set; }

        [ForeignKey("DonationAccount")]
        public int DonationAccount_Id { get; set; }

        [ForeignKey("DonationBatch")]
        public int DonationBatch_Id { get; set; }

        [Required]
        public int TenantOrganizationId {get; set; }

        public bool isEqualTo(Donation otherDonation)
        {
            if (Id.Equals(otherDonation.Id) &&
                DonationDonor.isEqualTo(otherDonation.DonationDonor) &&
                DonationBatch.isEqualTo(otherDonation.DonationBatch) &&
                ObjectDescription.Equals(otherDonation.ObjectDescription) &&
                Value.Equals(otherDonation.Value))
            {
                return true;
            }
            else return false;
        }

    }
}
