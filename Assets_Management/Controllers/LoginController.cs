using Assets_Management.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assets_Management.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApiConnect _apiConnect;
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration, ApiConnect apiConnect)
        {
            _configuration = configuration;
            _apiConnect = apiConnect;
        }

        public IActionResult Login()
        {
            ViewBag.ApiBasurl = _apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult Master()
        {
            return View();
        }
    }
}
