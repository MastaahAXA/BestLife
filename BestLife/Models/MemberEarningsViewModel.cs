using System.ComponentModel.DataAnnotations;
namespace BestLife.Models
    

{
    public class MemberEarningsViewModel
    {

        [Key]
        public int Id { get; set; }
        public Members Member { get; set; } = null!;
        public List<MemberEarnings> EarningsPerLevel { get; set; } = new();
    }
}
