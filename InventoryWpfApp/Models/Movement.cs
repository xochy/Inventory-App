namespace InventoryWpfApp.Models
{
    public class Movement
    {
        public int MovementId { get; set; }
        public DateTime MovementDate { get; set; }
        public string MovementType { get; set; }
        public int QuantityMoved { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductSize { get; set; }
        public string SizeNotation { get; set; }
        public int RemainingStock { get; set; }
        public int MinStockLimit { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeGroup { get; set; } 
        public string EmployeeType { get; set; }
        public string Notes { get; set; }
    }
}