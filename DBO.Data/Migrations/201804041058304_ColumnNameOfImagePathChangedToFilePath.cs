namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ColumnNameOfImagePathChangedToFilePath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advertisements", "FilePath", c => c.String());
            AddColumn("dbo.Advertisements", "Type", c => c.Int(nullable: false));
            DropColumn("dbo.Advertisements", "ImagePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Advertisements", "ImagePath", c => c.String());
            DropColumn("dbo.Advertisements", "Type");
            DropColumn("dbo.Advertisements", "FilePath");
        }
    }
}
