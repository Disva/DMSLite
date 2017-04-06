namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BatchDonorAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Receipts", "Address", c => c.String(maxLength: 255, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Receipts", "Address");
        }
    }
}
