namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCountryToCompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "Country", c => c.String(maxLength: 127));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "Country");
        }
    }
}
