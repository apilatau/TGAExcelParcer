namespace WindowsFormsApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTGAMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Initials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        FileCreationDate = c.DateTime(nullable: false),
                        UserTGA = c.String(),
                        InitialMass = c.Double(nullable: false),
                        InPercent = c.Boolean(nullable: false),
                        TreatmentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Treatments", t => t.TreatmentId)
                .Index(t => t.TreatmentId);
            
            CreateTable(
                "dbo.TGAs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SampleTemperature = c.Double(nullable: false),
                        TGAdata = c.Double(nullable: false),
                        InitialId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Initials", t => t.InitialId)
                .Index(t => t.InitialId);
            
            CreateTable(
                "dbo.Treatments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TreatmentDate = c.DateTime(nullable: false),
                        TreatersName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Initials", "TreatmentId", "dbo.Treatments");
            DropForeignKey("dbo.TGAs", "InitialId", "dbo.Initials");
            DropIndex("dbo.TGAs", new[] { "InitialId" });
            DropIndex("dbo.Initials", new[] { "TreatmentId" });
            DropTable("dbo.Treatments");
            DropTable("dbo.TGAs");
            DropTable("dbo.Initials");
        }
    }
}
