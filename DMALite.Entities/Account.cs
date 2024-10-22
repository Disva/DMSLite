﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Entities
{
    public class Account : IHaveTenant
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "AccountTitle", ResourceType = typeof(Resources.Resources))]
        public string Title { get; set; }

        [Required]
        public int TenantOrganizationId { get; set; }

        public bool isEqualTo(Account otherAccount)
        {
            if (Title.Equals(otherAccount.Title))
            {
                return true;
            }
            else return false;
        }

    }

}
