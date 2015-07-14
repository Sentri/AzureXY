namespace AzureXY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Owner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boards", "ApplicationUserID", c => c.Int(nullable: false));
            AddColumn("dbo.Boards", "Owner_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Boards", "Owner_Id");
            AddForeignKey("dbo.Boards", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Boards", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Boards", new[] { "Owner_Id" });
            DropColumn("dbo.Boards", "Owner_Id");
            DropColumn("dbo.Boards", "ApplicationUserID");
        }
    }
}
