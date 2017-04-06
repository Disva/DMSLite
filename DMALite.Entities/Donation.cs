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
        [Display(Name = "DonationDonor", ResourceType = typeof(Resources.Resources))]
        public Donor DonationDonor { get; set; }

        [ForeignKey("DonationDonor")]
        [Display(Name = "DonorID", ResourceType = typeof(Resources.Resources))]
        public int DonationDonor_Id { get; set; }

        [StringLength(255)]
        [Display(Name = "ObjectDescription", ResourceType = typeof(Resources.Resources))]
        public string ObjectDescription { get; set; }

        [Required]
        [Display(Name = "Value", ResourceType = typeof(Resources.Resources))]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public double Value { get; set; }

        [Required]
        [Display(Name = "DonationBatch", ResourceType = typeof(Resources.Resources))]
        public Batch DonationBatch { get; set; }

        [Display(Name = "DonationAccount", ResourceType = typeof(Resources.Resources))]
        public Account DonationAccount { get; set; }

        [ForeignKey("DonationAccount")]
        public int? DonationAccount_Id { get; set; }

        [ForeignKey("DonationBatch")]
        public int DonationBatch_Id { get; set; }

        [Display(Name = "DonationReceipt", ResourceType = typeof(Resources.Resources))]
        public Receipt DonationReceipt { get; set; }

        [ForeignKey("DonationReceipt")]
        public int DonationReceipt_Id { get; set; }

        [Required]
        public int TenantOrganizationId {get; set; }

        public bool Gift { get; set; }

        public bool isEqualTo(Donation otherDonation)
        {
            if (DonationAccount != null)
            {
                if (otherDonation.DonationAccount != null)
                {
                    if (!DonationAccount.isEqualTo(otherDonation.DonationAccount))
                        return false;
                }
                else return false;
            }
            else if (otherDonation.DonationAccount != null)
                return false;
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
