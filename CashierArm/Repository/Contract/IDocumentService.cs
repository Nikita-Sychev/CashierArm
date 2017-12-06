using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashierArm.Models;

namespace CashierArm.Repository.Contract
{
    public interface IDocumentService
    {
        List<Document> GetAll();
        Document AddDocument(Document item);
        Document PostingDocument(List<ProductOperation> items, Document document);
        List<Document> GetIsOpen();
        bool Delete(int documentId);
    }
}
