using System.Collections.Generic;

namespace BestLife.Models
{
    // Group of members by gender
    public class GenderGroupedMembers
    {
        public string Gender { get; set; } = string.Empty;
        public List<Members> Members { get; set; } = new();
    }

    // Dashboard ViewModel for logged-in Member
    public class DashboardViewModel
    {
        public string Role { get; set; } = "Member"; // Member or Admin (optional use in views)

        // Logged-in user's profile
        public Members MyProfile { get; set; } = new();

        // Growfund for the current member
        public Growfund MyGrowfund { get; set; } = new();

        // Referred members (used current user's referral code)
        public List<Members> MyReferrals { get; set; } = new();

        // Grouped referrals by gender
        public List<GenderGroupedMembers> MembersByGender { get; set; } = new();

        // Selected gender filter (for filtering UI)
        public string? GenderFilter { get; set; }

        // All payment records by this member
        public List<MemberPayment> MemberPayments { get; set; } = new();
    }

}