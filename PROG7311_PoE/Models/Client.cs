using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG7311_PoE.Models
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }
        public string UserId { get; set; }
        public string clientName { get; set; }
        public string clientEmail { get; set; }
        public string clientPhoneNumber { get; set; }
    }
}
