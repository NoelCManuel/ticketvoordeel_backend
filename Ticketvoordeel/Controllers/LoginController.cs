using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ticketvoordeel.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet("login")]
        public IActionResult Index()
        {
            return View();
        }
    }
}