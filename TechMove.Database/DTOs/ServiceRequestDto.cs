using System;
using System.Collections.Generic;
using System.Text;

namespace TechMove.Database.DTOs
{
    public class ServiceRequestDto
    {
        public int ServiceRequestId { get; set; }
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Currency { get; set; } = "ZAR";
    }
}
