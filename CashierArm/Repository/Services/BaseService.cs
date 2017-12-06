using System;
using CashierArm.Models;

namespace CashierArm.Repository.Services
{
    public class BaseService : IDisposable
    {
        protected readonly CashierArmContext Repository;

        public BaseService(CashierArmContext context)
        {
            Repository = context;
        }

        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}
