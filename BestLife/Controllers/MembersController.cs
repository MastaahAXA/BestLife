using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BestLife.Data;
using BestLife.Models;

namespace BestLife.Controllers
{
    [Route("Members")]
    public class MembersController : Controller
    {
        private readonly BestLifeDbContext _context;

        public MembersController(BestLifeDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Members.ToListAsync());
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var members = await _context.Members.FirstOrDefaultAsync(m => m.id == id);
            if (members == null) return NotFound();
            return View(members);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Name,IDNO,PhoneNo,DOB,Gender,DateJoined,Email,ReferralCode,InvitedByCode,TotalCashback,TotalVoucher,TotalSavings")] Members members)
        {
            if (ModelState.IsValid)
            {
                _context.Add(members);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(members);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var members = await _context.Members.FindAsync(id);
            if (members == null) return NotFound();
            return View(members);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Name,IDNO,PhoneNo,DOB,Gender,DateJoined,Email,ReferralCode,InvitedByCode,TotalCashback,TotalVoucher,TotalSavings")] Members members)
        {
            if (id != members.id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(members);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembersExists(members.id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(members);
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var members = await _context.Members.FirstOrDefaultAsync(m => m.id == id);
            if (members == null) return NotFound();
            return View(members);
        }

        [HttpPost("DeleteConfirmed/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var members = await _context.Members.FindAsync(id);
            if (members != null)
            {
                _context.Members.Remove(members);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MembersExists(int id)
        {
            return _context.Members.Any(e => e.id == id);
        }

        [HttpGet("GenderPaymentList")]
        public async Task<IActionResult> GenderPaymentList(string? gender, bool? hasPayment)
        {
            var query = _context.Members.AsQueryable();

            if (!string.IsNullOrEmpty(gender))
                query = query.Where(m => m.Gender == gender);

            if (hasPayment != null)
            {
                var paidIds = await _context.MemberPayments
                    .Select(p => p.MemberId)
                    .Distinct()
                    .ToListAsync();

                query = hasPayment.Value
                    ? query.Where(m => paidIds.Contains(m.id))
                    : query.Where(m => !paidIds.Contains(m.id));
            }

            var model = new GenderPaymentFilterViewModel
            {
                SelectedGender = gender,
                HasPayment = hasPayment,
                FilteredMembers = await query.ToListAsync()
            };

            return View(model);
        }

        [HttpGet("EarningsPerLevel/{memberId}")]
        public async Task<IActionResult> EarningsPerLevel(int? memberId)
        {
            if (memberId == null) return NotFound();
            var member = await _context.Members.FirstOrDefaultAsync(m => m.id == memberId);
            if (member == null) return NotFound();

            var payments = await _context.MemberPayments
                .Where(p => p.MemberId == memberId)
                .ToListAsync();

            var growfunds = await _context.Growfund
                .Where(g => g.MemberId == memberId)
                .ToListAsync();

            var rawEarnings = (payments.Any()
                ? payments.Select(p => new
                {
                    p.BoardLevel,
                    p.Cashback,
                    p.Voucher,
                    p.Savings,
                    Date = p.PaymentDate
                })
                : growfunds.Select(g => new
                {
                    g.BoardLevel,
                    g.Cashback,
                    g.Voucher,
                    g.Savings,
                    Date = g.DateJoined
                }))
                .GroupBy(x => x.BoardLevel)
                .ToDictionary(
                    g => g.Key,
                    g => new MemberEarnings
                    {
                        BoardLevel = g.Key,
                        Cashback = g.Sum(x => x.Cashback),
                        Voucher = g.Sum(x => x.Voucher),
                        Savings = g.Sum(x => x.Savings),
                        EarnedOn = g.Max(x => x.Date),
                        MemberId = memberId.Value,
                        Member = member
                    });

            var allBoardLevels = Enumerable.Range(1, 10).Select(i => "B" + i).ToList();

            var fullEarnings = allBoardLevels.Select(level =>
                rawEarnings.ContainsKey(level)
                    ? rawEarnings[level]
                    : new MemberEarnings
                    {
                        BoardLevel = level,
                        Cashback = 0m,
                        Voucher = 0m,
                        Savings = 0m,
                        EarnedOn = DateTime.MinValue,
                        MemberId = memberId.Value,
                        Member = member
                    }).ToList();

            var viewModel = new MemberEarningsViewModel
            {
                Member = member,
                EarningsPerLevel = fullEarnings
            };

            return View(viewModel);
        }

        [HttpGet("AddPayment/{memberId}")]
        public async Task<IActionResult> AddPayment(int memberId)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member == null) return NotFound();

            var payment = new MemberPayment
            {
                MemberId = memberId,
                PaymentDate = DateTime.Now,
                PaymentMethod = "Mpesa"
            };

            ViewBag.MemberName = member.Name;
            return View(payment);
        }

        [HttpPost("AddPayment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPayment(MemberPayment payment)
        {
            if (!ModelState.IsValid)
            {
                var member = await _context.Members.FindAsync(payment.MemberId);
                ViewBag.MemberName = member?.Name;
                return View(payment);
            }

            payment.PaymentDate = DateTime.Now;
            _context.MemberPayments.Add(payment);
            await _context.SaveChangesAsync();

            var memberRef = await _context.Members.FirstOrDefaultAsync(m => m.id == payment.MemberId);
            if (memberRef != null && !string.IsNullOrEmpty(memberRef.ReferralCode))
            {
                var referrals = await _context.Members
                    .Where(m => m.InvitedByCode == memberRef.ReferralCode)
                    .Select(m => m.id)
                    .ToListAsync();

                var referralPayments = await _context.MemberPayments
                    .Where(p => referrals.Contains(p.MemberId))
                    .Select(p => p.MemberId)
                    .Distinct()
                    .CountAsync();

                if (referralPayments >= 6 && !string.IsNullOrEmpty(payment.BoardLevel))
                {
                    int currentLevel = int.TryParse(payment.BoardLevel.Substring(1), out var lvl) ? lvl : 1;
                    if (currentLevel < 10)
                    {
                        var upgrade = new MemberPayment
                        {
                            MemberId = payment.MemberId,
                            BoardLevel = "B" + (currentLevel + 1),
                            AmountPaid = 0,
                            Cashback = 5000,
                            Voucher = 0,
                            Savings = 0,
                            PaymentMethod = "System",
                            PaymentDate = DateTime.Now
                        };

                        _context.MemberPayments.Add(upgrade);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction(nameof(MemberPayments), new { memberId = payment.MemberId });
        }

        [HttpGet("MemberPayments/{memberId}")]
        public async Task<IActionResult> MemberPayments(int memberId)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.id == memberId);
            if (member == null) return NotFound();

            var payments = await _context.MemberPayments
                .Where(p => p.MemberId == memberId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            ViewBag.MemberName = member.Name;
            ViewBag.MemberId = memberId;

            return View("MemberPayments", payments);
        }
    }
}
