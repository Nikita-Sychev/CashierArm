using System.Data.Entity;

namespace CashierArm.Models
{
    //Класс контекста БД
    public class CashierArmContext : DbContext
    {
        public CashierArmContext() : base("DefaultConnection")
        {
            // Указывает EF, что если модель изменилась,
            // нужно воссоздать базу данных с новой структурой
            Database.SetInitializer(
                new DropCreateDatabaseIfModelChanges<CashierArmContext>());
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<ProductOperation> ProductOperations { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentOperation> DocumentOperations { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<RuleSale> RuleSales { get; set; }
        public DbSet<StorageRemainder> StorageRemainders { get; set; }
    }
}
