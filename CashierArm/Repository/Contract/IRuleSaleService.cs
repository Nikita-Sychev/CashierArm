using System.Collections.Generic;
using CashierArm.Models;

namespace CashierArm.Repository.Contract
{
    public interface IRuleSaleService
    {
        List<RuleSale> GetAll();
        RuleSale AddRuleSale(RuleSale item);
        List<RuleSale> AddRuleSales(List<RuleSale> items);
    }
}
