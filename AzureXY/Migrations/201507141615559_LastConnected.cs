namespace AzureXY.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastConnected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boards", "LastConnected", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Boards", "LastConnected");
        }
    }
}
