using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Создание куки
        private async Task Authenticate(string userName)
        {
            // Claim - некоторая информация о пользователе
            // claims - список из одного элемента Claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim("Color", "pink")
            };

            // ClaimsIdentity - объединенная информация 
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            // Установка cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        // Авторизация
        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string login, string password)
        {
            var user = new Entity.User();
            user.Name = login;  
            user.Password = password;
            bool access = BL.UserBL.Authorization(user);

            if (access)
            {
                await Authenticate(user.Name);
                return RedirectToAction("Index", "Game");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View();
        }

        // Регистрация
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(string login, string password)
        {
            var user = new Entity.User();
            user.Name = login;
            user.Password = password;
            BL.UserBL.AddOrUpdate(user);

            return View();
        }

        // Выход из системы
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}