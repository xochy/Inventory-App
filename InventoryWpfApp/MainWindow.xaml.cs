using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using System.Windows.Media;

namespace InventoryWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=InventoryTechnicalTestDB;Integrated Security=True;TrustServerCertificate=True;";

        public MainWindow()
        {
            InitializeComponent();
        }

        // Evento que se dispara cuando la ventana ha terminado de cargarse
        // --- Métodos de Carga Inicial ---
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGroupsIntoComboBox(cmbGroup); // Para el ComboBox de Empleados
            LoadApplicabilityTypesIntoComboBox(); // Para el ComboBox de Productos
            LoadNotationTypesIntoComboBox(); // Para el ComboBox de Tallas
            LoadProductsIntoComboBox(cmbStockProduct); // Para ComboBox de stock inicial
            LoadSizesIntoComboBox(cmbStockSize); // Para ComboBox de stock inicial

            LoadEmployeesIntoComboBox(cmbDeliveryEmployee); // Para entregas
            LoadProductsIntoComboBox(cmbDeliveryProduct); // Para entregas

            RefreshMovementsHistory(); // Carga inicial del historial
        }

        // Método genérico para cargar grupos en un ComboBox
        private void LoadGroupsIntoComboBox(ComboBox comboBox)
        {
            comboBox.DisplayMemberPath = "[Group]";
            comboBox.SelectedValuePath = "GroupId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT GroupId, [Group] FROM Groups ORDER BY [Group]";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    adapter.Fill(dt);
                    comboBox.ItemsSource = dt.DefaultView;
                }
                catch (SqlException ex)
                {
                    ShowMessage(tblEmployeeMessage, "Error al cargar grupos: " + ex.Message, Brushes.Red);
                    MessageBox.Show("Error de Base de Datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Método para cargar tipos de aplicabilidad en el ComboBox de productos
        private void LoadApplicabilityTypesIntoComboBox()
        {
            // Ya están definidos como ComboBoxItem en XAML, solo aseguramos la selección por defecto
            cmbApplicabilityType.SelectedIndex = 0;
        }

        // Método para cargar tipos de notación en el ComboBox de tallas
        private void LoadNotationTypesIntoComboBox()
        {
            // Ya están definidos como ComboBoxItem en XAML, solo aseguramos la selección por defecto
            cmbNotationType.SelectedIndex = 0;
        }

        // Método genérico para cargar productos en un ComboBox
        private void LoadProductsIntoComboBox(ComboBox comboBox)
        {
            comboBox.DisplayMemberPath = "Name";
            comboBox.SelectedValuePath = "ProductId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductId, Name FROM Products ORDER BY Name";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    adapter.Fill(dt);
                    comboBox.ItemsSource = dt.DefaultView;
                }
                catch (SqlException ex)
                {
                    // Usa el TextBlock de mensaje relevante según el contexto
                    ShowMessage(tblProductMessage, "Error al cargar productos: " + ex.Message, Brushes.Red);
                    MessageBox.Show("Error de Base de Datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void cmbStockProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbStockProduct.SelectedValue != null)
            {
                int productId = (int)cmbStockProduct.SelectedValue;
                LoadSizesForProduct(productId, cmbStockSize); // Reutiliza el método de carga de tallas por producto
            }
            else
            {
                cmbStockSize.ItemsSource = null; // Limpia las tallas si no hay producto seleccionado
                cmbStockSize.SelectedIndex = -1;
            }
        }

        // Método genérico para cargar tallas en un ComboBox
        private void LoadSizesIntoComboBox(ComboBox comboBox)
        {
            comboBox.DisplayMemberPath = "SizeValue";
            comboBox.SelectedValuePath = "SizeId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT SizeId, SizeValue FROM Sizes ORDER BY SizeValue";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    adapter.Fill(dt);
                    comboBox.ItemsSource = dt.DefaultView;
                }
                catch (SqlException ex)
                {
                    // Usa el TextBlock de mensaje relevante según el contexto
                    ShowMessage(tblSizeMessage, "Error al cargar tallas: " + ex.Message, Brushes.Red);
                    MessageBox.Show("Error de Base de Datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Método para cargar empleados en un ComboBox
        private void LoadEmployeesIntoComboBox(ComboBox comboBox)
        {
            comboBox.DisplayMemberPath = "Name";
            comboBox.SelectedValuePath = "EmployeeId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT EmployeeId, Name FROM Employees ORDER BY Name";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    adapter.Fill(dt);
                    comboBox.ItemsSource = dt.DefaultView;
                }
                catch (SqlException ex)
                {
                    ShowMessage(tblDeliveryMessage, "Error al cargar empleados: " + ex.Message, Brushes.Red);
                    MessageBox.Show("Error de Base de Datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // --- Métodos de Gestión de Catálogos ---

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            string employeeName = txtEmployeeName.Text.Trim();
            if (string.IsNullOrEmpty(employeeName))
            {
                ShowMessage(tblEmployeeMessage, "Por favor, ingresa el nombre del empleado.", Brushes.Red);
                return;
            }
            if (cmbGroup.SelectedValue == null)
            {
                ShowMessage(tblEmployeeMessage, "Por favor, selecciona un grupo.", Brushes.Red);
                return;
            }
            int groupId = (int)cmbGroup.SelectedValue;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Employees (Name, GroupId) VALUES (@Name, @GroupId)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", employeeName);
                    command.Parameters.AddWithValue("@GroupId", groupId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage(tblEmployeeMessage, $"Empleado '{employeeName}' agregado exitosamente.", Brushes.Green);
                        txtEmployeeName.Clear();
                        cmbGroup.SelectedIndex = -1;
                        LoadEmployeesIntoComboBox(cmbDeliveryEmployee); // Actualiza la lista de empleados para la entrega
                    }
                    else
                    {
                        ShowMessage(tblEmployeeMessage, "No se pudo agregar el empleado.", Brushes.Red);
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowMessage(tblEmployeeMessage, "Error: " + ex.Message, Brushes.Red);
                MessageBox.Show("Error de Base de Datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ShowMessage(tblEmployeeMessage, "Ocurrió un error inesperado: " + ex.Message, Brushes.Red);
                MessageBox.Show("Error General: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            string productName = txtProductName.Text.Trim();
            string productDescription = txtProductDescription.Text.Trim();
            ComboBoxItem selectedApplicability = cmbApplicabilityType.SelectedItem as ComboBoxItem;

            if (string.IsNullOrEmpty(productName))
            {
                ShowMessage(tblProductMessage, "Por favor, ingresa el nombre del producto.", Brushes.Red);
                return;
            }
            if (selectedApplicability == null)
            {
                ShowMessage(tblProductMessage, "Por favor, selecciona el tipo de aplicabilidad.", Brushes.Red);
                return;
            }
            string applicabilityType = selectedApplicability.Content.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Products (Name, Description, ApplicabilityType) VALUES (@Name, @Description, @ApplicabilityType)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", productName);
                    command.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(productDescription) ? (object)DBNull.Value : productDescription);
                    command.Parameters.AddWithValue("@ApplicabilityType", applicabilityType);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage(tblProductMessage, $"Producto '{productName}' agregado exitosamente.", Brushes.Green);
                        txtProductName.Clear();
                        txtProductDescription.Clear();
                        cmbApplicabilityType.SelectedIndex = 0;
                        LoadProductsIntoComboBox(cmbStockProduct); // Actualiza la lista de productos para stock
                        LoadProductsIntoComboBox(cmbDeliveryProduct); // Actualiza la lista de productos para entrega
                    }
                    else
                    {
                        ShowMessage(tblProductMessage, "No se pudo agregar el producto.", Brushes.Red);
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowMessage(tblProductMessage, "Error: " + ex.Message, Brushes.Red);
                MessageBox.Show("Error de Base de Datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ShowMessage(tblProductMessage, "Ocurrió un error inesperado: " + ex.Message, Brushes.Red);
                MessageBox.Show("Error General: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddSize_Click(object sender, RoutedEventArgs e)
        {
            string sizeValue = txtSizeValue.Text.Trim();
            ComboBoxItem selectedNotation = cmbNotationType.SelectedItem as ComboBoxItem;

            if (string.IsNullOrEmpty(sizeValue))
            {
                ShowMessage(tblSizeMessage, "Por favor, ingresa el valor de la talla.", Brushes.Red);
                return;
            }
            if (selectedNotation == null)
            {
                ShowMessage(tblSizeMessage, "Por favor, selecciona el tipo de notación.", Brushes.Red);
                return;
            }
            string notationType = selectedNotation.Content.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Sizes (SizeValue, NotationType) VALUES (@SizeValue, @NotationType)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SizeValue", sizeValue);
                    command.Parameters.AddWithValue("@NotationType", notationType);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage(tblSizeMessage, $"Talla '{sizeValue}' agregada exitosamente.", Brushes.Green);
                        txtSizeValue.Clear();
                        cmbNotationType.SelectedIndex = 0;
                        LoadSizesIntoComboBox(cmbStockSize); // Actualiza la lista de tallas para stock
                        // No es necesario actualizar cmbDeliverySize aquí porque se actualiza dinámicamente
                    }
                    else
                    {
                        ShowMessage(tblSizeMessage, "No se pudo agregar la talla. Podría ser que ya existe.", Brushes.Red);
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowMessage(tblSizeMessage, "Error: " + ex.Message, Brushes.Red);
                MessageBox.Show("Error de Base de Datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ShowMessage(tblSizeMessage, "Ocurrió un error inesperado: " + ex.Message, Brushes.Red);
                MessageBox.Show("Error General: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddInitialStock_Click(object sender, RoutedEventArgs e)
        {
            if (cmbStockProduct.SelectedValue == null)
            {
                ShowMessage(tblStockMessage, "Por favor, selecciona un producto.", Brushes.Red);
                return;
            }
            if (cmbStockSize.SelectedValue == null)
            {
                ShowMessage(tblStockMessage, "Por favor, selecciona una talla.", Brushes.Red);
                return;
            }
            if (!int.TryParse(txtInitialQuantity.Text, out int initialQuantity) || initialQuantity <= 0)
            {
                ShowMessage(tblStockMessage, "Cantidad inicial inválida. Debe ser un número positivo.", Brushes.Red);
                return;
            }
            if (!int.TryParse(txtMinStock.Text, out int minStockLimit) || minStockLimit < 0)
            {
                ShowMessage(tblStockMessage, "Stock mínimo inválido. Debe ser un número no negativo.", Brushes.Red);
                return;
            }

            int productId = (int)cmbStockProduct.SelectedValue;
            int sizeId = (int)cmbStockSize.SelectedValue;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        IF NOT EXISTS (SELECT 1 FROM InventoryStock WHERE ProductId = @ProductId AND SizeId = @SizeId)
                        BEGIN
                            INSERT INTO InventoryStock (ProductId, SizeId, CurrentQuantity, MinStockLimit)
                            VALUES (@ProductId, @SizeId, @InitialQuantity, @MinStockLimit);
                        END
                        ELSE
                        BEGIN
                            UPDATE InventoryStock
                            SET CurrentQuantity = CurrentQuantity + @InitialQuantity,
                                MinStockLimit = @MinStockLimit -- Permite actualizar el stock mínimo
                            WHERE ProductId = @ProductId AND SizeId = @SizeId;
                        END";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@SizeId", sizeId);
                    command.Parameters.AddWithValue("@InitialQuantity", initialQuantity);
                    command.Parameters.AddWithValue("@MinStockLimit", minStockLimit);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ShowMessage(tblStockMessage, "Stock actualizado/agregado exitosamente.", Brushes.Green);
                        txtInitialQuantity.Clear();
                        txtMinStock.Clear();
                        cmbStockProduct.SelectedIndex = -1;
                        cmbStockSize.SelectedIndex = -1;
                    }
                    else
                    {
                        ShowMessage(tblStockMessage, "No se pudo actualizar/agregar el stock.", Brushes.Red);
                    }
                }
            }
            catch (SqlException ex)
            {
                ShowMessage(tblStockMessage, "Error: " + ex.Message, Brushes.Red);
                MessageBox.Show("Error de Base de Datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ShowMessage(tblStockMessage, "Ocurrió un error inesperado: " + ex.Message, Brushes.Red);
                MessageBox.Show("Error General: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Métodos de Entregas de Inventario ---

        // Evento que se dispara cuando se selecciona un producto para entrega
        private void cmbDeliveryProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDeliveryProduct.SelectedValue != null)
            {
                int productId = (int)cmbDeliveryProduct.SelectedValue;
                LoadSizesForProduct(productId, cmbDeliverySize);
            }
            else
            {
                cmbDeliverySize.ItemsSource = null; // Limpiar tallas si no hay producto seleccionado
            }
        }

        // Carga las tallas disponibles en stock para un producto seleccionado
        private void LoadSizesForProduct(int productId, ComboBox comboBox)
        {
            comboBox.DisplayMemberPath = "SizeValue";
            comboBox.SelectedValuePath = "InventoryStockId"; // Aquí queremos el InventoryStockId para el SP

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT
                        InvS.InventoryStockId,  -- <--- CAMBIO AQUÍ: Usar InvS
                        S.SizeValue
                    FROM
                        InventoryStock InvS     -- <--- CAMBIO AQUÍ: Usar InvS como alias
                    JOIN
                        Sizes S ON InvS.SizeId = S.SizeId
                    WHERE
                        InvS.ProductId = @ProductId AND InvS.CurrentQuantity > 0 -- <--- CAMBIO AQUÍ: Usar InvS
                    ORDER BY
                        S.SizeValue;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    adapter.Fill(dt);
                    comboBox.ItemsSource = dt.DefaultView;
                    comboBox.SelectedIndex = -1; // Limpiar selección previa
                }
                catch (SqlException ex)
                {
                    ShowMessage(tblDeliveryMessage, "Error al cargar tallas para el producto: " + ex.Message, Brushes.Red);
                    MessageBox.Show("Error de Base de Datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void cmbDeliveryEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Opcional: Podrías cargar aquí una lista de productos aplicables a este empleado
            // o mostrar un mensaje si el empleado es de un tipo específico.
            // Por simplicidad, no lo implementaremos en este ejemplo,
            // ya que el SP ya valida la aplicabilidad.
        }

        private void btnRegisterDelivery_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDeliveryEmployee.SelectedValue == null)
            {
                ShowMessage(tblDeliveryMessage, "Por favor, selecciona un empleado.", Brushes.Red);
                return;
            }
            if (cmbDeliverySize.SelectedValue == null) // cmbDeliverySize.SelectedValue contiene InventoryStockId
            {
                ShowMessage(tblDeliveryMessage, "Por favor, selecciona una talla del producto.", Brushes.Red);
                return;
            }
            if (!int.TryParse(txtDeliveryQuantity.Text, out int quantity) || quantity <= 0)
            {
                ShowMessage(tblDeliveryMessage, "Cantidad a entregar inválida. Debe ser un número positivo.", Brushes.Red);
                return;
            }

            int employeeId = (int)cmbDeliveryEmployee.SelectedValue;
            int inventoryStockId = (int)cmbDeliverySize.SelectedValue; // InventoryStockId es el valor de la talla seleccionada

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("sp_InventoryRegisterDelivery", connection);
                    command.CommandType = CommandType.StoredProcedure; // Indica que es un stored procedure

                    command.Parameters.AddWithValue("@InventoryStockId", inventoryStockId);
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@Quantity", quantity);

                    connection.Open();
                    command.ExecuteNonQuery(); // El SP maneja las excepciones internas y mensajes

                    ShowMessage(tblDeliveryMessage, "Entrega registrada exitosamente.", Brushes.Green);
                    txtDeliveryQuantity.Clear();
                    cmbDeliveryEmployee.SelectedIndex = -1;
                    cmbDeliveryProduct.SelectedIndex = -1;
                    cmbDeliverySize.ItemsSource = null; // Limpiar tallas

                    RefreshMovementsHistory(); // Actualiza el historial después de la entrega
                }
            }
            catch (SqlException ex)
            {
                // El SP puede lanzar RAISERROR, captúralo aquí
                ShowMessage(tblDeliveryMessage, "Error al registrar entrega: " + ex.Message, Brushes.Red);
                MessageBox.Show("Error en la Entrega: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ShowMessage(tblDeliveryMessage, "Ocurrió un error inesperado al registrar entrega: " + ex.Message, Brushes.Red);
                MessageBox.Show("Error General: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Métodos de Historial de Movimientos ---

        private void btnRefreshMovements_Click(object sender, RoutedEventArgs e)
        {
            RefreshMovementsHistory();
        }

        private void RefreshMovementsHistory()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM vw_InventoryWerehouseMovements ORDER BY MovementDate DESC";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    adapter.Fill(dt);
                    dgMovements.ItemsSource = dt.DefaultView; // Asigna los datos al DataGrid
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error al cargar historial de movimientos: " + ex.Message, "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // --- Método Auxiliar para Mensajes ---
        private void ShowMessage(TextBlock targetTextBlock, string message, SolidColorBrush color)
        {
            targetTextBlock.Text = message;
            targetTextBlock.Foreground = color;
        }
    }
}