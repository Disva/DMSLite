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

        public DbSet<Organization> Organizations { get; set; }
    }
}