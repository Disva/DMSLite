namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserOrganization : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations");
            //DropIndex("dbo.Donors", new[] { "DonorOrganization_Id" });
            //AddColumn("dbo.Donations", "DonationOrganization_Id", c => c.Int());
            //AddColumn("dbo.Donors", "Archived", c => c.Boolean(nullable: false, defaultValue: false));
            //AlterColumn("dbo.Donors", "DonorOrganization_Id", c => c.Int());
            //CreateIndex("dbo.Donations", "DonationOrganization_Id");
            //CreateIndex("dbo.Donors", "DonorOrganization_Id");
            //AddForeignKey("dbo.Donations", "DonationOrganization_Id", "dbo.Organizations", "Id");
            //AddForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations");
            DropForeignKey("dbo.Donations", "DonationOrganization_Id", "dbo.Organizations");
            DropIndex("dbo.Donors", new[] { "DonorOrganization_Id" });
            DropIndex("dbo.Donations", new[] { "DonationOrganization_Id" });
            AlterColumn("dbo.Donors", "DonorOrganization_Id", c => c.Int(nullable: false));
            DropColumn("dbo.Donors", "Archived");
            DropColumn("dbo.Donations", "DonationOrganization_Id");
            CreateIndex("dbo.Donors", "DonorOrganization_Id");
            AddForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations", "Id", cascadeDelete: true);
        }
    }
}
