using System.Collections.Generic;

namespace BestLife.Models
{
    public class AdminDashboardViewModel
    {
        // List of all registered members
        public List<Members> AllMembers { get; set; } = new();

        // List of all Growfund records
        public List<Growfund> AllGrowfunds { get; set; } = new();

        // List of all member payments
        public List<MemberPayment> AllPayments { get; set; } = new();

        // List of registered users (from Registration table)
        public List<Registration> RegisteredUsers { get; set; } = new();

        // Gender-grouped members
        public List<GenderGroupedMembers> MembersByGender { get; set; } = new();

        // Optionally, member payments grouped/filtered
        public List<MemberPayment> MemberPayments { get; set; } = new();
    }
}
