namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Companies3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Companies", "Name", c => c.String(maxLength: 255));
            AlterColumn("dbo.Companies", "Address", c => c.String(maxLength: 255));
            AlterColumn("dbo.Companies", "PostCode", c => c.String(maxLength: 15));
            AlterColumn("dbo.Companies", "City", c => c.String(maxLength: 255));
            AlterColumn("dbo.Companies", "Phone", c => c.String(maxLength: 15));
            AlterColumn("dbo.Companies", "Web", c => c.String(maxLength: 255));
            AlterColumn("dbo.Companies", "Email", c => c.String(maxLength: 255));
            AlterColumn("dbo.Companies", "PersonName", c => c.String(maxLength: 255));
            AlterColumn("dbo.Companies", "Chairman", c => c.String(maxLength: 255));
            AlterColumn("dbo.Companies", "IndustryText", c => c.String(maxLength: 255));
            AlterColumn("dbo.Companies", "Owner", c => c.String(maxLength: 255));
            AlterColumn("dbo.Companies", "Image", c => c.String(maxLength: 255));
            AlterColumn("dbo.Companies", "TextDescription", c => c.String(maxLength: 255));
            CreateIndex("dbo.Companies", "Name");
            CreateIndex("dbo.Companies", "City");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Companies", new[] { "City" });
            DropIndex("dbo.Companies", new[] { "Name" });
            AlterColumn("dbo.Companies", "TextDescription", c => c.String());
            AlterColumn("dbo.Companies", "Image", c => c.String());
            AlterColumn("dbo.Companies", "Owner", c => c.String());
            AlterColumn("dbo.Companies", "IndustryText", c => c.String());
            AlterColumn("dbo.Companies", "Chairman", c => c.String());
            AlterColumn("dbo.Companies", "PersonName", c => c.String());
            AlterColumn("dbo.Companies", "Email", c => c.String());
            AlterColumn("dbo.Companies", "Web", c => c.String());
            AlterColumn("dbo.Companies", "Phone", c => c.String());
            AlterColumn("dbo.Companies", "City", c => c.String());
            AlterColumn("dbo.Companies", "PostCode", c => c.String());
            AlterColumn("dbo.Companies", "Address", c => c.String());
            AlterColumn("dbo.Companies", "Name", c => c.String());
        }
    }
}
