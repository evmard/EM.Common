namespace Crm.DbUpdater.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MigrationInfoes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Major = c.Int(nullable: false),
                        Minor = c.Int(nullable: false),
                        Build = c.Int(nullable: false),
                        PatchHash = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AppliedPatches",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FileName = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        Hash = c.String(nullable: false),
                        Installed = c.DateTime(nullable: false),
                        MigrationInfo_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MigrationInfoes", t => t.MigrationInfo_Id)
                .Index(t => t.MigrationInfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppliedPatches", "MigrationInfo_Id", "dbo.MigrationInfoes");
            DropIndex("dbo.AppliedPatches", new[] { "MigrationInfo_Id" });
            DropTable("dbo.AppliedPatches");
            DropTable("dbo.MigrationInfoes");
        }
    }
}
