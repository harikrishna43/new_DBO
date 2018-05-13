namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateRegistrationCodes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegistrationCodes", "Cvr", c => c.Int(nullable: false));
            AddColumn("dbo.RegistrationCodes", "CompanyName", c => c.String(maxLength: 255));
            AddColumn("dbo.RegistrationCodes", "Email", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RegistrationCodes", "Email");
            DropColumn("dbo.RegistrationCodes", "CompanyName");
            DropColumn("dbo.RegistrationCodes", "Cvr");
        }
    }
}
