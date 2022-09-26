namespace Universe.Fias.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AsAddrObjs", "Code", c => c.String(maxLength: 20));
            AlterColumn("dbo.AsAddrObjs", "PlainCode", c => c.String(maxLength: 20));
            CreateIndex("dbo.AsAddrObjs", "Code");
            CreateIndex("dbo.AsAddrObjs", "PlainCode");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AsAddrObjs", new[] { "PlainCode" });
            DropIndex("dbo.AsAddrObjs", new[] { "Code" });
            AlterColumn("dbo.AsAddrObjs", "PlainCode", c => c.String());
            AlterColumn("dbo.AsAddrObjs", "Code", c => c.String());
        }
    }
}
