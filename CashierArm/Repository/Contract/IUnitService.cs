using System.Collections.Generic;
using CashierArm.Models;

namespace CashierArm.Repository.Contract
{
    public interface IUnitService
    {
        List<Unit> GetAll();
        Unit AddUnit(Unit item);
        List<Unit> AddUnits(List<Unit> items);
    }
}
