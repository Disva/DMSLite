namespace DMSLite.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GlobalTenantUserFilter : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.AspNetUsers", "UserOrganization_Id", "dbo.Organizations");
            //DropIndex("dbo.AspNetUsers", new[] { "UserOrganization_Id" });
            AddColumn("dbo.AspNetUsers", "TenantId", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "UserOrganization_Id");
            //DropTable("dbo.Organizations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "UserOrganization_Id", c => c.Int());
            DropColumn("dbo.AspNetUsers", "TenantId");
            CreateIndex("dbo.AspNetUsers", "UserOrganization_Id");
            AddForeignKey("dbo.AspNetUsers", "UserOrganization_Id", "dbo.Organizations", "Id");
        }
    }
}
