namespace DMSLite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DonationsAndBatches : DbMigration
    {
        public override void Up()
        {
            /*CreateTable(
                "dbo.Batches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        CreateDate = c.DateTime(nullable: false, precision: 0),
                        CloseDate = c.DateTime(precision: 0),
                        BatchOrganization_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.BatchOrganization_Id, cascadeDelete: true)
                .Index(t => t.BatchOrganization_Id);
            
            CreateTable(
                "dbo.Donations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectDescription = c.String(maxLength: 255, storeType: "nvarchar"),
                        Value = c.Double(nullable: false),
                        DonationBatch_Id = c.Int(nullable: false),
                        DonationDonor_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Batches", t => t.DonationBatch_Id, cascadeDelete: true)
                .ForeignKey("dbo.Donors", t => t.DonationDonor_Id, cascadeDelete: true)
                .Index(t => t.DonationBatch_Id)
                .Index(t => t.DonationDonor_Id);
            */
            //AddColumn("dbo.Donors", "DonorOrganization_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Donors", "FirstName", c => c.String(nullable: false, maxLength: 255, storeType: "nvarchar"));
            AlterColumn("dbo.Donors", "LastName", c => c.String(nullable: false, maxLength: 255, storeType: "nvarchar"));
            AlterColumn("dbo.Donors", "Type", c => c.Int(nullable: false));
            AlterColumn("dbo.Organizations", "Name", c => c.String(nullable: false, maxLength: 255, storeType: "nvarchar"));
            //CreateIndex("dbo.Donors", "DonorOrganization_Id");
            //AddForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Donations", "DonationDonor_Id", "dbo.Donors");
            DropForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations");
            DropForeignKey("dbo.Donations", "DonationBatch_Id", "dbo.Batches");
            DropForeignKey("dbo.Batches", "BatchOrganization_Id", "dbo.Organizations");
            DropIndex("dbo.Donors", new[] { "DonorOrganization_Id" });
            DropIndex("dbo.Donations", new[] { "DonationDonor_Id" });
            DropIndex("dbo.Donations", new[] { "DonationBatch_Id" });
            DropIndex("dbo.Batches", new[] { "BatchOrganization_Id" });
            AlterColumn("dbo.Organizations", "Name", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AlterColumn("dbo.Donors", "Type", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AlterColumn("dbo.Donors", "LastName", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AlterColumn("dbo.Donors", "FirstName", c => c.String(maxLength: 255, storeType: "nvarchar"));
            DropColumn("dbo.Donors", "DonorOrganization_Id");
            DropTable("dbo.Donations");
            DropTable("dbo.Batches");
        }
    }
}
