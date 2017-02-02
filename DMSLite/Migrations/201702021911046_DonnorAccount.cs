namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DonnorAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Donations", "DonationAccount_Id", c => c.Int());
            CreateIndex("dbo.Donations", "DonationAccount_Id");
            AddForeignKey("dbo.Donations", "DonationAccount_Id", "dbo.Accounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Donations", "DonationAccount_Id", "dbo.Accounts");
            DropIndex("dbo.Donations", new[] { "DonationAccount_Id" });
            DropColumn("dbo.Donations", "DonationAccount_Id");
        }
    }
}
