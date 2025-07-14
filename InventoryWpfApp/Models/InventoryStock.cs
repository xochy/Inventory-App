namespace InventoryWpfApp.Models
{
    public class InventoryStock
    {
        public int InventoryStockId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } // For display
        public int SizeId { get; set; }
        public string SizeValue { get; set; } // For display
        public int CurrentQuantity { get; set; }
        public int MinStockLimit { get; set; }
    }
}
