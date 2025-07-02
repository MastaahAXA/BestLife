using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BestLife.Data;
using BestLife.Models;

namespace BestLife.Controllers
{
    public class GrowfundsController : Controller
    {
        private readonly BestLifeDbContext _context;

        public GrowfundsController(BestLifeDbContext context)
        {
            _context = context;
        }

        // GET: Growfunds
        public async Task<IActionResult> Index()
        {
            var bestLifeDbContext = _context.Growfund.Include(g => g.Member);
            return View(await bestLifeDbContext.ToListAsync());
        }

        // GET: Growfunds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var growfund = await _context.Growfund
                .Include(g => g.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (growfund == null)
            {
                return NotFound();
            }

            return View(growfund);
        }

        // GET: Growfunds/Create
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Members, "id", "Email");
            return View();
        }

        // POST: Growfunds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MemberId,BoardLevel,Payment,Cashback,Voucher,NextLevel,Savings,InvitedBy,DateJoined")] Growfund growfund)
        {
            if (ModelState.IsValid)
            {
                _context.Add(growfund);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "id", "Email", growfund.MemberId);
            return View(growfund);
        }

        // GET: Growfunds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var growfund = await _context.Growfund.FindAsync(id);
            if (growfund == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "id", "Email", growfund.MemberId);
            return View(growfund);
        }

        // POST: Growfunds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberId,BoardLevel,Payment,Cashback,Voucher,NextLevel,Savings,InvitedBy,DateJoined")] Growfund growfund)
        {
            if (id != growfund.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(growfund);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GrowfundExists(growfund.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Members, "id", "Email", growfund.MemberId);
            return View(growfund);
        }

        // GET: Growfunds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var growfund = await _context.Growfund
                .Include(g => g.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (growfund == null)
            {
                return NotFound();
            }

            return View(growfund);
        }

        // POST: Growfunds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var growfund = await _context.Growfund.FindAsync(id);
            if (growfund != null)
            {
                _context.Growfund.Remove(growfund);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GrowfundExists(int id)
        {
            return _context.Growfund.Any(e => e.Id == id);
        }
    }
}
