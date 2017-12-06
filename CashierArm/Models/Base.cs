using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashierArm.Models
{
    /// <summary>
    /// единица измерения для товара
    /// </summary>
    [Table("Unit")]
    public class Unit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }

    /// <summary>
    /// товар (номенклатура)
    /// </summary>
    [Table("Product")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }       

        [ForeignKey("Unit")]
        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; }
    }

    /// <summary>
    /// Склад
    /// </summary>
    [Table("Storage")]
    public class Storage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
    }


    /// <summary>
    /// Складские остатки 
    /// заполняется/изменяется при изменении количества товара в момент операций (Реализация, Поступление и пр.)
    /// необходим для более простого получения данных по текущим остаткам (т.к. не нужно вычислять через данные по оборотам)
    /// </summary>
    [Table("StorageRemainder")]
    public class StorageRemainder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("Storage")]
        public int StorageId { get; set; }
        public virtual Storage Storage { get; set; }

        [DataType("Decimal(10, 3)")]
        public decimal Quantity { get; set; }               //текущее количество по товару

        [DataType("Decimal(10, 3)")]
        public decimal PurchasePrice { get; set; }         //Цена поступления
    }

    /// <summary>
    /// Правила реализации товара
    /// задается цена реализации, по определенному товару на дату
    /// так же можно добавить скидку
    /// (для продукта может быть задана только одна цена с IsActive=true)
    /// </summary>
    [Table("RuleSale")]
    public class RuleSale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [DataType("Decimal(10, 2)")]
        public decimal Price { get; set; }

        [DataType("DateTime")]
        public DateTime RuleDate { get; set; }  //дата с которой действует цена

        public bool IsActive { get; set; } //флаг - актуальности цены
    }

    /// <summary>
    /// Тип операции по документу
    /// </summary>
    [Table("OperationType")]
    public class OperationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDebit { get; set; }   //true - приход, по отношению к складу (пример: Поступление IsDebit=true, Реализация IsDebit=false)
    }

    /// <summary>
    /// Операция с товаром (в зависимости от документа)
    /// (Обороты товара)
    /// </summary>
    [Table("ProductOperation")]
    public class ProductOperation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }        

        [ForeignKey("OperationType")]
        public int OperationTypeId { get; set; }
        public virtual OperationType OperationType { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("Storage")]
        public int StorageId { get; set; }
        public virtual Storage Storage { get; set; }

        [DataType("DateTime")]
        public DateTime OperationDate { get; set; }

        [DataType("Decimal(10, 3)")]
        public decimal Quantity { get; set; }   //количество товаров при поступлении или реализации (10 пакетов молока, 15 Кг яблок)

        [DataType("Decimal(10, 2)")]
        public decimal Price { get; set; }      //цена за единицу товара (поступление одна цена, реализация - другая)
    }

    /// <summary>
    /// Тип документа (Реализация, Поступление, Списание)
    /// </summary>
    [Table("DocumentType")]
    public class DocumentType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }        
    }

    /// <summary>
    /// Документ
    /// </summary>
    [Table("Document")]
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; }

        [DataType("DateTime")]
        public DateTime DocumentDate { get; set; }

        //документу в процессе формирования IsOpen = true, 
        //после внесения данных по оборотам товаров и остаткам по складам (для позиций текущего документа) IsOpen = false
        public bool IsOpen { get; set; }    
    }

    /// <summary>
    /// Операции по документу
    /// связка операций с документом
    /// </summary>
    [Table("DocumentOperation")]
    public class DocumentOperation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Document")]
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; }

        [ForeignKey("ProductOperation")]
        public int ProductOperationId { get; set; }
        public virtual ProductOperation ProductOperation { get; set; }
    }
}
