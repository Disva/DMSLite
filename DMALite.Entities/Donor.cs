using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(255)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        private string _phoneNumber;

        [StringLength(255)]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber {
            get { return _phoneNumber; }
            set {
                if (String.IsNullOrEmpty(value))
                    return;

                string phoneNumber = Regex.Replace(value, "[^\\d]", "");

                // If the phone number is not something we can format, just set it
                if (!Regex.IsMatch(phoneNumber, "\\d{10}"))
                {
                    _phoneNumber = value;
                    return;
                }

                //This method currently assumes the phone number is at least ten digits and does not feature a "+1".
                string s1 = phoneNumber.Substring(0, 3);
                string s2 = phoneNumber.Substring(3, 3);
                string s3 = phoneNumber.Substring(6, 4);
                string s4 = "";
                if (phoneNumber.Length > 10)
                    s4 = phoneNumber.Substring(10);

                _phoneNumber = s1 + "-" + s2 + "-" + s3 + ((s4 == "") ? "" : " " + s4);
            }
        }

        [Required]
        public DonorType Type { get; set; }

        [StringLength(255)]
        [Display(Name = "Receipt Frequency")]
        public string ReceiptFrequency { get; set; }

        [Required]
        public int TenantOrganizationId { get; set; }

        public bool Archived { get; set; }

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
    }
}
