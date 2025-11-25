using System.Diagnostics;
using erpv01.Models;
using Microsoft.AspNetCore.Mvc;

namespace erpv01.Controllers
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
            // Sadece view dönüyoruz, data JS ile gelecek
            return View();
        }

        // === Toast Grid için Server-Side Pagination Endpoint'i ===
        [HttpPost]
        public IActionResult PagedData([FromBody] GridRequest dto)
        {
            if (dto == null || dto.page <= 0 || dto.pageSize <= 0)
            {
                return Json(new
                {
                    data = new List<object>(),
                    page = 1,
                    totalCount = 0
                });
            }

            int page = dto.page;          // 1,2,3...
            int pageSize = dto.pageSize;  // 50, 100 vs.
            int skip = (page - 1) * pageSize;

            // Simüle edilmiþ 10.000 satýr
            var allData = Enumerable.Range(1, 10000).Select(i => new
            {
                Id = i,
                Kod = "STK-" + i.ToString("00000"),
                Ad = "Stok Ürünü " + i,
                Miktar = Math.Round(Random.Shared.NextDouble() * 1000, 2),
                Tarih = DateTime.Now.AddDays(-Random.Shared.Next(0, 365)).ToString("yyyy-MM-dd")
            });

            int totalCount = allData.Count();

            var pageData = allData
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return Json(new
            {
                data = pageData,
                page = page,
                totalCount = totalCount
            });
        }

        public class GridRequest
        {
            public int page { get; set; }
            public int pageSize { get; set; }
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
