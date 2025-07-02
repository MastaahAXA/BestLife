using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestLife.Models
{
    [Table("Registration")]
    public class Registration
    {
        [Key]
        public int Id { get; set; }

        // Full Name
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        // Email Address
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(100)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        // Password
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        // Confirm Password - Not stored in the DB
        [NotMapped]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Phone Number
        [Phone(ErrorMessage = "Invalid phone number.")]
        [MaxLength(15)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        // Gender
        [Required(ErrorMessage = "Gender is required.")]
        [MaxLength(10)]
        public string Gender { get; set; } = string.Empty;

        // National ID
        [Required(ErrorMessage = "National ID is required.")]
        [MaxLength(20)]
        [Display(Name = "National ID")]
        public string IDNO { get; set; } = string.Empty;

        // Date of Birth
        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        // Date Joined
        [Required]
        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; } = DateTime.Now;

        // Email Confirmation Code
        [MaxLength(10)]
        [Display(Name = "Confirmation Code")]
        public string? ConfirmationCode { get; set; }

        // Email Confirmation Status
        [Display(Name = "Email Confirmed")]
        public bool IsEmailConfirmed { get; set; } = false;

        // Referral Code entered during registration (optional)
        [MaxLength(20)]
        [Display(Name = "Referral Code Used")]
        public string? ReferralCode { get; set; }

        // The code of the user who invited this one
        [MaxLength(20)]
        [Display(Name = "Invited By")]
        public string? InvitedBy { get; set; }

        // This user's own referral code to invite others
        [MaxLength(20)]
        [Display(Name = "My Referral Code")]
        public string? MyReferralCode { get; set; }

        // Role (Admin or Member)
        [Required(ErrorMessage = "User role is required.")]
        [MaxLength(20)]
        [Display(Name = "User Role")]
        public string Role { get; set; } = "Member"; // Default is "Member"
    }
}
