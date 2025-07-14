namespace InventoryWpfApp.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; } // For display purposes in UI
        public string EmployeeTypeName { get; set; } // For display purposes in UI
    }
}
