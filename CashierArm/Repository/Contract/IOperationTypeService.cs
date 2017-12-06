using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashierArm.Models;

namespace CashierArm.Repository.Contract
{
    public interface IOperationTypeService
    {
        List<OperationType> GetAll();
        OperationType AddOperationType(OperationType item);
        List<OperationType> AddOperationTypes(List<OperationType> items);
    }
}
