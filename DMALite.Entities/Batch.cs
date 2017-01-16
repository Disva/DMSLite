using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Entities
{
    public class Batch : IHaveTenant
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime? CloseDate { get; set; }

        [Required]
        public int TenantOrganizationId { get; set; }

        public bool isEqualTo(Batch otherBatch)
        {
            if (Title.Equals(otherBatch.Title) &&
                CreateDate.Equals(otherBatch.CreateDate) &&
                CloseDate.Equals(otherBatch.CloseDate) &&
                TenantOrganizationId.Equals(otherBatch.TenantOrganizationId))
            {
                return true;
            }
            else return false;
        }

    }

}
