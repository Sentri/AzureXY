namespace AzureXY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QueueRestructure : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Drawings", "BoardID", "dbo.Boards");
            DropIndex("dbo.Drawings", new[] { "BoardID" });
            CreateTable(
                "dbo.DrawingQueues",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DrawingID = c.Int(nullable: false),
                        QueueTime = c.DateTime(nullable: false),
                        BoardID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Boards", t => t.BoardID, cascadeDelete: true)
                .Index(t => t.BoardID);
            
            AddColumn("dbo.Drawings", "Name", c => c.String());
            DropColumn("dbo.Drawings", "Queued");
            DropColumn("dbo.Drawings", "BoardID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Drawings", "BoardID", c => c.Int(nullable: false));
            AddColumn("dbo.Drawings", "Queued", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.DrawingQueues", "BoardID", "dbo.Boards");
            DropIndex("dbo.DrawingQueues", new[] { "BoardID" });
            DropColumn("dbo.Drawings", "Name");
            DropTable("dbo.DrawingQueues");
            CreateIndex("dbo.Drawings", "BoardID");
            AddForeignKey("dbo.Drawings", "BoardID", "dbo.Boards", "ID", cascadeDelete: true);
        }
    }
}
