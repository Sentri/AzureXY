namespace AzureXY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Virtuals : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Drawings", "Board_ID", "dbo.Boards");
            DropIndex("dbo.Drawings", new[] { "Board_ID" });
            RenameColumn(table: "dbo.Drawings", name: "Board_ID", newName: "BoardID");
            AlterColumn("dbo.Drawings", "BoardID", c => c.Int(nullable: false));
            CreateIndex("dbo.Drawings", "BoardID");
            AddForeignKey("dbo.Drawings", "BoardID", "dbo.Boards", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Drawings", "BoardID", "dbo.Boards");
            DropIndex("dbo.Drawings", new[] { "BoardID" });
            AlterColumn("dbo.Drawings", "BoardID", c => c.Int());
            RenameColumn(table: "dbo.Drawings", name: "BoardID", newName: "Board_ID");
            CreateIndex("dbo.Drawings", "Board_ID");
            AddForeignKey("dbo.Drawings", "Board_ID", "dbo.Boards", "ID");
        }
    }
}
