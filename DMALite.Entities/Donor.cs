using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Entities
{
    public enum DonorType
    {
        Individual,
        Business,
        Charity,
        Foreign,
        Other
    }

    public class Donor : IHaveTenant
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        public DonorType Type { get; set; }

        [StringLength(255)]
        [Display(Name = "Receipt Frequency")]
        public string ReceiptFrequency { get; set; }

        public Organization DonorOrganization { get; set; }

        public bool Archived { get; set; }

        public int TenantId
        {
            get
            {
                return DonorOrganization.Id;
            }
        }

        //ASSUMPTION
        //no users with NULL firstname, lastname, email or phone number can exist
        //TODO: update this method to allow fornull emails and phone numbers
        public bool isEqualTo(Donor otherDonor)
        {
            if (FirstName.Equals(otherDonor.FirstName) &&
                LastName.Equals(otherDonor.LastName) &&
                Email.Equals(otherDonor.Email) &&
                PhoneNumber.Equals(otherDonor.PhoneNumber))
            {
                return true;
            }
            else return false;
        }

        public bool isValid()
        {
            return (
                (!String.IsNullOrWhiteSpace(FirstName))
                && (!String.IsNullOrWhiteSpace(LastName)));
        }

    }
}
