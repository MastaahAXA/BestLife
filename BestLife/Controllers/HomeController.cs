using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BestLife.Models;
using BestLife.Data;

namespace BestLife.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BestLifeDbContext _context;

        public HomeController(BestLifeDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Index()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "Account");

            var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
            if (member == null) return RedirectToAction("Login", "Account");

            var referrals = await _context.Members
                .Where(m => m.InvitedByCode == member.ReferralCode)
                .ToListAsync();

            var groupedByGender = referrals
                .GroupBy(m => m.Gender)
                .Select(g => new GenderGroupedMembers
                {
                    Gender = g.Key,
                    Members = g.ToList()
                }).ToList();

            var growfund = await _context.Growfund.FirstOrDefaultAsync(g => g.MemberId == member.id);
            var payments = await _context.MemberPayments
                .Where(p => p.MemberId == member.id)
                .ToListAsync();

            var dashboard = new DashboardViewModel
            {
                MyProfile = member,
                MyGrowfund = growfund ?? new Growfund(),
                MyReferrals = referrals,
                MembersByGender = groupedByGender,
                MemberPayments = payments,
                GenderFilter = ""
            };

            return View("Index", dashboard);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var allMembers = await _context.Members.ToListAsync();
            var allGrowfunds = await _context.Growfund.ToListAsync();
            var allPayments = await _context.MemberPayments.ToListAsync();
            var registeredUsers = await _context.Registration.ToListAsync();

            var groupedByGender = allMembers
                .GroupBy(m => m.Gender)
                .Select(g => new GenderGroupedMembers
                {
                    Gender = g.Key,
                    Members = g.ToList()
                }).ToList();

            var dashboard = new AdminDashboardViewModel
            {
                AllMembers = allMembers,
                AllGrowfunds = allGrowfunds,
                AllPayments = allPayments,
                RegisteredUsers = registeredUsers,
                MembersByGender = groupedByGender,
                MemberPayments = allPayments
            };

            return View("AdminDashboard", dashboard);
        }

        [AllowAnonymous]
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
