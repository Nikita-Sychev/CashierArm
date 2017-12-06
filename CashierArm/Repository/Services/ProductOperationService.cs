using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CashierArm.Enums;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm.Repository.Services
{
    public class ProductOperationService : BaseService, IProductOperationService
    {
        private readonly IStorageRemainderService _storageRemainderService;

        public ProductOperationService(CashierArmContext context) : base(context)
        {
            _storageRemainderService = new StorageRemainderService(context);
        }

        public List<ProductOperation> GetAll()
        {
            return Repository.ProductOperations.ToList();
        }

        public List<ProductOperation> AddProductOperations(List<ProductOperation> items, int docType)
        {
            try
            {
                Repository.ProductOperations.AddRange(items);
                switch (docType)
                {
                    case (int)DocumentTypeEn.Receipt:
                        DebitStorageRemainders(items);
                        break;
                    case (int)DocumentTypeEn.Sale:
                        СreditStorageRemainders(items);
                        break;
                }
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return items;
        }

        private bool DebitStorageRemainders(List<ProductOperation> items)
        {
            var result =_storageRemainderService.DebitStorageRemainders(items);
            if (result.Count == 0 || result[0].Id == 0)
                throw new Exception("Не удалось обновить остатки по складам");
            return true;
        }

        private bool СreditStorageRemainders(List<ProductOperation> items)
        {
            var result = _storageRemainderService.СreditStorageRemainders(items);
            if (result.Count == 0 || result[0].Id == 0)
                throw new Exception("Не удалось обновить остатки по складам");
            return true;
        }
    }
}
