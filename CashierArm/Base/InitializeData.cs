using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CashierArm.Enums;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm.Base
{
    /// <summary>
    /// класс заносить в Базу начальные данные (заполняет справочники)
    /// реализует первое поступление товара
    /// </summary>
    public class InitializeData
    {
        private readonly IProductService _productService;
        private readonly IStorageService _storageService;
        private readonly IUnitService _unitService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IProductOperationService _productOperationService;
        private readonly IRuleSaleService _ruleSaleService;
        private readonly IOperationTypeService _operationTypeService;
        private readonly IDocumentService _documentService;
        private readonly IStorageRemainderService _storageRemainderService;
        private readonly IDocumentOperationService _documentOperationService;

        public InitializeData(
            IProductService productService, 
            IStorageService storageService, 
            IUnitService unitService,
            IDocumentTypeService documentTypeService, 
            IProductOperationService productOperationService,
            IRuleSaleService ruleSaleService,
            IOperationTypeService operationTypeService,
            IDocumentService documentService,
            IStorageRemainderService storageRemainderService,
            IDocumentOperationService documentOperationService)
        {
            _productService = productService;
            _storageService = storageService;
            _unitService = unitService;
            _documentTypeService = documentTypeService;
            _productOperationService = productOperationService;
            _ruleSaleService = ruleSaleService;
            _operationTypeService = operationTypeService;
            _documentService = documentService;
            _storageRemainderService = storageRemainderService;
            _documentOperationService = documentOperationService;
        }

        /// <summary>
        /// т.к. при запуске программы создается документ, а при закрытии не удаляется
        /// Будем удалять не закрытые документы при откурытии приложения, т.к. они не нужны (только для текущего решения)
        /// </summary>
        public void DropOpenDocuments()
        {
            var openDocs =_documentService.GetIsOpen();
            openDocs.ForEach(item => _documentService.Delete(item.Id));
        }

        /// <summary>
        /// Заведем в базу первоначальные данные при первой загрузке приложения
        /// </summary>
        public void Initialize()
        {
            if (_storageService.GetAll().Count != 0) return;

            var storage = _storageService.AddStorage(new Storage {Name = "Основной склад"});
            if(storage.Id == 0) throw new Exception("Ошибка при инициализации данных в базе: не удалось создать склад");

            //заполнить единицы измерения
            var units = FillUnits();            
            //заполнить возможные продукты
            var products = FillProducts(units);
            //заполнить правило реализации товара (цены реализации)
            FillRuleSales(products);
            //заполнить типы операции
            var operationTypes = FillOperationTypes();
            //заполнить типы документа
            var documentTypes = FillDocumentTypes();

            //---Создадим поступление товара
            //задать операцию поступления по документу (Приход)
            var operationTypeId = operationTypes.FirstOrDefault(f => f.IsDebit)?.Id;            
            //задать тип документа
            var documentTypeId = documentTypes.FirstOrDefault(f => f.Name.Equals("Поступление", StringComparison.CurrentCultureIgnoreCase))?.Id;
            //Создать первый документ поступления 
            CreateDocumentReceipt(documentTypeId, PrepareOperationsArrivalVar1(products, storage.Id, operationTypeId));
            //Создать второй документ поступления 
            CreateDocumentReceipt(documentTypeId, PrepareOperationsArrivalVar2(products, storage.Id, operationTypeId));
            //Создать третий документ поступления 
            CreateDocumentReceipt(documentTypeId, PrepareOperationsArrivalVar3(products, storage.Id, operationTypeId));
        }

        private List<Unit> FillUnits()
        {
            return _unitService.AddUnits(new List<Unit>
            {
                new Unit {Name = "Килограмм", ShortName = "Кг"},
                new Unit {Name = "Литр", ShortName = "Л"},
                new Unit {Name = "Штука", ShortName = "Шт"},
                new Unit {Name = "Упаковка", ShortName = "Уп"},
                new Unit {Name = "Пачка", ShortName = "Пч"}
            });
        }

        private List<Product> FillProducts(List<Unit> units)
        {
            var pieceId = units
                .FirstOrDefault(f => f.Name.Equals("штука", StringComparison.CurrentCultureIgnoreCase))?.Id;
            if (pieceId == null || pieceId == 0)
                throw new Exception("Ошибка при инициализации данных в базе: не удалось получить иед. изм.");
            var kilo = units.FirstOrDefault(
                           f => f.Name.Equals("килограмм", StringComparison.CurrentCultureIgnoreCase))?.Id ??
                       (int)pieceId;
            var pack = units.FirstOrDefault(
                           f => f.Name.Equals("упаковка", StringComparison.CurrentCultureIgnoreCase))?.Id ??
                       (int)pieceId;

            return _productService.AddProducts(new List<Product>
            {
                new Product {Name = "Докторская колбаса", UnitId = kilo},
                new Product {Name = "Краковская колбаса", UnitId = kilo},
                new Product {Name = "Мороженое", UnitId = (int)pieceId},
                new Product {Name = "Хлеб пшеничный", UnitId = (int)pieceId},
                new Product {Name = "Хлеб ржаной", UnitId = (int)pieceId},
                new Product {Name = "Молоко", UnitId = pack},
                new Product {Name = "Кефир", UnitId = pack},
                new Product {Name = "Сметана", UnitId = pack},
                new Product {Name = "Конфеты", UnitId = kilo},
                new Product {Name = "Сыр", UnitId = kilo},
                new Product {Name = "Чай", UnitId = pack},
                new Product {Name = "Крупа гречневая", UnitId = kilo},
                new Product {Name = "Рис", UnitId = kilo},
                new Product {Name = "Яблоки", UnitId = kilo},
                new Product {Name = "Бананы", UnitId = kilo},
                new Product {Name = "Мандарины", UnitId = kilo},
                new Product {Name = "Картофель", UnitId = kilo},
                new Product {Name = "Морковь", UnitId = kilo},
                new Product {Name = "Яйцо куриное", UnitId = (int)pieceId}
            });
        }

        private List<OperationType> FillOperationTypes()
        {
            return _operationTypeService.AddOperationTypes(new List<OperationType>
            {
                new OperationType {Name = "Приход", IsDebit = true},
                new OperationType {Name = "Расход", IsDebit = false}
            });
        }

        private List<DocumentType> FillDocumentTypes()
        {
            return _documentTypeService.AddDocumentTypes(new List<DocumentType>
            {
                new DocumentType {Name = "Поступление"},
                new DocumentType {Name = "Реализация"},
                new DocumentType {Name = "Списание"}
            });
        }

        private void FillRuleSales(List<Product> products)
        {
            var ruleSales = new List<RuleSale>(products.Count);
            foreach (var product in products)
            {
                decimal price = 0;
                if (product.Name.Equals("Докторская колбаса", StringComparison.CurrentCultureIgnoreCase))
                    price = 150;
                else if (product.Name.Equals("Краковская колбаса", StringComparison.CurrentCultureIgnoreCase))
                    price = 200;
                else if (product.Name.Equals("Мороженое", StringComparison.CurrentCultureIgnoreCase))
                    price = 20.90m;
                else if (product.Name.Equals("Хлеб пшеничный", StringComparison.CurrentCultureIgnoreCase))
                    price = 20;
                else if (product.Name.Equals("Хлеб ржаной", StringComparison.CurrentCultureIgnoreCase))
                    price = 20;
                else if (product.Name.Equals("Молоко", StringComparison.CurrentCultureIgnoreCase))
                    price = 45;
                else if (product.Name.Equals("Кефир", StringComparison.CurrentCultureIgnoreCase))
                    price = 50;
                else if (product.Name.Equals("Сметана", StringComparison.CurrentCultureIgnoreCase))
                    price = 70;
                else if (product.Name.Equals("Конфеты", StringComparison.CurrentCultureIgnoreCase))
                    price = 35;
                else if (product.Name.Equals("Сыр", StringComparison.CurrentCultureIgnoreCase))
                    price = 340;
                else if (product.Name.Equals("Чай", StringComparison.CurrentCultureIgnoreCase))
                    price = 121.55m;
                else if (product.Name.Equals("Крупа гречневая", StringComparison.CurrentCultureIgnoreCase))
                    price = 45.70m;
                else if (product.Name.Equals("Рис", StringComparison.CurrentCultureIgnoreCase))
                    price = 50.75m;
                else if (product.Name.Equals("Яблоки", StringComparison.CurrentCultureIgnoreCase))
                    price = 80;
                else if (product.Name.Equals("Бананы", StringComparison.CurrentCultureIgnoreCase))
                    price = 56;
                else if (product.Name.Equals("Мандарины", StringComparison.CurrentCultureIgnoreCase))
                    price = 99.90m;
                else if (product.Name.Equals("Картофель", StringComparison.CurrentCultureIgnoreCase))
                    price = 30;
                else if (product.Name.Equals("Морковь", StringComparison.CurrentCultureIgnoreCase))
                    price = 50;
                else if (product.Name.Equals("Яйцо куриное", StringComparison.CurrentCultureIgnoreCase))
                    price = 4.50m;

                ruleSales.Add(new RuleSale
                {
                    IsActive = true,
                    ProductId = product.Id,
                    Price = price,
                    RuleDate = DateTime.Now
                });
            }
            _ruleSaleService.AddRuleSales(ruleSales);
        }

        private void CreateDocumentReceipt(int? documentTypeId, List<ProductOperation> productOperations)
        {
            if (documentTypeId == null || documentTypeId == 0) throw new Exception("Ошибка при инициализации данных в базе: не удалось получить ID типа документа");
            
            //Создать документ
            var document = _documentService.AddDocument(new Document{DocumentDate = DateTime.Now, DocumentTypeId = (int)documentTypeId, IsOpen = true});
            if (document.Id == 0) throw new Exception("Ошибка при инициализации данных в базе: не удалось создать документ");

            //занести в БД операции, закрыть документ
            _documentService.PostingDocument(productOperations, document);
        }


        /// <summary>
        /// Операции поступления
        /// ВАРИАНТ 1
        /// </summary>
        private List<ProductOperation> PrepareOperationsArrivalVar1(List<Product> products, int storageId, int? operationTypeId)
        {            
            var result = new List<ProductOperation>
            {
                FillProductOperations(products, "Докторская колбаса", storageId, operationTypeId, 150, 11),
                FillProductOperations(products, "Краковская колбаса", storageId, operationTypeId, 200, 15),
                FillProductOperations(products, "Мороженое", storageId, operationTypeId, 20.90m, 14),
                FillProductOperations(products, "Хлеб пшеничный", storageId, operationTypeId, 20, 23),
                FillProductOperations(products, "Хлеб ржаной", storageId, operationTypeId, 20, 15),
                FillProductOperations(products, "Молоко", storageId, operationTypeId, 45, 11),
                FillProductOperations(products, "Кефир", storageId, operationTypeId, 50, 7),
                FillProductOperations(products, "Сметана", storageId, operationTypeId, 70, 13),
                FillProductOperations(products, "Конфеты", storageId, operationTypeId, 35, 7),
                FillProductOperations(products, "Сыр", storageId, operationTypeId, 340, 8),
                FillProductOperations(products, "Чай", storageId, operationTypeId, 121.55m, 30),
                FillProductOperations(products, "Крупа гречневая", storageId, operationTypeId, 45.70m, 21),
                FillProductOperations(products, "Рис", storageId, operationTypeId, 50.75m, 18),
                FillProductOperations(products, "Яблоки", storageId, operationTypeId, 80, 37),
                FillProductOperations(products, "Бананы", storageId, operationTypeId, 56, 75),
                FillProductOperations(products, "Мандарины", storageId, operationTypeId, 99.90m, 100),
                FillProductOperations(products, "Картофель", storageId, operationTypeId, 30, 150),
                FillProductOperations(products, "Морковь", storageId, operationTypeId, 50, 37),
                FillProductOperations(products, "Яйцо куриное", storageId, operationTypeId, 4.50m, 200)
            };            
            return result;
        }

        /// <summary>
        /// Операции поступления
        /// ВАРИАНТ 2
        /// </summary>
        private List<ProductOperation> PrepareOperationsArrivalVar2(List<Product> products, int storageId, int? operationTypeId)
        {
            var result = new List<ProductOperation>
            {
                FillProductOperations(products, "Мороженое", storageId, operationTypeId, 23.90m, 5),
                FillProductOperations(products, "Хлеб пшеничный", storageId, operationTypeId, 15, 14),
                FillProductOperations(products, "Кефир", storageId, operationTypeId, 47, 5),
                FillProductOperations(products, "Конфеты", storageId, operationTypeId, 35, 7),
                FillProductOperations(products, "Рис", storageId, operationTypeId, 34.75m, 30),
                FillProductOperations(products, "Яблоки", storageId, operationTypeId, 57, 15),
                FillProductOperations(products, "Бананы", storageId, operationTypeId, 44, 39),
                FillProductOperations(products, "Морковь", storageId, operationTypeId, 50, 24),
                FillProductOperations(products, "Яйцо куриное", storageId, operationTypeId, 3.70m, 75)
            };
            return result;
        }

        /// <summary>
        /// Операции поступления
        /// ВАРИАНТ 3
        /// </summary>
        private List<ProductOperation> PrepareOperationsArrivalVar3(List<Product> products, int storageId, int? operationTypeId)
        {
            var result = new List<ProductOperation>
            {
                FillProductOperations(products, "Докторская колбаса", storageId, operationTypeId, 145, 7),
                FillProductOperations(products, "Молоко", storageId, operationTypeId, 47, 8),
                FillProductOperations(products, "Кефир", storageId, operationTypeId, 50, 3),
                FillProductOperations(products, "Сметана", storageId, operationTypeId, 73, 15),
                FillProductOperations(products, "Конфеты", storageId, operationTypeId, 35, 2),
                FillProductOperations(products, "Крупа гречневая", storageId, operationTypeId, 37.75m, 7),
                FillProductOperations(products, "Рис", storageId, operationTypeId, 50.75m, 9),
                FillProductOperations(products, "Яблоки", storageId, operationTypeId, 57, 15),
                FillProductOperations(products, "Бананы", storageId, operationTypeId, 60, 17),
                FillProductOperations(products, "Мандарины", storageId, operationTypeId, 157.15m, 27),
            };
            return result;
        }

        private ProductOperation FillProductOperations(List<Product> products, string productName, int storageId, int? operationTypeId, decimal price, decimal quantity)
        {
            if (storageId == 0 || operationTypeId == 0 || operationTypeId == null) throw new Exception("Ошибка при инициализации данных в базе: пустой ID склада или операции над товаром");
            var productId = products
                .FirstOrDefault(f => f.Name.Equals(productName, StringComparison.CurrentCultureIgnoreCase))?.Id;
            if(productId == null) throw new Exception("Ошибка при инициализации данных в базе: продукта с таким наименованием не существует в справочнике товаров");

            return new ProductOperation
            {
                OperationDate = DateTime.Now,
                OperationTypeId = (int)operationTypeId,
                Price = price,
                ProductId = (int) productId,
                Quantity = quantity,
                StorageId = storageId
            };
        }

    }
}
