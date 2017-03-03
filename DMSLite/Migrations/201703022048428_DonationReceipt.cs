namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DonationReceipt : DbMigration
    {
        public override void Up()
        {
            /*
            CreateTable(
                "dbo.Receipts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IssueDate = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Donations", "DonationReceipt_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Donations", "DonationReceipt_Id");
            AddForeignKey("dbo.Donations", "DonationReceipt_Id", "dbo.Receipts", "Id", cascadeDelete: true);
            */
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Donations", "DonationReceipt_Id", "dbo.Receipts");
            DropIndex("dbo.Donations", new[] { "DonationReceipt_Id" });
            DropColumn("dbo.Donations", "DonationReceipt_Id");
            DropTable("dbo.Receipts");
        }
    }
}
