namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Companies2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "TextDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "TextDescription");
        }
    }
}
