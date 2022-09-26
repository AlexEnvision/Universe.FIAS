namespace Universe.Fias.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexesV : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AsHouses", "AoId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AsHouses", new[] { "AoId" });
        }
    }
}
