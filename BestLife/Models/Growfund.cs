using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestLife.Models
{
    [Table("Growfund")]
    public class Growfund
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to Members
        [Required]
        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Members? Member { get; set; }

        [Required]
        public string BoardLevel { get; set; } = string.Empty; // e.g. "B1", "B2"

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Payment { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cashback { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Voucher { get; set; }

        [Required]
        public string NextLevel { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Savings { get; set; }

        [Required]
        public string InvitedBy { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateJoined { get; set; } = DateTime.Now;

   
    }
}
