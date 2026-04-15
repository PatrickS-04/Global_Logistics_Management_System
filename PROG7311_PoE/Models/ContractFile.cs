using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG7311_PoE.Models
{
    [Table("ContractFiles", Schema = "dbo")]
    public class ContractFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContractFileId { get; set; }
        public string fileName { get; set; }
        public string storedFileName { get; set; }
        public long fileSize { get; set; }
        public string filePath { get; set; }
        public int ContractId { get; set; }
    }
}
