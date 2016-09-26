namespace DMSLite
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class dbContext : DbContext
    {
        public dbContext()
            : base("name=dbContext")
        {
        }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<PermissionLevel> PermissionLevels { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<PermissionLevel>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<PermissionLevel>()
                .HasMany(e => e.Users)
                .WithOptional(e => e.PermissionLevel)
                .HasForeignKey(e => e.permissionId);

            modelBuilder.Entity<User>()
                .Property(e => e.username)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.password)
                .IsUnicode(false);
        }
    }
}
