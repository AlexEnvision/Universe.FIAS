namespace Universe.Fias.DataAccess.Npg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitializeDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AsActStatuses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Actstatid = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AsAddrObjs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AoId = c.Guid(nullable: false),
                        ParentAoId = c.Guid(),
                        DivType = c.Int(),
                        AoLevel = c.Int(),
                        TypeShortName = c.String(),
                        Name = c.String(maxLength: 128),
                        OffName = c.String(),
                        Code = c.String(maxLength: 20),
                        PlainCode = c.String(maxLength: 20),
                        PostalCode = c.String(),
                        OKATO = c.String(),
                        OKTMO = c.String(),
                        LiveStatusId = c.Int(nullable: false),
                        UpdateDate = c.DateTime(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        TypeCode = c.String(),
                        AoCode = c.String(maxLength: 20),
                        ActStatusId = c.Long(),
                        CentStatusId = c.Long(),
                        OperStatusId = c.Long(),
                        NextId = c.Guid(),
                        PrevId = c.Guid(),
                        TypeId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AsActStatuses", t => t.ActStatusId)
                .ForeignKey("dbo.AsCenterStatuses", t => t.CentStatusId)
                .ForeignKey("dbo.AsOperStatuses", t => t.OperStatusId)
                .ForeignKey("dbo.AsAddrObjTypes", t => t.TypeId)
                .Index(t => t.Name)
                .Index(t => t.Code)
                .Index(t => t.PlainCode)
                .Index(t => t.AoCode)
                .Index(t => t.ActStatusId)
                .Index(t => t.CentStatusId)
                .Index(t => t.OperStatusId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.AsCenterStatuses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Centerstid = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AsOperStatuses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Operstatid = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AsAddrObjTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Level = c.Int(),
                        ShortName = c.String(),
                        Name = c.String(),
                        ThereAreStreets = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AsEstStatuses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                        Eststatid = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AsHouses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        HouseId = c.Guid(nullable: false),
                        AoId = c.Guid(nullable: false),
                        DivType = c.Int(),
                        HouseNum = c.String(maxLength: 24),
                        BuildNum = c.String(),
                        StrucNum = c.String(),
                        PostalCode = c.String(),
                        OKATO = c.String(),
                        OKTMO = c.String(),
                        UpdateDate = c.DateTime(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        RegionCode = c.String(),
                        EstStatusId = c.Long(),
                        StrStatusId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AsEstStatuses", t => t.EstStatusId)
                .ForeignKey("dbo.AsStrStatuses", t => t.StrStatusId)
                .Index(t => t.AoId)
                .Index(t => t.HouseNum)
                .Index(t => t.EstStatusId)
                .Index(t => t.StrStatusId);
            
            CreateTable(
                "dbo.AsStrStatuses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                        Strstatid = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AsVersionFiles",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FileName = c.String(),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        StartDateProcessing = c.DateTimeOffset(precision: 7),
                        EndDateProcessed = c.DateTimeOffset(precision: 7),
                        IsProcessed = c.Boolean(),
                        Error = c.String(),
                        VersionId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AsVersions", t => t.VersionId)
                .Index(t => t.VersionId);
            
            CreateTable(
                "dbo.AsVersions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        VersionId = c.Int(nullable: false),
                        TextVersion = c.String(),
                        FiasFullDbfUrl = c.String(),
                        IsDownloadedFiasFullDbf = c.Boolean(),
                        FiasDeltaDbfUrl = c.String(),
                        IsDownloadedFiasDeltaDbf = c.Boolean(),
                        IsProcesed = c.Boolean(),
                        IsVersionFromFiasService = c.Boolean(nullable: false),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                        StartDateDownload = c.DateTimeOffset(precision: 7),
                        EndDateDownload = c.DateTimeOffset(precision: 7),
                        FiasDbfArchivePath = c.String(),
                        FiasDbfFolderPath = c.String(),
                        FiasCsvFolderPath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AsVersionFiles", "VersionId", "dbo.AsVersions");
            DropForeignKey("dbo.AsHouses", "StrStatusId", "dbo.AsStrStatuses");
            DropForeignKey("dbo.AsHouses", "EstStatusId", "dbo.AsEstStatuses");
            DropForeignKey("dbo.AsAddrObjs", "TypeId", "dbo.AsAddrObjTypes");
            DropForeignKey("dbo.AsAddrObjs", "OperStatusId", "dbo.AsOperStatuses");
            DropForeignKey("dbo.AsAddrObjs", "CentStatusId", "dbo.AsCenterStatuses");
            DropForeignKey("dbo.AsAddrObjs", "ActStatusId", "dbo.AsActStatuses");
            DropIndex("dbo.AsVersionFiles", new[] { "VersionId" });
            DropIndex("dbo.AsHouses", new[] { "StrStatusId" });
            DropIndex("dbo.AsHouses", new[] { "EstStatusId" });
            DropIndex("dbo.AsHouses", new[] { "HouseNum" });
            DropIndex("dbo.AsHouses", new[] { "AoId" });
            DropIndex("dbo.AsAddrObjs", new[] { "TypeId" });
            DropIndex("dbo.AsAddrObjs", new[] { "OperStatusId" });
            DropIndex("dbo.AsAddrObjs", new[] { "CentStatusId" });
            DropIndex("dbo.AsAddrObjs", new[] { "ActStatusId" });
            DropIndex("dbo.AsAddrObjs", new[] { "AoCode" });
            DropIndex("dbo.AsAddrObjs", new[] { "PlainCode" });
            DropIndex("dbo.AsAddrObjs", new[] { "Code" });
            DropIndex("dbo.AsAddrObjs", new[] { "Name" });
            DropTable("dbo.AsVersions");
            DropTable("dbo.AsVersionFiles");
            DropTable("dbo.AsStrStatuses");
            DropTable("dbo.AsHouses");
            DropTable("dbo.AsEstStatuses");
            DropTable("dbo.AsAddrObjTypes");
            DropTable("dbo.AsOperStatuses");
            DropTable("dbo.AsCenterStatuses");
            DropTable("dbo.AsAddrObjs");
            DropTable("dbo.AsActStatuses");
        }
    }
}
