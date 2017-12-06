using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashierArm.Models;

namespace CashierArm.Repository.Contract
{
    public interface IProductService
    {
        List<Product> GetAll();
        Product AddProduct(Product item);
        List<Product> AddProducts(List<Product> items);
        List<ProductAndRemains> GetAllWithRemains();
    }
}
