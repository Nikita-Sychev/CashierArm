using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CashierArm.Models
{
    //View models
    public class SalesItem : INotifyPropertyChanged
    {
        private int _productId;
        private int _storageId;
        private string _name;
        private decimal _quantity;
        private decimal _amount;
        private string _unitShortName;

        public int ProductId
        {
            get => _productId;
            set
            {
                _productId = value;
                OnPropertyChanged("ProductId");
            }
        }

        public int StorageId
        {
            get => _storageId;
            set
            {
                _storageId = value;
                OnPropertyChanged("StorageId");
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public decimal Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged("Amount");
            }
        }

        public string UnitShortName
        {
            get => _unitShortName;
            set
            {
                _unitShortName = value;
                OnPropertyChanged("UnitShortName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProductAndRemains
    {
        public int ProductId { get; set; }
        public int StorageId { get; set; }
        public int UnitId { get; set; }
        public string Name { get; set; }
        public string UnitShortName { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalePrice { get; set; }
    }

    public class DocumentView
    {
        public int DocumentId { get; set; }
        public int DocumentTypeId { get; set; }
        public string TypeName { get; set; }
        public DateTime DocumentDate { get; set; }
        public bool IsOpen { get; set; }
        public int PositionCount { get; set; }
        public decimal Amount { get; set; }
    }
}
