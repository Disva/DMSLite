using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Entities
{
    public class Donor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255)]
        public string LastName { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Type { get; set; }

        [StringLength(255)]
        public string ReceiptFrequency { get; set; }

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
