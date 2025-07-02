using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BestLife.Data;
using BestLife.Models;

namespace BestLife.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly BestLifeDbContext _context;

        public AdminDashboardController(BestLifeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var allMembers = await _context.Members.ToListAsync();
            var allPayments = await _context.MemberPayments.ToListAsync();

            var dashboard = new DashboardViewModel
            {
                MyProfile = new Members(), // Dummy admin profile (optional)
                MyGrowfund = new Growfund(), // Optional
                MyReferrals = allMembers,
                MembersByGender = allMembers
                    .GroupBy(m => m.Gender)
                    .Select(g => new GenderGroupedMembers
                    {
                        Gender = g.Key,
                        Members = g.ToList()
                    }).ToList(),
                MemberPayments = allPayments
            };

            return View("AdminDashboard", dashboard);
        }
    }
}
