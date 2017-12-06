using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm.Repository.Services
{
    public class RuleSaleService : BaseService, IRuleSaleService
    {
        public RuleSaleService(CashierArmContext context) : base(context)
        {

        }

        public List<RuleSale> GetAll()
        {
            return Repository.RuleSales.ToList();
        }

        public RuleSale AddRuleSale(RuleSale item)
        {
            try
            {
                Repository.RuleSales.Add(item);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return item;
        }

        public List<RuleSale> AddRuleSales(List<RuleSale> items)
        {
            try
            {
                Repository.RuleSales.AddRange(items);
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
