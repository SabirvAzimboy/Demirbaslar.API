using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Demirbaslar.API.Models
{
    public class Repair : Base
    {        
        public int AssetId { get; set; }
        public Assets Asset { get; set; }
        public int RepairPersonId { get; set; }
        public RepairPersons RepairPerson { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool Repaired { get; set; } = true;

        [Range(0, 9999999999999999.99, ErrorMessage = "Стоимость ремонта не может быть отрицательной")]
        [Precision(18, 2)]
        public decimal? RepairCost { get; set; }
        public string? Notes { get; set; }
    }
}
