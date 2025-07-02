using System.ComponentModel.DataAnnotations;

namespace BestLife.Models
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
