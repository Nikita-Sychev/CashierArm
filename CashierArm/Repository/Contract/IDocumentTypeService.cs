using System.Collections.Generic;
using CashierArm.Models;

namespace CashierArm.Repository.Contract
{
    public interface IDocumentTypeService
    {
        List<DocumentType> GetAll();
        DocumentType AddDocumentType(DocumentType item);
        List<DocumentType> AddDocumentTypes(List<DocumentType> items);
    }
}
