namespace Demirbaslar.API.Models
{
    public class Assets : Base
    {        
        public string? InventoryNumber { get; set; } 
        public string Nomenclature { get; set; } = string.Empty;
        public string Characteristics { get; set; } = string.Empty;
        public string? SerialNumber { get; set; }
        public int? CurrentHolderId { get; set; }
        public Employee? CurrentHolder { get; set; }
        public DateTime? CommissioningDate { get; set; }
        public byte Location { get; set; } = 0;
        public bool Perfect { get; set; } = true;
    }
}
