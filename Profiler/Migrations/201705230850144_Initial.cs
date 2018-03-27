namespace Profiling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Level = c.Int(nullable: false),
                        WarnTime = c.DateTime(nullable: false),
                        ProcessId = c.Long(nullable: false),
                        Message = c.String(),
                        CallerName = c.String(maxLength: 512),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Level)
                .Index(t => t.ProcessId)
                .Index(t => t.CallerName);
            
            CreateTable(
                "dbo.StatisticValues",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ActorName = c.String(maxLength: 512),
                        ProcessId = c.Long(nullable: false),
                        Min = c.Long(nullable: false),
                        Max = c.Long(nullable: false),
                        CalledTimes = c.Long(nullable: false),
                        Summ = c.Long(nullable: false),
                        Average = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ActorName)
                .Index(t => t.ProcessId);
            
            CreateTable(
                "dbo.WarningItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        WarnTime = c.DateTime(nullable: false),
                        ProcessId = c.Long(nullable: false),
                        CallerName = c.String(maxLength: 512),
                        CallParams = c.String(),
                        TimeElapsed = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProcessId)
                .Index(t => t.CallerName);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.WarningItems", new[] { "CallerName" });
            DropIndex("dbo.WarningItems", new[] { "ProcessId" });
            DropIndex("dbo.StatisticValues", new[] { "ProcessId" });
            DropIndex("dbo.StatisticValues", new[] { "ActorName" });
            DropIndex("dbo.LogItems", new[] { "CallerName" });
            DropIndex("dbo.LogItems", new[] { "ProcessId" });
            DropIndex("dbo.LogItems", new[] { "Level" });
            DropTable("dbo.WarningItems");
            DropTable("dbo.StatisticValues");
            DropTable("dbo.LogItems");
        }
    }
}
