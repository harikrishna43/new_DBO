namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateRegistrationCodes1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RegistrationCodes", "CompanyId", "dbo.Companies");
            DropIndex("dbo.RegistrationCodes", new[] { "CompanyId" });
            AddColumn("dbo.RegistrationCodes", "Name", c => c.String(maxLength: 255));
            AddColumn("dbo.RegistrationCodes", "IsCompany", c => c.Boolean(nullable: false, defaultValue: true));
            AlterColumn("dbo.RegistrationCodes", "Generated", c => c.DateTime());
            AlterColumn("dbo.RegistrationCodes", "CompanyId", c => c.Int());
            CreateIndex("dbo.RegistrationCodes", "CompanyId");
            AddForeignKey("dbo.RegistrationCodes", "CompanyId", "dbo.Companies", "Id");
            DropColumn("dbo.RegistrationCodes", "CompanyName");
        }

        public override void Down()
        {
            AddColumn("dbo.RegistrationCodes", "CompanyName", c => c.String(maxLength: 255));
            DropForeignKey("dbo.RegistrationCodes", "CompanyId", "dbo.Companies");
            DropIndex("dbo.RegistrationCodes", new[] { "CompanyId" });
            AlterColumn("dbo.RegistrationCodes", "CompanyId", c => c.Int(nullable: false));
            AlterColumn("dbo.RegistrationCodes", "Generated", c => c.DateTime(nullable: false));
            DropColumn("dbo.RegistrationCodes", "IsCompany");
            DropColumn("dbo.RegistrationCodes", "Name");
            CreateIndex("dbo.RegistrationCodes", "CompanyId");
            AddForeignKey("dbo.RegistrationCodes", "CompanyId", "dbo.Companies", "Id", cascadeDelete: true);
        }
    }
}
