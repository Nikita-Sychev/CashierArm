namespace CashierArm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentOperation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        ProductOperationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Document", t => t.DocumentId, cascadeDelete: true)
                .ForeignKey("dbo.ProductOperation", t => t.ProductOperationId, cascadeDelete: true)
                .Index(t => t.DocumentId)
                .Index(t => t.ProductOperationId);
            
            CreateTable(
                "dbo.Document",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentTypeId = c.Int(nullable: false),
                        DocumentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DocumentType", t => t.DocumentTypeId, cascadeDelete: true)
                .Index(t => t.DocumentTypeId);
            
            CreateTable(
                "dbo.DocumentType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductOperation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OperationTypeId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        StorageId = c.Int(nullable: false),
                        OperationDate = c.DateTime(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OperationType", t => t.OperationTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Storage", t => t.StorageId, cascadeDelete: true)
                .Index(t => t.OperationTypeId)
                .Index(t => t.ProductId)
                .Index(t => t.StorageId);
            
            CreateTable(
                "dbo.OperationType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsDebit = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Unit", t => t.UnitId, cascadeDelete: true)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.Unit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Storage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RuleSale",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RuleDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.StorageRemainder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        StorageId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Storage", t => t.StorageId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.StorageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StorageRemainder", "StorageId", "dbo.Storage");
            DropForeignKey("dbo.StorageRemainder", "ProductId", "dbo.Product");
            DropForeignKey("dbo.RuleSale", "ProductId", "dbo.Product");
            DropForeignKey("dbo.DocumentOperation", "ProductOperationId", "dbo.ProductOperation");
            DropForeignKey("dbo.ProductOperation", "StorageId", "dbo.Storage");
            DropForeignKey("dbo.ProductOperation", "ProductId", "dbo.Product");
            DropForeignKey("dbo.Product", "UnitId", "dbo.Unit");
            DropForeignKey("dbo.ProductOperation", "OperationTypeId", "dbo.OperationType");
            DropForeignKey("dbo.DocumentOperation", "DocumentId", "dbo.Document");
            DropForeignKey("dbo.Document", "DocumentTypeId", "dbo.DocumentType");
            DropIndex("dbo.StorageRemainder", new[] { "StorageId" });
            DropIndex("dbo.StorageRemainder", new[] { "ProductId" });
            DropIndex("dbo.RuleSale", new[] { "ProductId" });
            DropIndex("dbo.Product", new[] { "UnitId" });
            DropIndex("dbo.ProductOperation", new[] { "StorageId" });
            DropIndex("dbo.ProductOperation", new[] { "ProductId" });
            DropIndex("dbo.ProductOperation", new[] { "OperationTypeId" });
            DropIndex("dbo.Document", new[] { "DocumentTypeId" });
            DropIndex("dbo.DocumentOperation", new[] { "ProductOperationId" });
            DropIndex("dbo.DocumentOperation", new[] { "DocumentId" });
            DropTable("dbo.StorageRemainder");
            DropTable("dbo.RuleSale");
            DropTable("dbo.Storage");
            DropTable("dbo.Unit");
            DropTable("dbo.Product");
            DropTable("dbo.OperationType");
            DropTable("dbo.ProductOperation");
            DropTable("dbo.DocumentType");
            DropTable("dbo.Document");
            DropTable("dbo.DocumentOperation");
        }
    }
}
