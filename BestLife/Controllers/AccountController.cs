using BestLife.Data;
using BestLife.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BestLife.Controllers
{
    public class AccountController : Controller
    {
        private readonly BestLifeDbContext _context;
        private readonly IPasswordHasher<Registration> _passwordHasher;

        public AccountController(BestLifeDbContext context, IPasswordHasher<Registration> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // ✅ GET: /Account/Login
        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        // ✅ POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Registration
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsEmailConfirmed);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or email not confirmed.");
                return View(model);
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Invalid password.");
                return View(model);
            }

            int? memberId = null;
            if (user.Role == "Member")
            {
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == model.Email);
                memberId = member?.id;
            }

            await SignInUser(user.Email, memberId, user.Role);

            return user.Role == "Admin"
                ? RedirectToAction("AdminDashboard", "Home")
                : RedirectToAction("Index", "Home");
        }

        // ✅ GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View(new Registration());

        // ✅ POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(Registration model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _context.Registration.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            model.Role = "Member";
            model.Password = _passwordHasher.HashPassword(model, model.Password);
            model.MyReferralCode = $"REF-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
            model.ConfirmationCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            model.IsEmailConfirmed = true;
            model.DateJoined = DateTime.Now;

            _context.Registration.Add(model);
            await _context.SaveChangesAsync();

            int.TryParse(model.PhoneNumber, out int phoneNo);
            var member = new Members
            {
                Name = model.FullName,
                Email = model.Email,
                PhoneNo = phoneNo,
                IDNO = model.IDNO,
                Gender = model.Gender,
                DOB = model.DOB,
                DateJoined = model.DateJoined,
                ReferralCode = model.MyReferralCode,
                InvitedByCode = model.InvitedBy ?? "None",
                TotalCashback = 0,
                TotalVoucher = 0,
                TotalSavings = 0
            };
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // ✅ POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        // ✅ Session Sign-in Logic
        private async Task SignInUser(string email, int? memberId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
            };

            if (memberId.HasValue)
                claims.Add(new Claim("MemberId", memberId.Value.ToString()));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
                });
        }

        // ✅ GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // ✅ POST: /Account/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required.");
                return View();
            }

            var user = await _context.Registration.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account found with that email.");
                return View();
            }

            string resetCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            user.ConfirmationCode = resetCode;
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Reset code: {resetCode}";
            return RedirectToAction("ResetPassword", new { email = user.Email });
        }

        // ✅ GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        // ✅ POST: /Account/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string confirmationCode, string newPassword)
        {
            var user = await _context.Registration.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.ConfirmationCode != confirmationCode)
            {
                ModelState.AddModelError("", "Invalid confirmation code or email.");
                return View();
            }

            user.Password = _passwordHasher.HashPassword(user, newPassword);
            user.ConfirmationCode = null;
            await _context.SaveChangesAsync();

            TempData["Message"] = "Password reset successful.";
            return RedirectToAction("Login");
        }
    }
}
