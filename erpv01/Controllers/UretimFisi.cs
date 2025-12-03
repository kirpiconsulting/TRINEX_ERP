using erpv01.Data;
using erpv01.Models;
using erpv01.Models.Grid;
using erpv01.Models.Uretim;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class UretimFisiController : Controller
{
    private readonly AppDbContext _db;

    public UretimFisiController(AppDbContext db)
    {
        _db = db;
    }

    public class SilmeIstekDto
    {
        public List<string> EvrakNolari { get; set; }
    }

    [HttpPost]
    public IActionResult Sil([FromBody] SilmeIstekDto dto)
    {
        if (dto?.EvrakNolari == null || dto.EvrakNolari.Count == 0)
            return Json(new { success = false, message = "Silinecek fiş seçilmedi!" });

        foreach (var evrakNo in dto.EvrakNolari)
        {
            // 1) TÜKETİMLERİ SİL
            var tuketimler = _db.UretimFisiTuketimlers
                .Where(t => t.EvrakNo == evrakNo)
                .ToList();

            if (tuketimler.Any())
                _db.UretimFisiTuketimlers.RemoveRange(tuketimler);


            // 2) KALEMLERİ SİL
            var kalemler = _db.UretimFisiKalemleris
                .Where(k => k.EvrakNo == evrakNo)
                .ToList();

            if (kalemler.Any())
                _db.UretimFisiKalemleris.RemoveRange(kalemler);


            // 3) ANA FİŞİ SİL
            var fis = _db.UretimFisleris
                .FirstOrDefault(f => f.EvrakNo == evrakNo);

            if (fis != null)
                _db.UretimFisleris.Remove(fis);
        }

        _db.SaveChanges();

        return Json(new { success = true, message = "Seçilen üretim fişleri başarıyla silindi." });
    }


    public IActionResult Liste1()
    {
        var data = (
            from k in _db.UretimFisiKalemleris
            join f in _db.UretimFisleris on k.EvrakNo equals f.EvrakNo
            select new UretimFisiListeDto
            {
                EvrakNo = k.EvrakNo,
                Tarih = f.Tarih,
                StokKod = k.StokKod,
                Miktar = k.Miktar,
                is_emri_no = k.IsEmriNo,
                lot_no=k.LotNo,
                seri_no=k.SeriNo,
                UretimDeposuKodu=f.UretimDeposuKodu,
                TuketimDeposuKodu=f.TuketimDeposuKodu
            }
        ).ToList();

        var kolonlar = GridHelper.BuildColumnsFromType<UretimFisiListeDto>();

        var jsonCamel = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var vm = new GridViewModel
        {
            Baslik = "Üretim Fişleri",
            DataJson = JsonSerializer.Serialize(data, jsonCamel),
            ColumnsJson = JsonSerializer.Serialize(kolonlar)
        };

        return View(vm);
    }
}
