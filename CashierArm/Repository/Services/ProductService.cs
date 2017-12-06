using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm.Repository.Services
{
    public class ProductService: BaseService, IProductService
    {
        private readonly IStorageRemainderService _storageRemainderService;
        private readonly IUnitService _unitService;
        private readonly IStorageService _storageService;
        private readonly IRuleSaleService _ruleSaleService;

        public ProductService(CashierArmContext context): base(context)
        {
            _storageRemainderService = new StorageRemainderService(context);
            _unitService = new UnitService(context);
            _storageService = new StorageService(context);
            _ruleSaleService = new RuleSaleService(context);
        }

        public List<Product> GetAll()
        {
            return Repository.Products.ToList();
        }

        public Product AddProduct(Product item)
        {
            try
            {
                Repository.Products.Add(item);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return item;
        }

        public List<Product> AddProducts(List<Product> items)
        {
            try
            {
                Repository.Products.AddRange(items);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return items;
        }

        /// <summary>
        /// Список товаров по складам с остатками
        /// </summary>
        /// <returns></returns>
        public List<ProductAndRemains> GetAllWithRemains()
        {
            var storages = _storageService.GetAll();
            var storageRemainders = _storageRemainderService.GetAll();
            var productsId = storageRemainders.Select(s => s.ProductId).Distinct().ToList();
            var products = Repository.Products.Where(w => productsId.Contains(w.Id)).ToList();
            var unitsId = products.Select(s => s.UnitId).Distinct().ToList();
            var units = _unitService.GetAll().Where(w => unitsId.Contains(w.Id)).ToList();
            var saleRule = _ruleSaleService.GetAll().Where(w => productsId.Contains(w.ProductId)).ToList();
            if (units == null)
                throw new Exception(
                    "Не удалось получить единицы измерения при формировании списка товаров с остатками по складам");
            return (storages.SelectMany(storage => products, (storage, product) => new {storage, product})
                .Select(s => new {s, unit = units.FirstOrDefault(w => w.Id == s.product.UnitId)})
                .Select(s2 => new
                {
                    s2,
                    quantity = storageRemainders
                        .Where(f => f.ProductId == s2.s.product.Id && f.StorageId == s2.s.storage.Id)
                        .Select(s => s.Quantity)
                        .Sum(),
                    price = saleRule.FirstOrDefault(f => f.ProductId == s2.s.product.Id)?.Price ?? throw new Exception(
                                $"Не удалось получить цену реализации товара {s2.s.product.Name}")
                })
                .Select(s3 => new ProductAndRemains
                {
                    Name = s3.s2.s.product.Name,
                    Quantity = s3.quantity,
                    StorageId = s3.s2.s.storage.Id,
                    ProductId = s3.s2.s.product.Id,
                    UnitId = s3.s2.s.product.UnitId,
                    UnitShortName = s3.s2.unit.ShortName ?? "пусто",
                    SalePrice = s3.price
                })).ToList();
        }
    }
}
