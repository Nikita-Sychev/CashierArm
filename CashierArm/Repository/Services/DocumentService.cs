using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm.Repository.Services
{
    public class DocumentService : BaseService, IDocumentService
    {
        private readonly IDocumentOperationService _documentOperationService;
        private readonly IProductOperationService _productOperationService;

        public DocumentService(CashierArmContext context) : base(context)
        {
            _documentOperationService = new DocumentOperationService(context);
            _productOperationService = new ProductOperationService(context);
        }

        /// <summary>
        /// получить все документу из БД
        /// </summary>
        /// <returns></returns>
        public List<Document> GetAll()
        {
            return Repository.Documents.ToList();
        }

        /// <summary>
        /// создать документ
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Document AddDocument(Document item)
        {
            try
            {
                Repository.Documents.Add(item);
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return item;
        }

        /// <summary>
        /// получить открытые документу
        /// </summary>
        /// <returns></returns>
        public List<Document> GetIsOpen()
        {
            return Repository.Documents.Where(w => w.IsOpen).ToList();
        }

        /// <summary>
        /// Удалить документ
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public bool Delete(int documentId)
        {
            if (documentId == 0) return false;
            var item = Repository.Documents.FirstOrDefault(w => w.Id == documentId);
            if (item == null || !item.IsOpen) return false;         //закрытые документы удалять не будем
            if (GetDocumentOperations(documentId).Count > 0) return false;  //если по документу есть операции - не удаляем
            try
            {
                Repository.Entry(item).State = EntityState.Deleted;
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        /// <summary>
        /// внести изменения по операциям документа, остаткам на складах и закрыть документ
        /// </summary>
        public Document PostingDocument(List<ProductOperation> items, Document document)
        {
            if (document == null || document.Id == 0 || !document.IsOpen) return document;
            if(items.Count == 0) return CloseDocument(document);
            var operations = AddDocumentOperations(PrepareDocumentOperations(document.Id, _productOperationService.AddProductOperations(items, document.DocumentTypeId)));
            if (operations.Count == 0 || operations[0].Id == 0)
                throw new Exception("Не удалось добавить связку операций по документу");
            return CloseDocument(document);
        }

        /// <summary>
        /// закрыть документ
        /// </summary>
        private Document CloseDocument(Document document)
        {
            document.IsOpen = false;
            try
            {
                Repository.Entry(document).State = EntityState.Modified;
                Repository.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return document;
        }


        /// <summary>
        /// занести в БД связку документа и операций
        /// </summary>
        private List<DocumentOperation> AddDocumentOperations(List<DocumentOperation> items)
        {
            return _documentOperationService.AddDocumentOperations(items);
        }

        /// <summary>
        /// получить список операций по документу
        /// </summary>
        private List<DocumentOperation> GetDocumentOperations(int documentId)
        {
            return _documentOperationService.GetForDocument(documentId);
        }

        private List<DocumentOperation> PrepareDocumentOperations(int documentId, List<ProductOperation> productOperations)
        {
            var result = new List<DocumentOperation>();

            foreach (var oparation in productOperations)
            {
                if (oparation.Id == 0) throw new Exception("Пустой ID операции поступления");
                result.Add(new DocumentOperation { DocumentId = documentId, ProductOperationId = oparation.Id });
            }

            return result;
        }
    }
}
