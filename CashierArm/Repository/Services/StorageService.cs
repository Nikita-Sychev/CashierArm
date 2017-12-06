using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm.Repository.Services
{
    public class StorageService: BaseService, IStorageService
    {
        public StorageService(CashierArmContext context):base(context)
        {
        }

        public List<Storage> GetAll()
        {
            return Repository.Storages.ToList();
        }

        public Storage AddStorage(Storage item)
        {
            try
            {
                Repository.Storages.Add(item);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
            return item;
        }
    }
}
