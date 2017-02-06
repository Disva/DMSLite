namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Donor : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Donors", "FirstName", c => c.String(maxLength: 255, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Donors", "FirstName", c => c.String(nullable: false, maxLength: 255, storeType: "nvarchar"));
        }
    }
}
