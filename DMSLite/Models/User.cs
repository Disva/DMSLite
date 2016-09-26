namespace DMSLite
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DMSLite.Users")]
    public partial class User
    {
        [Column(TypeName = "uint")]
        public long id { get; set; }

        [Required]
        [StringLength(255)]
        public string username { get; set; }

        [Required]
        [StringLength(255)]
        public string password { get; set; }

        [Column(TypeName = "uint")]
        public long? companyId { get; set; }

        [Column(TypeName = "uint")]
        public long? permissionId { get; set; }

        public virtual Company Company { get; set; }

        public virtual PermissionLevel PermissionLevel { get; set; }
    }
}
