namespace Universe.Fias.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddXmlSettingsFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AsVersions", "FiasXmlFolderPath", c => c.String());
            AddColumn("dbo.AsVersions", "FiasXmlArchivePath", c => c.String());
            AddColumn("dbo.AsVersions", "IsDownloadedFiasFullXml", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AsVersions", "IsDownloadedFiasFullXml");
            DropColumn("dbo.AsVersions", "FiasXmlArchivePath");
            DropColumn("dbo.AsVersions", "FiasXmlFolderPath");
        }
    }
}
