using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashierArm.Models;

namespace CashierArm.Repository.Contract
{
    public interface IStorageService
    {
        List<Storage> GetAll();
        Storage AddStorage(Storage item);
    }
}
