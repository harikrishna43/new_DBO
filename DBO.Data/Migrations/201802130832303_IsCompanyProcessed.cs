namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsCompanyProcessed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "IsProcessed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "IsProcessed");
        }
    }
}
