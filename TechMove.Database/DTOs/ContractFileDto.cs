using System;
using System.Collections.Generic;
using System.Text;

namespace TechMove.Database.DTOs
{
    public class ContractFileDto
    {
        public int ContractId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public DateTime DateOfUpload { get; set; }
    }
}
