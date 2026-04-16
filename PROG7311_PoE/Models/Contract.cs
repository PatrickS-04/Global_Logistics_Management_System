using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG7311_PoE.Models
{
    public enum ContractStatus
    {
        None,
        Draft,
        Active,
        Expired,
        OnHold
    }

    public class Contract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContractId { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public ContractStatus contractStatus { get; set; }
        public string serviceLevel { get; set; }
        public ContractFile? File { get; set; }
        public string defaultCurrency { get; set; } = "ZAR";
    }

        public class StandardContract : Contract { }
        public class PremiumContract : Contract { }
        public class InternationalContract : Contract { }
}
