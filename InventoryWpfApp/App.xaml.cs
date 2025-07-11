using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.Repositories.Helpers;
using InventoryWpfApp.Repositories.Implementations;
using InventoryWpfApp.ViewModels.Implementations;
using System.Windows;
using System.Windows.Controls;

namespace InventoryWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Setup Dependency Injection Container (basic manual setup)
            // Register DbConnectionFactory
            IDbConnectionFactory connectionFactory = new SqlConnectionFactory();

            // Register Repositories
            IEmployeeRepository employeeRepository = new EmployeeRepository(connectionFactory);
            IGroupRepository groupRepository = new GroupRepository(connectionFactory);
            IEmployeeTypeRepository employeeTypeRepository = new EmployeeTypeRepository(connectionFactory);
            IProductRepository productRepository = new ProductRepository(connectionFactory);
            ISizeRepository sizeRepository = new SizeRepository(connectionFactory);
            IInventoryStockRepository inventoryStockRepository = new InventoryStockRepository(connectionFactory);
            IMovementRepository movementRepository = new MovementRepository(connectionFactory);

            // Register ViewModels (injecting their dependencies)
            var employeeViewModel = new EmployeeViewModel(employeeRepository, groupRepository);
            var productViewModel = new ProductViewModel(productRepository);
            var sizeViewModel = new SizeViewModel(sizeRepository);
            var inventoryStockViewModel = new InventoryStockViewModel(inventoryStockRepository, productRepository, sizeRepository);
            var deliveryViewModel = new DeliveryViewModel(movementRepository, employeeRepository, productRepository, inventoryStockRepository);
            var movementHistoryViewModel = new MovementHistoryViewModel(movementRepository);


            // 2. Create and show the MainWindow
            MainWindow mainWindow = new MainWindow();
            TabControl mainTabControl = mainWindow.MainTabControl;

            // 3. Set DataContext for each UserControl within MainWindow
            // This assumes UserControls are directly nested as shown in MainWindow.xaml
            // A more robust solution might use DataTemplates, but this is simpler for the test.
            ((Views.EmployeeView)((TabItem)mainTabControl.Items[0]).Content).DataContext = employeeViewModel;
            ((Views.ProductView)((TabItem)mainTabControl.Items[1]).Content).DataContext = productViewModel;
            ((Views.SizeView)((TabItem)mainTabControl.Items[2]).Content).DataContext = sizeViewModel;
            ((Views.InventoryStockView)((TabItem)mainTabControl.Items[3]).Content).DataContext = inventoryStockViewModel;
            ((Views.DeliveryView)((TabItem)mainTabControl.Items[4]).Content).DataContext = deliveryViewModel;
            ((Views.MovementHistoryView)((TabItem)mainTabControl.Items[5]).Content).DataContext = movementHistoryViewModel;


            mainWindow.Show();
        }
    }
}
