using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StreamingService.Data;
using StreamingService.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

//var userId = Request.Cookies["Preference"]; get cookie data
namespace StreamingService.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountController(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        {
            _db = serviceProvider.GetRequiredService<ApplicationDbContext>();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (_httpContextAccessor.HttpContext?.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = await Authenticate(user);
            if (userId != 0)
            {
                _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", userId);
                return RedirectToAction("Index", "Home");
            }
            ViewData["WrongModel"] = "Email or Password is incorrect";
            return View();
        }

        public IActionResult Register()
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, IFormFile imageFile)
        {


            User? repeatedEmail = await _db.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
            if (repeatedEmail != null)
            {
                ViewData["WrongModel"] = "Email is already in use, try another one";
                return View();
            }

            

            var fileName = GetUniqueName(imageFile.FileName);
            var filePathNewImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UserLogos", fileName);

            using (var stream = new FileStream(filePathNewImage, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            user.ImagePath = "/UserLogos/" + fileName;


            // Generate a random salt value
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            User userToBeAdded;

            // Hash the user's password with the salt
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.UserPassword + Convert.ToBase64String(salt)));

                // Store the hashed password and the salt in the database
                // (e.g. using a database context or repository)
                userToBeAdded = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserPassword = "none",
                    Nickname = user.Nickname,
                    PasswordHash = Convert.ToBase64String(hashedPassword),
                    Salt = salt,
                    ImagePath = user.ImagePath
                    
                };
            }

            await _db.Users.AddAsync(userToBeAdded);
            await _db.SaveChangesAsync();

            int userId = await Authenticate(user);

            _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", userId);
            return RedirectToAction("Index", "Home");

        }

        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public async Task<int> Authenticate(User user)
        {
            if (user != null)
            {
                User? userData = await _db.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
                if (userData != null)
                {
                    byte[] salt = userData.Salt;
                    using (var sha256 = SHA256.Create())
                    {
                        byte[] hashedPassword = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.UserPassword + Convert.ToBase64String(salt)));

                        // Compare the computed hash with the stored hash
                        if (Convert.ToBase64String(hashedPassword) == userData.PasswordHash)
                        {
                            // Passwords match - authenticate the user
                            return userData.Id;
                        }
                        else
                        {
                            // Passwords do not match - reject the login attempt
                            return 0;
                        }
                    }
                }

            }
            return 0;
        }

        string GetUniqueName(string fileName)
        {
            return DateTime.UtcNow.Year.ToString() + "-" +
                                        DateTime.UtcNow.Month.ToString() + "-" +
                                        DateTime.UtcNow.Day.ToString() + "-" +
                                        DateTime.UtcNow.Hour.ToString() + "-" +
                                        DateTime.UtcNow.Minute.ToString() + "-" +
                                        DateTime.UtcNow.Second.ToString() + "-" +
                                        DateTime.UtcNow.Millisecond.ToString() + "-" + Regex.Replace(fileName, "[^a-zA-Z0-9.]", "-").Replace(" ", "-");
        }
    }
}
