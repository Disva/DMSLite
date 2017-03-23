namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DonationGift : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Donations", "Gift", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Donations", "Gift");
        }
    }
}
