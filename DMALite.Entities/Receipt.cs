﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Entities
{
    public class Receipt : IHaveTenant
    {
        public int Id { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        [Required]
        public int TenantOrganizationId { get; set; }

        [StringLength(255)]
        public string Address { get; set; }
    }
}