namespace DMSLite.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class GlobalTenantFilter : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Batches", "BatchOrganization_Id", "dbo.Organizations");
            /*DropForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations");
            DropForeignKey("dbo.Donations", "DonationOrganization_Id", "dbo.Organizations");
            DropIndex("dbo.Batches", new[] { "BatchOrganization_Id" });
            DropIndex("dbo.Donations", new[] { "DonationOrganization_Id" });
            DropIndex("dbo.Donors", new[] { "DonorOrganization_Id" });*/
            /*AlterTableAnnotations(
                "dbo.Batches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        CreateDate = c.DateTime(nullable: false, precision: 0),
                        CloseDate = c.DateTime(precision: 0),
                        TenantOrganizationId = c.Int(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Batch_TenantFilter",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                });
            
            AlterTableAnnotations(
                "dbo.Donations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectDescription = c.String(maxLength: 255, storeType: "nvarchar"),
                        Value = c.Double(nullable: false),
                        TenantOrganizationId = c.Int(nullable: false),
                        DonationBatch_Id = c.Int(nullable: false),
                        DonationDonor_Id = c.Int(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Donation_TenantFilter",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                });
            
            AlterTableAnnotations(
                "dbo.Donors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        LastName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        Email = c.String(maxLength: 255, storeType: "nvarchar"),
                        PhoneNumber = c.String(maxLength: 255, storeType: "nvarchar"),
                        Type = c.Int(nullable: false),
                        ReceiptFrequency = c.String(maxLength: 255, storeType: "nvarchar"),
                        TenantOrganizationId = c.Int(nullable: false),
                        Archived = c.Boolean(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Donor_TenantFilter",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                });*/
            
            /*AddColumn("dbo.Batches", "TenantOrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.Donations", "TenantOrganizationId", c => c.Int(nullable: false));
            AddColumn("dbo.Donors", "TenantOrganizationId", c => c.Int(nullable: false));
            DropColumn("dbo.Batches", "BatchOrganization_Id");
            DropColumn("dbo.Donations", "DonationOrganization_Id");
            DropColumn("dbo.Donors", "DonorOrganization_Id");*/
        }
        
        public override void Down()
        {
            AddColumn("dbo.Donors", "DonorOrganization_Id", c => c.Int());
            AddColumn("dbo.Donations", "DonationOrganization_Id", c => c.Int());
            AddColumn("dbo.Batches", "BatchOrganization_Id", c => c.Int(nullable: false));
            DropColumn("dbo.Donors", "TenantOrganizationId");
            DropColumn("dbo.Donations", "TenantOrganizationId");
            DropColumn("dbo.Batches", "TenantOrganizationId");
            AlterTableAnnotations(
                "dbo.Donors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        LastName = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        Email = c.String(maxLength: 255, storeType: "nvarchar"),
                        PhoneNumber = c.String(maxLength: 255, storeType: "nvarchar"),
                        Type = c.Int(nullable: false),
                        ReceiptFrequency = c.String(maxLength: 255, storeType: "nvarchar"),
                        TenantOrganizationId = c.Int(nullable: false),
                        Archived = c.Boolean(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Donor_TenantFilter",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                });
            
            AlterTableAnnotations(
                "dbo.Donations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectDescription = c.String(maxLength: 255, storeType: "nvarchar"),
                        Value = c.Double(nullable: false),
                        TenantOrganizationId = c.Int(nullable: false),
                        DonationBatch_Id = c.Int(nullable: false),
                        DonationDonor_Id = c.Int(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Donation_TenantFilter",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                });
            
            AlterTableAnnotations(
                "dbo.Batches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        CreateDate = c.DateTime(nullable: false, precision: 0),
                        CloseDate = c.DateTime(precision: 0),
                        TenantOrganizationId = c.Int(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Batch_TenantFilter",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                });
            
            CreateIndex("dbo.Donors", "DonorOrganization_Id");
            CreateIndex("dbo.Donations", "DonationOrganization_Id");
            CreateIndex("dbo.Batches", "BatchOrganization_Id");
            AddForeignKey("dbo.Donations", "DonationOrganization_Id", "dbo.Organizations", "Id");
            AddForeignKey("dbo.Donors", "DonorOrganization_Id", "dbo.Organizations", "Id");
            AddForeignKey("dbo.Batches", "BatchOrganization_Id", "dbo.Organizations", "Id", cascadeDelete: true);
        }
    }
}
