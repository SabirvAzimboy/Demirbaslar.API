namespace Demirbaslar.API.Models
{
    public class Employee : Base
    {        
        public string PersonnelNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

}

