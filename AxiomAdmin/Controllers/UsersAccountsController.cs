using AxiomAdmin.Models;
using AxiomAdmin.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Data;
using MimeKit;

namespace AxiomAdmin.Controllers
{
    [Authorize(Roles = "Administrator,Super-Administration")]
    public class UsersAccountsController : Controller
    {
        private readonly AdminDbContext _context;
        private readonly RoleManager<IdentityRole>? _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public UsersAccountsController(AdminDbContext context, RoleManager<IdentityRole>? roleManager, UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _config = config;
        }

        // GET: UsersAccounts
        public async Task<IActionResult> Index()
        {
            var usersAccounts = await _context.UsersAccounts.ToListAsync();

            return View(usersAccounts);
        }

        private static string GenerateRandomPassword()
        {
            const int passwordLength = 16;
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%#>?&*";
            Random random = new Random();
            char[] password = new char[passwordLength];

            for (int i = 0; i < passwordLength; i++)
            {
                password[i] = validChars[random.Next(0, validChars.Length)];
            }

            return new string(password);
        }

        // GET: UsersAccounts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.UsersAccounts == null)
            {
                return NotFound();
            }

            var usersAccount = await _context.UsersAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersAccount == null)
            {
                return NotFound();
            }

            return View(usersAccount);
        }

        // GET: UsersAccounts/Create
        public IActionResult CreateUsersAccounts()
        {
            var roles = _context.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

            var viewModel = new UsersAccountsViewModle
            {
                Roles = roles,
            };

            return PartialView(viewModel);
        }

        public IActionResult Create()
        {
            return RedirectToAction(nameof(Index));
        }

        // POST: UsersAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(UsersAccount usersAccount)
        {
            bool isDuplicate = await CheckDuplicateFields(usersAccount.Email, usersAccount.MobileNumber);
            bool isFullNameDuplicate = await CheckFullNameDuplicateFields(usersAccount.FirstName, usersAccount.LastName);
            string usernameDuplicate = new string(usersAccount.FullName.Where(c => char.IsLetterOrDigit(c)).ToArray());

            if (isDuplicate)
            {
                // Return an error message if the staff already exists
                return BadRequest(
                    "A staff member with the same email, mobile number already exists.");
            }

            if (isFullNameDuplicate)
            {
                // Return an error message if the staff already exists
                return BadRequest(
                    "Please note that there is already a staff member with the same name in our records.");
            }

            bool isDuplicateUsername = await _userManager.FindByNameAsync(usernameDuplicate) != null;

            if (isDuplicateUsername)
            {
                // Handle the duplicate username scenario, perhaps return an error to the user.
                return BadRequest("The chosen username is already in use. Please choose a different username.");
            }

            if (ModelState.IsValid)
            {
                var staff = usersAccount;
                var Password = GenerateRandomPassword();

                await CreateUserIfNotExists(_userManager, staff.FullName, staff.Email, Password, usersAccount.Roles);
                staff.Username = usernameDuplicate;  // Use the cleaned username

                _context.Add(staff);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));

            //return View(usersAccount);
        }

        private async Task CreateUserIfNotExists(UserManager<IdentityUser> userManager, string fullName, string email, string password, string? roles)
        {
            IdentityUser user = await userManager.FindByEmailAsync(email);
            // Remove spaces and invalid characters from fullname
            string username = new string(fullName.Where(c => char.IsLetterOrDigit(c)).ToArray());
            if (user == null)
            {
                IdentityResult result = await userManager.CreateAsync(new IdentityUser { UserName = username, Email = email, EmailConfirmed = true }, password);
                IdentityUser users = await userManager.FindByEmailAsync(email);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(users, roles);
                }
                else
                {
                    // Password does not meet requirements, generate a new random password
                    string newPassword = GenerateRandomPassword();
                    password = newPassword;
                    await CreateUserIfNotExists(userManager, username, email, newPassword, roles);
                }
                await SendEmailInfo(email, username, fullName, password);
            }
        }

        private Task SendEmailInfo(string email, string username, string fullName, string password)
        {
            string displayName = _config.GetSection("SMTPConfig:DisplayName").Value;
            string senderEmail = _config.GetSection("SMTPConfig:UserName").Value;
            string senderPassword = _config.GetSection("SMTPConfig:Password").Value;
            string host = _config.GetSection("SMTPConfig:Host").Value;
            int port = 587;
            string recipientEmail = email; // Replace with the recipient's email address

            // Setup the message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(displayName, senderEmail));
            message.To.Add(new MailboxAddress("Recipient Name", recipientEmail));
            message.Subject = " Axiom Indexing, Account Details!";

            // Body of the email
            message.Body = new TextPart("plain")
            {
                Text = "Welcome aboard! Your registration is complete. Account details:" + " Full name " + fullName + " " + "Username " + username + " " + "Email " + email + ", " + "Please keep your password confidential " + password + ". " + "Thank You."
            };

            // Setup the SMTP client
            using var client = new SmtpClient();

            try
            {
                // Connect to the SMTP server with the correct hostname
                client.Connect(host, port, SecureSocketOptions.StartTls);

                // Authenticate with the server
                client.Authenticate(senderEmail, senderPassword);

                // Send the email
                client.Send(message);

                // Disconnect from the server
                client.Disconnect(true);

                // Returning a completed Task as the method is asynchronous
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Returning a faulted Task in case of an exception
                return Task.FromException(ex);
            }
        }

        private async Task<bool> CheckFullNameDuplicateFields(string firstName, string lastName)
        {
            return await _context.UsersAccounts.AnyAsync(x => x.FirstName == firstName && x.LastName == lastName);
        }

        private async Task<bool> CheckDuplicateFields(string email, string mobileNumber)
        {
            return await _context.UsersAccounts.AnyAsync(x => x.Email == email || x.MobileNumber == mobileNumber);
        }

        // GET: UsersAccounts/Edit/5
        public async Task<IActionResult> EditUsersAccounts(Guid? id)
        {
            if (id == null || _context.UsersAccounts == null)
            {
                return NotFound();
            }
            var roles = _context.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

            var usersAccount = await _context.UsersAccounts.FindAsync(id);
            var viewModel = new UsersAccountsViewModle
            {
                GetUsersAccount = usersAccount,
                Roles = roles
            };
            if (usersAccount == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: UsersAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UsersAccount usersAccount)
        {
            if (id != usersAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usersAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersAccountExists(usersAccount.Id))
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
            return View(usersAccount);
        }

        // GET: UsersAccounts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.UsersAccounts == null)
            {
                return NotFound();
            }

            var usersAccount = await _context.UsersAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersAccount == null)
            {
                return NotFound();
            }

            return View(usersAccount);
        }

        // POST: UsersAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.UsersAccounts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UsersAccounts'  is null.");
            }
            var usersAccount = await _context.UsersAccounts.FindAsync(id);
            if (usersAccount != null)
            {
                _context.UsersAccounts.Remove(usersAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersAccountExists(Guid id)
        {
            return (_context.UsersAccounts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}