using System;
using System.ComponentModel.DataAnnotations;

namespace BestLife.Models
{
    public class Members
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID Number is required")]
        public string IDNO { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        public int PhoneNo { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateJoined { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        // ✅ Unique referral code for this member
        [Required]
        public string ReferralCode { get; set; } = string.Empty;

        // ✅ Code of the member who referred this one (if any)
        public string InvitedByCode { get; set; } = "None";
        public int TotalCashback { get; set; } = 0;
        public int TotalVoucher { get; set; } = 0;
        public int TotalSavings { get; set; } = 0;

    }
}
