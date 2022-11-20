namespace Universe.Fias.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddXmlSettingsFieldsII : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AsVersions", "IsDownloadedFiasDeltaXml", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AsVersions", "IsDownloadedFiasDeltaXml");
        }
    }
}
