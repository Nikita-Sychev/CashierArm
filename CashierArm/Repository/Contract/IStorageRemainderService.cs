using System.Collections.Generic;
using CashierArm.Models;

namespace CashierArm.Repository.Contract
{
    public interface IStorageRemainderService
    {
        List<StorageRemainder> GetAll();
        StorageRemainder AddStorageRemainder(StorageRemainder item);
        List<StorageRemainder> AddStorageRemainders(List<StorageRemainder> items);
        List<StorageRemainder> DebitStorageRemainders(List<ProductOperation> productOpers);
        List<StorageRemainder> СreditStorageRemainders(List<ProductOperation> productOpers);
        void DeleteStorageRemainders(List<StorageRemainder> items);
    }
}
