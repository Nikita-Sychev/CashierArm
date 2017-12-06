using System.Collections.Generic;
using CashierArm.Models;

namespace CashierArm.Repository.Contract
{
    public interface IDocumentOperationService
    {
        List<DocumentOperation> GetAll();
        DocumentOperation AddDocumentOperation(DocumentOperation item);
        List<DocumentOperation> AddDocumentOperations(List<DocumentOperation> items);
        List<DocumentOperation> GetForDocument(int documentId);
    }
}
