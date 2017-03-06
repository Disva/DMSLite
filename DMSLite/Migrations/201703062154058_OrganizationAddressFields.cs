namespace DMSLite.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class OrganizationAddressFields : DbMigration
    {
        public override void Up()
        {
            /*AlterTableAnnotations(
                "dbo.Receipts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IssueDate = c.DateTime(nullable: false, precision: 0),
                        TenantOrganizationId = c.Int(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Receipt_TenantFilter",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                });*/
            
            AddColumn("dbo.Organizations", "DBA", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Organizations", "Address_AddressLineOne", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Organizations", "Address_AddressLineTwo", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Organizations", "Address_City", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Organizations", "Address_Province", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Organizations", "Address_Country", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Organizations", "Address_PostalCode", c => c.String(maxLength: 255, storeType: "nvarchar"));
            AddColumn("dbo.Organizations", "RegistrationNumer", c => c.String(maxLength: 255, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organizations", "RegistrationNumer");
            DropColumn("dbo.Organizations", "Address_PostalCode");
            DropColumn("dbo.Organizations", "Address_Country");
            DropColumn("dbo.Organizations", "Address_Province");
            DropColumn("dbo.Organizations", "Address_City");
            DropColumn("dbo.Organizations", "Address_AddressLineTwo");
            DropColumn("dbo.Organizations", "Address_AddressLineOne");
            DropColumn("dbo.Organizations", "DBA");
            AlterTableAnnotations(
                "dbo.Receipts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IssueDate = c.DateTime(nullable: false, precision: 0),
                        TenantOrganizationId = c.Int(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Receipt_TenantFilter",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                });
            
        }
    }
}
