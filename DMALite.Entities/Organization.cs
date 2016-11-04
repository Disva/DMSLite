using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Entities
{
    public class Organization
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }


    }
}
