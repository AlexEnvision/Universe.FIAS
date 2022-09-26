namespace Universe.Fias.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFiasCsvFolderPath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AsVersions", "FiasCsvFolderPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AsVersions", "FiasCsvFolderPath");
        }
    }
}
