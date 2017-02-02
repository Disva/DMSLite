namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DonorAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Donors", "Address", c => c.String(maxLength: 255, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Donors", "Address");
        }
    }
}
