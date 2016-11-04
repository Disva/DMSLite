/* this file is used as a fix to let the migration history table have a primary key small enough for MySQL.
 * Source: http://www.codeproject.com/Tips/788357/How-to-set-up-application-using-ASP-NET-Identity-w
 */

using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations.History;

namespace DMSLite.Migrations
{
    public class MySqlHistoryContext : HistoryContext
    {
        public MySqlHistoryContext(DbConnection connection, string defaultSchema)
            : base(connection, defaultSchema)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity < HistoryRow > ().Property(h => h.MigrationId).HasMaxLength(100).IsRequired();
            modelBuilder.Entity < HistoryRow > ().Property(h => h.ContextKey).HasMaxLength(200).IsRequired();
        }
    }
}