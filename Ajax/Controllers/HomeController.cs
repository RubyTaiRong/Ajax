using Ajax.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Ajax.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult First()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Address() 
        { 
            return View(); 
        }
        public IActionResult Spots() 
        {
            return View();
        }

        //從Client端 呼叫ASP.NET Core Web API (WebAPI)
        //呼叫時要注意 WebAPI是否在啟動狀態
        public IActionResult Cors()
        {
            return View();
        }

        public IActionResult AutoComplete()
        {
            return View();
        }
        public IActionResult Avatar()
        {
            return View();
        }
        public IActionResult JsonDemo()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
