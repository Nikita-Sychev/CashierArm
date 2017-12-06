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
    public class UnitService: BaseService, IUnitService
    {
        public UnitService(CashierArmContext context):base(context)
        {
        }

        public List<Unit> GetAll()
        {
            return Repository.Units.ToList();
        }

        public Unit AddUnit(Unit item)
        {
            try
            {
                Repository.Units.Add(item);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return item;
        }

        public List<Unit> AddUnits(List<Unit> items)
        {
            try
            {
                Repository.Units.AddRange(items);
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
