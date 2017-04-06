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
        [Display(Name = "OrgName", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }

        [StringLength(255)]
        [Display(Name = "DBA", ResourceType = typeof(Resources.Resources))]
        public string DBA { get; set; }

        [Display(Name = "OrgAddress", ResourceType = typeof(Resources.Resources))]
        public Address Address { get; set; } 

        [StringLength(255)]
        [Display(Name = "RegistrationNumer", ResourceType = typeof(Resources.Resources))]
        public string RegistrationNumer { get; set; }
    }
}
