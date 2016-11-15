using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DMSLite.Entities;
using DMSLite.Models;
using EntityFramework.DynamicFilters;
using Microsoft.AspNet.Identity;
using System.Threading;
using Microsoft.AspNet.Identity.Owin;

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

            modelBuilder.Filter("TenantFilter", (IHaveTenant entity, int organizationId) => entity.TenantId == organizationId, () => HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(Thread.CurrentPrincipal.Identity.GetUserId()).UserOrganization.Id);
        }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Donation> Donations { get; set; }
    }
}