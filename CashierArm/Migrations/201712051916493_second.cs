namespace CashierArm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Product", "Quantity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Product", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
