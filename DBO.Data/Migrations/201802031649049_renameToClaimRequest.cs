namespace DBO.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameToClaimRequest : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Registrations", newName: "ClaimRequests");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ClaimRequests", newName: "Registrations");
        }
    }
}
