namespace Universe.Fias.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexesIV : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AsHouses", "HouseNum", c => c.String(maxLength: 24));
            CreateIndex("dbo.AsHouses", "HouseNum");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AsHouses", new[] { "HouseNum" });
            AlterColumn("dbo.AsHouses", "HouseNum", c => c.String());
        }
    }
}
