using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CashierArm.Models;
using CashierArm.Repository.Contract;
using CashierArm.Repository.Services;
using Ninject;

namespace CashierArm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKernel container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureContainer();
            ComposeObjects();
            Current.MainWindow.Show();
        }

        private void ConfigureContainer()
        {
            this.container = new StandardKernel();
            container.Bind<IProductService>().To<ProductService>().WithConstructorArgument("context", new CashierArmContext());
            container.Bind<IStorageService>().To<StorageService>().WithConstructorArgument("context", new CashierArmContext());
            container.Bind<IDocumentOperationService>().To<DocumentOperationService>().WithConstructorArgument("context", new CashierArmContext());
            container.Bind<IDocumentService>().To<DocumentService>().WithConstructorArgument("context", new CashierArmContext());
            container.Bind<IDocumentTypeService>().To<DocumentTypeService>().WithConstructorArgument("context", new CashierArmContext());
            container.Bind<IOperationTypeService>().To<OperationTypeService>().WithConstructorArgument("context", new CashierArmContext());
            container.Bind<IProductOperationService>().To<ProductOperationService>().WithConstructorArgument("context", new CashierArmContext());
            container.Bind<IRuleSaleService>().To<RuleSaleService>().WithConstructorArgument("context", new CashierArmContext());
            container.Bind<IStorageRemainderService>().To<StorageRemainderService>().WithConstructorArgument("context", new CashierArmContext());
            container.Bind<IUnitService>().To<UnitService>().WithConstructorArgument("context", new CashierArmContext());
        }

        private void ComposeObjects()
        {
            Current.MainWindow = this.container.Get<MainWindow>();
            Current.MainWindow.Title = "АРМ Кассира";
        }
    }
}
