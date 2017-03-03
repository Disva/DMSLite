namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReceiptTenantId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Receipts", "TenantOrganizationId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Receipts", "TenantOrganizationId");
        }
    }
}
