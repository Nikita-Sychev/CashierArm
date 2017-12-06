using System.Collections.Generic;
using CashierArm.Models;

namespace CashierArm.Repository.Contract
{
    public interface IProductOperationService
    {
        List<ProductOperation> GetAll();        
        List<ProductOperation> AddProductOperations(List<ProductOperation> items, int docType);
    }
}
