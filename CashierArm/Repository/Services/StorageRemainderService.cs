using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm.Repository.Services
{
    public class StorageRemainderService : BaseService, IStorageRemainderService
    {
        public StorageRemainderService(CashierArmContext context) : base(context)
        {

        }

        public List<StorageRemainder> GetAll()
        {
            return Repository.StorageRemainders.ToList();
        }

        public StorageRemainder AddStorageRemainder(StorageRemainder item)
        {
            try
            {
                Repository.StorageRemainders.Add(item);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return item;
        }

        public List<StorageRemainder> AddStorageRemainders(List<StorageRemainder> items)
        {
            try
            {
                Repository.StorageRemainders.AddRange(items);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return items;
        }

        public List<StorageRemainder> UpdateStorageRemainders(List<StorageRemainder> items)
        {
            var result = new List<StorageRemainder>(items.Count);
            if (items.Count == 0) return result;
            try
            {
                //добавляем новые записи в остатках, если раньше таких небыло
                var addItems = items.Where(w => w.Id == 0).ToList();
                if(addItems.Count > 0)
                    result.AddRange(AddStorageRemainders(addItems));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        public void DeleteStorageRemainders(List<StorageRemainder> items)
        {
            if (items.Count == 0) return;

            try
            {
                var deleteItems = items.Where(w => w.Id != 0).ToList();
                Repository.Entry(deleteItems).State = EntityState.Deleted;
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// обновить складские остатки при операции поступления
        /// </summary>
        /// <param name="productOpers"></param>
        /// <returns></returns>
        public List<StorageRemainder> DebitStorageRemainders(List<ProductOperation> productOpers)
        {
            var result = new List<StorageRemainder>(productOpers.Count);
            var isRemainders = Repository.StorageRemainders.Any(a => a.Quantity > 0);
            try
            {
                //проверка входящих данных
                CheckDataBeforeDebitСredit(productOpers);
                var storages = productOpers.Select(s => s.StorageId).Distinct();
                foreach (var storage in storages)
                {                    
                    //получить входящие данные по складам
                    var productOpersByStorage = productOpers.Where(w => w.StorageId == storage).ToList();
                    var productsId = productOpersByStorage.Select(s => s.ProductId).Distinct();

                    //получить остатки в базе, по складам
                    var storageRemainders = isRemainders ? Repository.StorageRemainders.Where(w =>
                        productsId.Contains(w.ProductId) && w.StorageId == storage).ToList() : null;

                    //собрать данные для обновления
                    foreach (var oper in productOpersByStorage)
                    {
                        var remains = storageRemainders?.FirstOrDefault(f =>
                            f.ProductId == oper.ProductId && f.PurchasePrice == oper.Price);
                        if (remains != null)
                        {
                            remains.Quantity += oper.Quantity;
                            result.Add(remains);
                        }
                        else
                            result.Add(new StorageRemainder
                            {
                                StorageId = storage,
                                ProductId = oper.ProductId,
                                PurchasePrice = oper.Price,
                                Quantity = oper.Quantity
                            });
                    }
                }

                var output = UpdateStorageRemainders(result);
                if (output.Count == 0 || output[0].Id == 0)
                    throw new Exception("Не удалось обновить остатки по складам");
                return output;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        public List<StorageRemainder> СreditStorageRemainders(List<ProductOperation> productOpers)
        {
            var result = new List<StorageRemainder>(productOpers.Count);
            var daleteList = new List<StorageRemainder>();
            var isRemainders = Repository.StorageRemainders.Any(a => a.Quantity > 0);
            try
            {
                //проверка входящих данных
                CheckDataBeforeDebitСredit(productOpers);
                var storages = productOpers.Select(s => s.StorageId).Distinct();
                foreach (var storage in storages)
                {
                    //получить входящие данные по складам
                    var productOpersByStorage = productOpers.Where(w => w.StorageId == storage).ToList();
                    var productsId = productOpersByStorage.Select(s => s.ProductId).Distinct();

                    //получить остатки в базе, по складам
                    var storageRemainders = isRemainders ? Repository.StorageRemainders.Where(w =>
                        productsId.Contains(w.ProductId) && w.StorageId == storage).ToList() : null;

                    foreach (var oper in productOpersByStorage)
                    {
                        var remains = storageRemainders?.FirstOrDefault(f =>
                            f.ProductId == oper.ProductId && f.PurchasePrice == oper.Price);
                        if (remains != null)
                        {
                            daleteList.AddRange(SelectDataForUpdate(oper.Quantity, ref remains));
                            result.Add(remains);
                        }
                        else
                            throw new Exception(
                                $"Ошибка при списании остатков по складам: остатков по номенклатуре {oper.Product.Name}, не существует");
                    }
                }

                DeleteStorageRemainders(daleteList);
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// проверка данных перед обновлением складских остатков
        /// </summary>
        /// <param name="productOpers"></param>
        /// <returns></returns>
        private void CheckDataBeforeDebitСredit(List<ProductOperation> productOpers)
        {
            //проверить на дубли
            productOpers.ForEach(item =>
            {
                if (productOpers.Where(w =>
                            w.ProductId == item.ProductId && w.Price == item.Price && w.StorageId == item.StorageId)
                        .ToList().Count > 1)
                    throw new Exception(
                        "Ошибка при записи остатков по складам: Дублирование данных в операциях документа");
            });

            //проверить что тип операции одинаковый
            if (productOpers.Select(s => s.OperationTypeId).Distinct().ToList().Count > 1)
                throw new Exception("Ошибка при записи остатков по складам: тип операции должен быть один");
        }

        /// <summary>
        /// определяет какие записи по остаткам будем удалять и какую запись обновлять,
        /// если количество списания больше чем есть для одной записи
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        private IEnumerable<StorageRemainder> SelectDataForUpdate(decimal quantity, ref StorageRemainder update)
        {
            var deleteList = new List<StorageRemainder>();
            if (update.Quantity < quantity && update.Quantity > 0)
            {
                var newQquantity = quantity - update.Quantity;
                deleteList.Add(update);
                var updateItem = update;
                var remainders = Repository.StorageRemainders
                    .Where(w => w.StorageId == updateItem.StorageId && w.ProductId == updateItem.ProductId).ToList();

                foreach (var remains in remainders)
                {
                    if (remains.Quantity < newQquantity && remains.Quantity > 0)
                    {
                        newQquantity -= remains.Quantity;
                        deleteList.Add(remains);
                    }
                    else if (remains.Quantity <= 0)
                    {
                        deleteList.Add(remains);
                    }
                    else
                    {
                        remains.Quantity -= newQquantity;
                        update = remains;
                        return deleteList;
                    }
                }
            }

            update.Quantity -= quantity;
            return deleteList;
        }
    }
}
