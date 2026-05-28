using System;
using System.Collections.Generic;
using System.Text;

namespace TechMove.Database.DTOs
{
    public class ContractDto
    {
        public int ContractId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ContractStatus { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public string DefaultCurrency {  get; set; } = "ZAR";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
