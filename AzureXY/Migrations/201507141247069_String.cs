namespace AzureXY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class String : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Boards", new[] { "Owner_Id" });
            DropColumn("dbo.Boards", "ApplicationUserID");
            RenameColumn(table: "dbo.Boards", name: "Owner_Id", newName: "ApplicationUserID");
            AlterColumn("dbo.Boards", "ApplicationUserID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Boards", "ApplicationUserID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Boards", new[] { "ApplicationUserID" });
            AlterColumn("dbo.Boards", "ApplicationUserID", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Boards", name: "ApplicationUserID", newName: "Owner_Id");
            AddColumn("dbo.Boards", "ApplicationUserID", c => c.Int(nullable: false));
            CreateIndex("dbo.Boards", "Owner_Id");
        }
    }
}
