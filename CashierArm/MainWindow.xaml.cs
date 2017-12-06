using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CashierArm.Base;
using CashierArm.Enums;
using CashierArm.Models;
using CashierArm.Repository.Contract;

namespace CashierArm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InitializeData _dataInit;
        private readonly IProductService _productService;
        private readonly IStorageService _storageService;
        private readonly IUnitService _unitService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IProductOperationService _productOperationService;
        private readonly IRuleSaleService _ruleSaleService;
        private readonly IOperationTypeService _operationTypeService;
        private readonly IDocumentService _documentService;
        private readonly IStorageRemainderService _storageRemainderService;
        private readonly IDocumentOperationService _documentOperationService;
        private ProductAndRemains _currentItem;
        private ProductAndRemains _currentSelectedItem;
        private List<ProductAndRemains> _nomenclatureList;
        private string _decSeparator;
        private const int QuantityDec = 3;
        private const int CashDec = 2;
        public ObservableCollection<SalesItem> SalesList { get; set; }
        public DocumentView CurrentDocument;
        private Document _document;

        public MainWindow(
            IProductService productService,
            IStorageService storageService,
            IUnitService unitService,
            IDocumentTypeService documentTypeService,
            IProductOperationService productOperationService,
            IRuleSaleService ruleSaleService,
            IOperationTypeService operationTypeService,
            IDocumentService documentService,
            IStorageRemainderService storageRemainderService,
            IDocumentOperationService documentOperationService)
        {
            _productService = productService;
            _storageService = storageService;
            _unitService = unitService;
            _documentTypeService = documentTypeService;
            _productOperationService = productOperationService;
            _ruleSaleService = ruleSaleService;
            _operationTypeService = operationTypeService;
            _documentService = documentService;
            _storageRemainderService = storageRemainderService;
            _documentOperationService = documentOperationService;

            try
            {               
                InitializeData();
                InitializeComponent();
                InitializedParams();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// инициализация переменных окна
        /// </summary>
        private void InitializedParams()
        {
            TbTimeNow.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture).Split(' ').FirstOrDefault() ?? "";
            SalesList = new ObservableCollection<SalesItem>();
            _currentItem = new ProductAndRemains();
            _decSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            UpdateDataDocInPanel();
        }

        /// <summary>
        /// внести в БД данные если она пустая
        /// </summary>
        private void InitializeData()
        {
            _dataInit = new InitializeData(
                _productService, 
                _storageService, 
                _unitService, 
                _documentTypeService,
                _productOperationService,
                _ruleSaleService,
                _operationTypeService,
                _documentService,
                _storageRemainderService,
                _documentOperationService
            );
            _dataInit.Initialize(); //инициализация данных при первой загрузке приложения
            _dataInit.DropOpenDocuments();  //удалить открытые документы
            CreateSaleDocument();
        }

        /// <summary>
        /// событие выбора товара в выпадающем списке
        /// </summary>
        private void NomenclatureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                TbCurrentItemQuantity.Text = string.Empty;
                BtnAddNewItem.IsEnabled = false;
                if (sender is ComboBox cBox)
                {
                    _currentItem = cBox.SelectedItem as ProductAndRemains;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// Получить список номенклатуры
        /// </summary>
        private void NomenclatureList_Initialized(object sender, EventArgs e)
        {
            UpdateNomenclatureList();
        }

        /// <summary>
        /// Контроль введения значения кол-ва товара(символ ввода)
        /// </summary>
        private void TbCurrentItemQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var c = Convert.ToChar(e.Text);
            e.Handled = !char.IsNumber(c) && !c.Equals(_decSeparator[0]);            
            base.OnPreviewTextInput(e);
        }
        /// <summary>
        /// Контроль введения значения кол-ва товара(значения в целом)
        /// </summary>
        private void TbCurrentItemQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (e.Source is TextBox source)
            {
                e.Handled = !CheckEnterQuantity(source.Text, QuantityDec);
                BtnAddNewItem.IsEnabled = !string.IsNullOrEmpty(source.Text);
            }

            if (!e.Handled) return;
            var parts = TbCurrentItemQuantity.Text.Split(_decSeparator[0]);
            TbCurrentItemQuantity.Text = parts[0] + _decSeparator[0] + parts[1].Remove(parts[1].Length - 1, 1);
        }

        /// <summary>
        /// Добавить товар в список покупок
        /// </summary>
        private void BtnAddNewItem_Click(object sender, RoutedEventArgs e)
        {

            var valueQuantity = Convert.ToDecimal(TbCurrentItemQuantity.Text);
            if (valueQuantity > _currentItem.Quantity)
                MessageBox.Show($"Введенное количество товара превышает остаток на складе - \"{_currentItem.Quantity}\"");
            else if (SalesList.Any(a => a.ProductId == _currentItem.ProductId))
            {
                MessageBox.Show("Данный товар уже добавлен");
            }
            else
            {
                AddDocumentPosition(valueQuantity);
            }
        }        

        /// <summary>
        /// событие - закрыть чек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCloseCheck_Click(object sender, RoutedEventArgs e)
        {
            if (!CloseSaleDocument()) return;
            SalesList = new ObservableCollection<SalesItem>();
            ProductList.ItemsSource = SalesList;
            BtnCloseCheck.IsEnabled = false;
            BtnDropList.IsEnabled = false;
            TbCash.Text = "0";
            CreateSaleDocument();
        }

        /// <summary>
        /// Удалить весь список товаров
        /// </summary>
        private void BtnDropList_Click(object sender, RoutedEventArgs e)
        {
            DropAllDocumentPositions();
            UpdateDataSelectedItemInPanel();
        }

        /// <summary>
        /// Контроль введения значения наличных(символ ввода)
        /// </summary>
        private void TbCash_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var c = Convert.ToChar(e.Text);
            e.Handled = !char.IsNumber(c) && !c.Equals(_decSeparator[0]);
            base.OnPreviewTextInput(e);
        }

        /// <summary>
        /// Контроль введения значения наличных(значения в целом)
        /// </summary>
        private void TbCash_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Source is TextBox source)
            {
                var val = !string.IsNullOrEmpty(source.Text)? Convert.ToDecimal(source.Text):0;
                e.Handled = !CheckEnterQuantity(source.Text, CashDec);
                BtnCloseCheck.IsEnabled = !string.IsNullOrEmpty(source.Text) && val >= CurrentDocument.Amount && CurrentDocument.Amount > 0;
                FillBackCash(val);
            }

            if (!e.Handled) return;
            var parts = TbCash.Text.Split(_decSeparator[0]);
            TbCash.Text = parts[0] + _decSeparator[0] + parts[1].Remove(parts[1].Length - 1, 1);
        }

        /// <summary>
        /// Выделение позиции в списке товаров
        /// </summary>
        private void ProductList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is ListView list)) return;
            if (list.SelectedItem is SalesItem item)
                FillSelectedItemData(item);
        }




        /// <summary>
        /// Проверка количества знаков после запятой для количества товара и суммы наличных
        /// </summary>
        /// <param name="newText"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool CheckEnterQuantity(string newText, int count)
        {
            var parts = (newText).Split(_decSeparator[0]);
            if (parts.Length > 1)
                return parts[1].Length <= count;
            return true;
        }

        /// <summary>
        /// расчет сдачи
        /// </summary>
        /// <param name="val"></param>
        private void FillBackCash(decimal val)
        {
            if (BtnCloseCheck.IsEnabled)
                TbBackCash.Text = BtnCloseCheck.IsEnabled
                    ? (val - CurrentDocument.Amount).ToString(CultureInfo.CurrentCulture)
                    : "0";
            else
                TbBackCash.Text = "0";
        }

        /// <summary>
        /// проверка на возможность закрыть чек (если сумма наличных не меньше суммы по документу реализации)
        /// </summary>
        private void CheckCashVolume()
        {
            var cash = string.IsNullOrEmpty(TbCash.Text) ? 0 : Convert.ToDecimal(TbCash.Text);
            BtnCloseCheck.IsEnabled = cash >= CurrentDocument.Amount && CurrentDocument.Amount > 0;
            FillBackCash(cash);
        }

        /// <summary>
        /// Добавление новой позиции в документ(список покупок) и изменение связанных компонентов
        /// </summary>
        /// <param name="valueQuantity"></param>
        private void AddDocumentPosition(decimal valueQuantity)
        {
            var amount = Math.Round(_currentItem.SalePrice * valueQuantity, 2);
            CurrentDocument.Amount += amount;
            CurrentDocument.PositionCount++;
            SalesList.Add(new SalesItem
            {
                StorageId = _currentItem.StorageId,
                Quantity = valueQuantity,
                Name = _currentItem.Name,
                ProductId = _currentItem.ProductId,
                UnitShortName = _currentItem.UnitShortName,
                Amount = amount
            });
            ProductList.ItemsSource = SalesList;
            BtnDropList.IsEnabled = true;
            UpdateDataDocInPanel();
            TbCurrentItemQuantity.Text = string.Empty;
            CheckCashVolume();
        }

        /// <summary>
        /// Удалить весь список товаров
        /// </summary>
        private void DropAllDocumentPositions()
        {
            SalesList = new ObservableCollection<SalesItem>();
            ProductList.ItemsSource = SalesList;
            BtnDropList.IsEnabled = false;
            CurrentDocument.Amount = 0;
            CurrentDocument.PositionCount = 0;
            _currentSelectedItem = null;
            UpdateDataDocInPanel();
            TbCash.Text = "0";
        }        

        /// <summary>
        /// обновить текстовые поля в панели о текущем документе
        /// </summary>
        private void UpdateDataDocInPanel()
        {
            if (TbDocType != null) TbDocType.Text = CurrentDocument?.TypeName ?? string.Empty;
            if (TbDocNumber != null) TbDocNumber.Text = CurrentDocument?.DocumentId.ToString() ?? string.Empty;
            if (TbDocPositions != null) TbDocPositions.Text = CurrentDocument?.PositionCount.ToString() ?? "0";
            if (TbDocAmount != null)
                TbDocAmount.Text = CurrentDocument?.Amount.ToString(CultureInfo.CurrentCulture) ?? "0";
        }

        /// <summary>
        /// Обновит/получить список товаров с текущими остатками
        /// </summary>
        private void UpdateNomenclatureList()
        {
            _nomenclatureList = _productService.GetAllWithRemains();
            if(NomenclatureList != null) NomenclatureList.ItemsSource = _nomenclatureList;
        }

        /// <summary>
        /// заполнить данные по выделенной позиции и обновить текстовые поля
        /// </summary>
        /// <param name="item"></param>
        private void FillSelectedItemData(SalesItem item)
        {
            var salePrice = _nomenclatureList.FirstOrDefault(w => w.ProductId == item.ProductId)?.SalePrice;
            _currentSelectedItem = new ProductAndRemains
            {
                StorageId = item.StorageId,
                Quantity = item.Quantity,
                Name = item.Name,
                ProductId = item.ProductId,
                SalePrice = salePrice ?? 0,
                UnitShortName = item.UnitShortName
            };
            UpdateDataSelectedItemInPanel();
        }

        /// <summary>
        /// обновить текстовые поля в панели о выделенной позиции в списке товаров
        /// </summary>
        private void UpdateDataSelectedItemInPanel()
        {
            if (TbSelectedItemCode != null)
                TbSelectedItemCode.Text = _currentSelectedItem?.ProductId.ToString() ?? string.Empty;
            if (TbSelectedItemCount != null)
                TbSelectedItemCount.Text = _currentSelectedItem?.Quantity.ToString(CultureInfo.CurrentCulture) ?? "0";
            if (TbSelectedItemPrice != null)
                TbSelectedItemPrice.Text = _currentSelectedItem?.SalePrice.ToString(CultureInfo.CurrentCulture) ?? "0";
            if (TbSelectedItemAmount != null)
                TbSelectedItemAmount.Text =
                    (_currentSelectedItem?.SalePrice * _currentSelectedItem?.Quantity)?.ToString(CultureInfo
                        .CurrentCulture) ?? "0";
            if (TbSelectedItemName != null) TbSelectedItemName.Text = _currentSelectedItem?.Name ?? string.Empty;
        }

        /// <summary>
        /// Создает документ реализации
        /// </summary>
        private void CreateSaleDocument()
        {
            _document = _documentService.AddDocument(new Document
            {
                IsOpen = true,
                DocumentTypeId = (int)DocumentTypeEn.Sale,
                DocumentDate = DateTime.Now //пока текущая дата
            });
            if (_document == null || _document.Id == 0) throw new Exception("Не удалось создать документ реализации");
            CurrentDocument = new DocumentView
            {
                IsOpen = false,
                DocumentTypeId = _document.DocumentTypeId,
                DocumentId = _document.Id,
                PositionCount = 0,
                TypeName = "Реализация (Открыт)",
                Amount = 0
            };
            UpdateNomenclatureList();
            UpdateDataDocInPanel();
        }

        /// <summary>
        /// отразить операции по товарам и закрыть документ
        /// </summary>
        private bool CloseSaleDocument()
        {
            var operations = new List<ProductOperation>(SalesList.Count);
            SalesList.ToList().ForEach(item =>
            {
                var salePrice = _nomenclatureList.FirstOrDefault(w => w.ProductId == item.ProductId)?.SalePrice;
                operations.Add(new ProductOperation
                {
                    StorageId = item.StorageId,
                    Quantity = item.Quantity,
                    OperationDate = DateTime.Now,
                    OperationTypeId = (int) OperationTypeEn.Сost,
                    Price = salePrice ?? 0,
                    ProductId = item.ProductId
                });
            });
            var result = _documentService.PostingDocument(operations, _document);
            if (result != null && !result.IsOpen)
            {
                MessageBox.Show($"Документ реализации № {result.Id} - проведен");
                return true;
            }
            MessageBox.Show($"Не удалось провести документ № {_document.Id}");
            return false;
        }
    }
}
