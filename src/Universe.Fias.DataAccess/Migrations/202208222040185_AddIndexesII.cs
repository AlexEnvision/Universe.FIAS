namespace Universe.Fias.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexesII : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AsAddrObjs", "AoCode", c => c.String(maxLength: 20));
            CreateIndex("dbo.AsAddrObjs", "AoCode");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AsAddrObjs", new[] { "AoCode" });
            AlterColumn("dbo.AsAddrObjs", "AoCode", c => c.String());
        }
    }
}
