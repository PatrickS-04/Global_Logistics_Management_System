using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMove.Database.Models
{
    public class ServiceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServiceRequestId { get; set; }
        public int ContractId { get; set; }
        public Contract Contract { get; set; }
        public string description { get; set; }
        public decimal cost { get; set; }
        public string status { get; set; }
    }
}
