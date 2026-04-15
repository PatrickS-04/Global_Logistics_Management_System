namespace PROG7311_PoE.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }
        public int ContractId { get; set; }
        public Contract Contract { get; set; }
        public string description { get; set; }
        public decimal cost { get; set; }
        public string status { get; set; }
    }
}
