namespace InventoryWpfApp.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; } // Renamed from 'Group' to 'Name' to avoid keyword conflict
        public int EmployeeTypeId { get; set; }
        public string EmployeeTypeName { get; set; } // For display purposes in UI
    }
}