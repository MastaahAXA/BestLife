using System.Collections.Generic;

namespace BestLife.Models
{
    public class GenderPaymentFilterViewModel
    {
        public string? SelectedGender { get; set; }
        public bool? HasPayment { get; set; }
        public List<Members> FilteredMembers { get; set; } = new();
    }
}
