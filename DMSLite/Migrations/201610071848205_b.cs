namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Donors", "DonorOrganization_Id", c => c.Int());
            CreateIndex("dbo.Donors", "DonorOrganization_Id");
            AddForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations");
            DropIndex("dbo.Donors", new[] { "DonorOrganization_Id" });
            DropColumn("dbo.Donors", "DonorOrganization_Id");
        }
    }
}
