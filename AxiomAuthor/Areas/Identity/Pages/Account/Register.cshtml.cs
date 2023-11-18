// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using AxiomAuthor.Data;
using DMS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace AxiomAuthor.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "Please enter your first name")]
            [StringLength(255)]
            [DisplayName("First name")]
            public string FirstName { get; set; }

            // Middle name of the author
            [StringLength(200)]
            [DisplayName("Middle Name")]
            public string MiddleName { get; set; }

            // Last name of the author
            [Required(ErrorMessage = "Please enter your last name ")]
            [StringLength(255)]
            [DisplayName("Last name")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Please select your gender")]
            [StringLength(25)]
            public string Gender { get; set; }

            // Date of birth of the author
            [Required(ErrorMessage = "Please enter your date of birth ")]
            [DisplayName("Date Of Birth")]
            [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            // Contact number of the author
            [Required(ErrorMessage = "Please enter the contact")]
            [Phone(ErrorMessage = "Please enter a valid phone number")]
            public string Contact { get; set; }

            // Address of the author
            [StringLength(200)]
            public string Address { get; set; }

            // Country of origin of the author
            [Required(ErrorMessage = "Please enter the country of origin")]
            [StringLength(100)]
            [DisplayName("Country Of Origin")]
            public string CountryOfOrigin { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            public string Username { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(InputModel inputModel, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                await CreateUserIfNotExists(_userManager, inputModel.Username, inputModel.Email, inputModel.Password, "Author");

                await CreateAuthorInformation(inputModel);

                // If everything is successful, you might redirect the user to another page
                return RedirectToPage("/SuccessPage");
            }

            // Log or print the validation errors
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    // Log the error using your preferred logging mechanism
                    // For example, if you're using ILogger:
                    _logger.LogError(error.ErrorMessage);
                    // Alternatively, print it to the console
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private async Task CreateUserIfNotExists(UserManager<IdentityUser> userManager, string username, string email, string password, string role)
        {
            IdentityUser existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                IdentityResult result = await userManager.CreateAsync(new IdentityUser { UserName = username, Email = email, EmailConfirmed = true }, password);
                IdentityUser newUser = await userManager.FindByEmailAsync(email);

                if (!result.Succeeded)
                {
                    throw new ApplicationException("User creation failed."); // Use a more specific exception type if available
                }

                await userManager.AddToRoleAsync(newUser, role);
            }
            else
            {
                throw new ApplicationException("A user with the same email already exists."); // Use a more specific exception type if available
            }
        }

        private async Task CreateAuthorInformation(InputModel inputModel)
        {
            var authorSave = new Author()
            {
                FirstName = inputModel.FirstName,
                MiddleName = inputModel.MiddleName,
                LastName = inputModel.LastName,
                Gender = inputModel.Gender,
                DateOfBirth = inputModel.DateOfBirth,
                Email = inputModel.Email,
                Contact = inputModel.Contact,
                Address = inputModel.Address,
                CountryOfOrigin = inputModel.CountryOfOrigin,
                RegistrationStatus = "Pening"
            };
            await _context.AddAsync(authorSave);
            await _context.SaveChangesAsync();
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}