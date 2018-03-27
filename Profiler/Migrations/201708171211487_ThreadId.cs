namespace Profiling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThreadId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LogItems", "TreadId", c => c.Long(nullable: false));
            AddColumn("dbo.WarningItems", "TreadId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WarningItems", "TreadId");
            DropColumn("dbo.LogItems", "TreadId");
        }
    }
}
