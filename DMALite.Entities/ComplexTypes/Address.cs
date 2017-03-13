using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Entities.ComplexTypes
{
    [ComplexType]
    public class Address
    {
        [StringLength(255)]
        [Display(Name = "Address Line 1")]
        public string AddressLineOne { get; set; }

        [StringLength(255)]
        [Display(Name = "Address Line 2")]
        public string AddressLineTwo { get; set; }

        [StringLength(255)]
        public string City { get; set; }

        [StringLength(255)]
        public string Province { get; set; }

        [StringLength(255)]
        public string Country { get; set; }

        [StringLength(255)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
    }
}
