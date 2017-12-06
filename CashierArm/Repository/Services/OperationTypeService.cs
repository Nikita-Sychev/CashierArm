using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm.Repository.Services
{
    public class OperationTypeService : BaseService, IOperationTypeService
    {
        public OperationTypeService(CashierArmContext context) : base(context)
        {

        }

        public List<OperationType> GetAll()
        {
            return Repository.OperationTypes.ToList();
        }

        public OperationType AddOperationType(OperationType item)
        {
            try
            {
                Repository.OperationTypes.Add(item);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return item;
        }

        public List<OperationType> AddOperationTypes(List<OperationType> items)
        {
            try
            {
                Repository.OperationTypes.AddRange(items);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return items;
        }
    }
}
