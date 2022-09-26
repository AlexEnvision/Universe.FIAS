namespace Universe.Fias.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexesIII : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AsAddrObjs", "Name", c => c.String(maxLength: 128));
            CreateIndex("dbo.AsAddrObjs", "Name");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AsAddrObjs", new[] { "Name" });
            AlterColumn("dbo.AsAddrObjs", "Name", c => c.String());
        }
    }
}
