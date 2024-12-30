using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SomberQueenWeb_Axis_.Models;
using SomberQueenWeb_Axis_.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;


namespace SomberQueenWeb_Axis_.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBHelper _dbHelper;

        public AccountController(DBHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _dbHelper.QueryFirstOrDefault<AuthUser>(
                        "SELECT * FROM auth_users WHERE username = @username AND password = @password",
                        new { username = model.Username, password = model.Password });

                    if (user != null)
                    {
                        var userInfo = _dbHelper.QueryFirstOrDefault<User>(
                            "SELECT user_id FROM users WHERE username = @username",
                            new { username = user.username });

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.username),
                            new Claim(ClaimTypes.Role, user.role)
                        };

                        if (userInfo != null)
                        {
                            claims.Add(new Claim("UserId", userInfo.user_id.ToString()));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            new AuthenticationProperties { IsPersistent = model.RememberMe });

                        if (user.role == "victim")
                        {
                            return RedirectToAction("Dashboard", "Victim");
                        }
                        
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during login: {ex.Message}");
                }

                ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre");
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
} 