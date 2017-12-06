using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm.Repository.Services
{
    public class DocumentOperationService : BaseService, IDocumentOperationService
    {
        public DocumentOperationService(CashierArmContext context) : base(context)
        {

        }

        public List<DocumentOperation> GetAll()
        {
            return Repository.DocumentOperations.ToList();
        }

        public DocumentOperation AddDocumentOperation(DocumentOperation item)
        {
            try
            {
                Repository.DocumentOperations.Add(item);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return item;
        }

        public List<DocumentOperation> AddDocumentOperations(List<DocumentOperation> items)
        {
            try
            {
                Repository.DocumentOperations.AddRange(items);                
                Repository.SaveChanges();
                if (items.Count == 0 || items[0].Id == 0)
                    throw new Exception("Не удалось создать связку документа и операций");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return items;
        }

        public List<DocumentOperation> GetForDocument(int documentId)
        {
            return Repository.DocumentOperations.Where(w => w.DocumentId == documentId).ToList();
        }
    }
}
