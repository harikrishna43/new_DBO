namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTitleToCompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "Title", c => c.String(maxLength: 127));
            AlterColumn("dbo.Industries", "Name", c => c.String(maxLength: 127));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Industries", "Name", c => c.String());
            DropColumn("dbo.Companies", "Title");
        }
    }
}
