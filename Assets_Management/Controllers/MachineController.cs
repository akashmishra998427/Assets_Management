using Assets_Management.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assets_Management.Controllers
{
    public class MachineController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApiConnect _apiConnect;

        public MachineController(IConfiguration configuration, ApiConnect apiConnect)
        {
            _configuration = configuration;
            _apiConnect = apiConnect;
        }

        public IActionResult MachineReport()
        {
            ViewBag.ApiBasurl = _apiConnect.WebApiUrl();
            return View();
        }

        public IActionResult MachineComplaint()
        {
            ViewBag.Apiurl = _apiConnect.WebApiUrl();
            return View();
        }

        public IActionResult addComplaint()
        {
            ViewBag.Apiurl = _apiConnect.WebApiUrl();
            return View();
        }

        public IActionResult EditMachineDetails(string id)
        {
            ViewBag.Apiurl = _apiConnect.WebApiUrl();
            return View();
        }

    }
}
