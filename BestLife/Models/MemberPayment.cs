using System;

namespace BestLife.Models
{
    public class MemberPayment
    {
        public int Id { get; set; }

        // Foreign key reference to Members table
        public int MemberId { get; set; }

        // Navigation property for related member
        public Members Member { get; set; } = null!;

        // Board level associated with the payment (e.g., B1, B2, etc.)
        public string BoardLevel { get; set; } = string.Empty;

        // Amount paid by the member
        public decimal AmountPaid { get; set; }

        // Date when the payment was made
        public DateTime PaymentDate { get; set; }

        // ✅ New: Payment method (Mpesa or Cash)
        public string PaymentMethod { get; set; } = "Mpesa";

        // Optional link to related Growfund entry for additional earnings
        public Growfund? Growfund { get; set; }

        // Optional earnings breakdown associated with this payment
        public decimal Cashback { get; set; }
        public decimal Voucher { get; set; }
        public decimal Savings { get; set; }
    }
}
