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

            // Dynamic filter that filters database objects based off of the current user's tenant id
            modelBuilder.Filter("TenantFilter", (IHaveTenant entity, int organizationId) => entity.TenantOrganizationId == organizationId, () => GetTenantId());
        }

        // Add an item to the database and set its tenant id
        public void Add<TEntity>(TEntity entity) where TEntity : class, IHaveTenant
        {
            entity.TenantOrganizationId = GetTenantId();
            Set<TEntity>().Add(entity);
            SaveChanges();
        }

        // Modify an item in the database and set its tenant id
        public void Modify<TEntity>(TEntity entity) where TEntity : class, IHaveTenant
        {
            entity.TenantOrganizationId = GetTenantId();
            Entry(entity).State = EntityState.Modified;
            SaveChanges();
        }
        
        // Retrieve the tenant id of the currently logged in user
        protected virtual int GetTenantId()
        {
            return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(Thread.CurrentPrincipal.Identity.GetUserId()).TenantId;
        }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}