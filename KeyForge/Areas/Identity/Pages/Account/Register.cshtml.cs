using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using KeyForge.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;
using KeyForge.Core;

namespace KeyForge.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {

        [TempData]
        public string Message { get; set; }
        [BindProperty]
        public string ErrorMsg { get; set; }

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        { 
            [Required(ErrorMessage = "To pole jest wymagane.")]
            [Display(Name = "Nazwa organizatora")]
            public string Company { get; set; }

            [Required(ErrorMessage = "To pole jest wymagane.")]
            [Display(Name = "Adres, pod którym będa odbywać się zawody")]
            public string Address { get; set; }

            [Required(ErrorMessage = "To pole jest wymagane.")]
            [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "To pole jest wymagane.")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Hasło")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potwierdź hasło")]
            [Compare("Password", ErrorMessage = "Hasło i potwierdzenie hasła nie są zgodne.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "To pole jest wymagane.")]
            [Display(Name = "ID potwierdzające")]
            public string AuthId { get; set; }

        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                IEnumerable<RegistrationAuthentication> query = from r in _db.RegistrationAuthentication
                                                                where r.AuthId == Input.AuthId
                                                                select r;
                //Add info that registration was wrong
                //Remove selected row from SQL of RegistrationAuthentication
                if (query.Count() != 0)
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        //Remove data from DB
                        _db.RegistrationAuthentication.Remove(query.FirstOrDefault()); 
                        //Add new company
                        var tempCompany = new Company();
                        tempCompany.Name = Input.Company;
                        tempCompany.Address = Input.Address;
                        tempCompany.Email = Input.Email; 
                        _db.Company.Add(tempCompany);
                        //Save
                        _db.SaveChanges();

                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "VaultManager - Potwierdź e-mail",
                            $"Potwierdź rejestrację konta klikając <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>tutaj</a>.");

                        //await _signInManager.SignInAsync(user, isPersistent: false);

                        ErrorMsg = "Potwierdzenie wysłane na wskazany adres e-mail.";

                        return LocalRedirect(returnUrl);
                    }
                    foreach (var error in result.Errors)
                    {
                        if(error.Code == "DuplicateUserName")
                        {
                            ErrorMsg += "Ten adres e-mail jest już zarejestrowany.\n";
                        }
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ErrorMsg = "Niepoprawne ID potwierdzające!";
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
