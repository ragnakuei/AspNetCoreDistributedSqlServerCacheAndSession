using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetCoreSession.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace AspNetCoreSession.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor    _contextAccessor;
        private readonly IDistributedCache       _cache;

        public HomeController(ILogger<HomeController> logger,
                              IHttpContextAccessor    contextAccessor,
                              IDistributedCache       cache)
        {
            _logger          = logger;
            _contextAccessor = contextAccessor;
            _cache           = cache;
        }

        public string SessionId => _contextAccessor.HttpContext.Session.Id;

        public IActionResult Index()
        {
            ViewBag.SessionId = SessionId;

            _contextAccessor.HttpContext.Session.SetString("name", "Kuei");
            ViewBag.SessionData = _contextAccessor.HttpContext.Session.GetString("name");

            _contextAccessor.HttpContext.Session.Set("Test", Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
            ViewBag.SessionByteArrayData = Encoding.UTF8.GetString(_contextAccessor.HttpContext.Session.Get("Test"));

            _cache.Set("TestCache", Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
            ViewBag.SessionByteArrayData = Encoding.UTF8.GetString(_cache.Get("TestCache"));

            return View();
        }
    }
}
