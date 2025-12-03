using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using erpv01.Data;
using erpv01.Models.Grid;
using System.ComponentModel.DataAnnotations;

namespace erpv01.Controllers
{
    public class SureKayitlariController : Controller
    {
        private readonly AppDbContext _db;

        public SureKayitlariController(AppDbContext db)
        {
            _db = db;
        }

        // ==============================
        //  DTO – Liste ekranında gösterilecek alanlar
        // ==============================
        public class SureKayitListeDto
        {
            [Display(Name = "Evrak No")]
            public string EvrakNo { get; set; }

            [Display(Name = "Tarih")]
            public DateTime? Tarih { get; set; }

            [Display(Name = "Operatör")]
            public string OperatorKod { get; set; }

            [Display(Name = "Toplam Süre")]
            public decimal ToplamSure { get; set; }

            [Display(Name = "Toplam Dakika")]
            public decimal ToplamDakika { get; set; }

            [Display(Name = "İş Emri No")]
            public string? IsEmriNo { get; set; }

            [Display(Name = "Notlar")]
            public string? Notlar { get; set; }
        }

        // ==============================
        //  Silme isteği DTO’su
        // ==============================
        public class SureKayitlariSilRequest
        {
            public List<string> EvrakNolari { get; set; }
        }

        // ==============================
        //  LİSTE1 – Süre Kayıtları Liste
        // ==============================
        public IActionResult Liste1()
        {
            // SureKayitlariOperatorler tablosundan liste çekiyoruz
            var data = _db.SureKayitlariOperatorlers
                .Select(x => new SureKayitListeDto
                {
                    EvrakNo = x.EvrakNo,
                    Tarih = x.Tarih,
                    OperatorKod = x.OperatorKod,
                    ToplamSure = x.ToplamSure,
                    ToplamDakika = x.ToplamDakika,
                    IsEmriNo = x.IsEmriNo,
                    Notlar = x.Notlar
                })
                .OrderByDescending(x => x.Tarih)
                .ThenBy(x => x.EvrakNo)
                .ToList();

            // Grid kolonlarını modelden otomatik üret
            var kolonlar = GridHelper.BuildColumnsFromType<SureKayitListeDto>();

            var jsonOpts = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var vm = new GridViewModel
            {
                Baslik = "Süre Kayıtları",
                DataJson = JsonSerializer.Serialize(data, jsonOpts),
                ColumnsJson = JsonSerializer.Serialize(kolonlar)
            };

            return View(vm);   // Views/SureKayitlari/Liste1.cshtml
        }

        // ==============================
        //  SİL – Seçili süre kayıtlarını sil
        // ==============================
        [HttpPost]
        public IActionResult Sil([FromBody] SureKayitlariSilRequest request)
        {
            if (request?.EvrakNolari == null || request.EvrakNolari.Count == 0)
            {
                return Json(new { success = false, message = "Silinecek kayıt seçilmedi." });
            }

            var evrakList = request.EvrakNolari
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            if (!evrakList.Any())
            {
                return Json(new { success = false, message = "Geçerli evrak numarası bulunamadı." });
            }

            try
            {
                // 1) ÖNCE KALEMLER
                var kalemler = _db.SureKayitlariKalemleris
                    .Where(k => evrakList.Contains(k.EvrakNo))
                    .ToList();

                if (kalemler.Any())
                {
                    _db.SureKayitlariKalemleris.RemoveRange(kalemler);
                }

                // 2) SONRA OPERATÖRLER
                var operatorler = _db.SureKayitlariOperatorlers
                    .Where(o => evrakList.Contains(o.EvrakNo))
                    .ToList();

                if (operatorler.Any())
                {
                    _db.SureKayitlariOperatorlers.RemoveRange(operatorler);
                }

                // 3) EN SON ANA SÜRE KAYITLARI
                var sureKayitlari = _db.SureKayitlaris
                    .Where(s => evrakList.Contains(s.EvrakNo))
                    .ToList();

                if (sureKayitlari.Any())
                {
                    _db.SureKayitlaris.RemoveRange(sureKayitlari);
                }

                _db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Seçilen süre kayıtları başarıyla silindi."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Silme sırasında hata oluştu: " + ex.Message
                });
            }
        }
    }
}
