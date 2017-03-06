using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMSLite.Entities.ComplexTypes;

namespace DMSLite.Entities
{
    public class Organization
    {
        public int Id { get; set; }

        [StringLength(255)]
        [Required]
        public string Name { get; set; }

        [StringLength(255)]
        public string DBA { get; set; }

        public Address Address { get; set; } 

        [StringLength(255)]
        public string RegistrationNumer { get; set; }
    }
}
