using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm.Repository.Services
{
    public class DocumentTypeService: BaseService, IDocumentTypeService
    {
        public DocumentTypeService(CashierArmContext context) : base(context)
        {
            
        }

        public List<DocumentType> GetAll()
        {
            return Repository.DocumentTypes.ToList();
        }

        public DocumentType AddDocumentType(DocumentType item)
        {
            try
            {
                Repository.DocumentTypes.Add(item);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return item;
        }

        public List<DocumentType> AddDocumentTypes(List<DocumentType> items)
        {
            try
            {
                Repository.DocumentTypes.AddRange(items);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return items;
        }
    }
}
