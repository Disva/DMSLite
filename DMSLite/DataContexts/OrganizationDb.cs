using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DMSLite.Entities;
using DMSLite.Models;

namespace DMSLite.DataContexts
{
    public class OrganizationDb : DbContext
    {
        public OrganizationDb() : base("LocalMySqlServer")
        {
            Database.SetInitializer(new MySqlInitializer());
        }

        public static OrganizationDb Create()
        {
            return new OrganizationDb();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<OrganizationDb>(null);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Donor> Donors { get; set; }
    }
}