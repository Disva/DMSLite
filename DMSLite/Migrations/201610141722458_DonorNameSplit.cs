namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DonorNameSplit : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations");
            //DropIndex("dbo.Donors", new[] { "DonorOrganization_Id" });
            AddColumn("dbo.Donors", "FirstName", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Donors", "LastName", c => c.String(maxLength: 255, storeType: "nvarchar"));
            //AlterColumn()
            //Sql("Update dbo.Donors Set FirstName=Name");
            DropColumn("dbo.Donors", "Name");
            //DropColumn("dbo.Donors", "DonorOrganization_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Donors", "DonorOrganization_Id", c => c.Int());
            AddColumn("dbo.Donors", "Name", c => c.String(maxLength: 255, storeType: "nvarchar"));
            DropColumn("dbo.Donors", "LastName");
            DropColumn("dbo.Donors", "FirstName");
            CreateIndex("dbo.Donors", "DonorOrganization_Id");
            AddForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations", "Id");
        }
    }
}
