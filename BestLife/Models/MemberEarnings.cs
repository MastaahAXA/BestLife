using System.ComponentModel.DataAnnotations;

namespace BestLife.Models
{
    public class MemberEarnings
    {
        [Key]
        public int Id { get; set; }

        public int MemberId { get; set; }
        public Members Member { get; set; } = null!;

        public string BoardLevel { get; set; } = string.Empty;

        public decimal Cashback { get; set; }
        public decimal Voucher { get; set; }
        public decimal Savings { get; set; }

        public DateTime EarnedOn { get; set; }
    }
}
