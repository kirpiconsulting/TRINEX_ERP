using erpv01.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace erpv01.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StokController : Controller
    {

        private readonly AppDbContext _db;

        public StokController(AppDbContext db)
        {
            _db = db;
        }

        public class StokListeDto
        {
            public string Kod { get; set; }
            public string Ad { get; set; }
            public string Birim { get; set; }
            public string Ozellikler { get; set; }
            public string Resim { get; set; }
        }
        public IActionResult Liste1()
        {
            var liste = _db.Stoklars
                .Select(s => new StokListeDto
                {
                    Kod = s.Kod,
                    Ad = s.Ad,
                    Birim = s.AnaBirim,
                    Ozellikler = s.EkAlan3,
                    Resim = s.EkAlan4

                })
                .OrderByDescending(x => x.Kod)
                .ToList();

            return View(liste);
        }
    }
}
