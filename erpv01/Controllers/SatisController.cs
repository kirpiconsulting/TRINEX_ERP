using erpv01.Data;
using erpv01.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace erpv01.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SatisController : Controller
    {
        private readonly AppDbContext _db;

        public SatisController(AppDbContext db)
        {
            _db = db;
        }

        // --- DTO BURADA TANIMLANIYOR ---
        public class SatisListeDto
        {
            public string EvrakNo { get; set; }
            public DateTime Tarih { get; set; }
            public string CariKodu { get; set; }
            public string CariUnvani { get; set; }
        }

        public class SatisKalemListeDto
        {
            public string EvrakNo { get; set; }
            public DateTime SiparisTarihi { get; set; }
            public string CariKodu { get; set; }
            public string CariAdi { get; set; }

            public string KalemKodu { get; set; }
            public string StokKodu { get; set; }
            public string Birim { get; set; }
            public decimal Miktar { get; set; }
            public DateTime? TeslimTarihi { get; set; }

            public string StokAdi { get; set; }

            public decimal AcilanIsEmriMiktari { get; set; }
        }


        public IActionResult Liste1()
        {
            var liste = _db.SatisSiparisleris
                .Select(s => new SatisListeDto
                {
                    EvrakNo = s.EvrakNo,
                    Tarih = s.Tarih,
                    CariKodu = s.CariKod,

                    // SQL'deki subquery'nin EF karşılığı
                    CariUnvani = _db.Carilers
                                    .Where(c => c.Kod == s.CariKod)
                                    .Select(c => c.Ad)
                                    .FirstOrDefault()
                })
                .OrderByDescending(x => x.Tarih)
                .ToList();

            return View(liste);
        }


        public IActionResult Liste2()
        {
            var liste = (
                from kal in _db.SatisSiparisKalemleris
                join sip in _db.SatisSiparisleris
                    on kal.EvrakNo equals sip.EvrakNo
                join c in _db.Carilers
                    on sip.CariKod equals c.Kod into carilerJoin
                from c in carilerJoin.DefaultIfEmpty()
                join s in _db.Stoklars
                    on kal.StokKod equals s.Kod into stokJoin
                from s in stokJoin.DefaultIfEmpty()
                orderby kal.EvrakNo, kal.SiraNumarasi
                select new SatisKalemListeDto
                {
                    EvrakNo = sip.EvrakNo,
                    SiparisTarihi = sip.Tarih,
                    CariKodu = sip.CariKod,
                    CariAdi = c != null ? c.Ad : null,

                    KalemKodu = kal.KalemKodu,
                    StokKodu = kal.StokKod,
                    Birim = kal.StokBirim,
                    Miktar = kal.Miktar,
                    TeslimTarihi = kal.TeslimTarihi,

                    StokAdi = s != null ? s.Ad : null,

                    AcilanIsEmriMiktari = _db.IsEmirleris
                .Where(i =>
                    i.CariSiparisEvrakno == kal.EvrakNo &&
                    i.CariSiparisKalemKodu == kal.KalemKodu)
                .Sum(i => (decimal?)i.PlanlananMiktar) ?? 0
                }
            ).ToList();

            return View(liste);
        }


        [HttpPost]
        public IActionResult IsEmirleriniOlustur([FromBody] List<IsEmriGelenDto> liste)
        {
            if (liste == null || liste.Count == 0)
                return Json(new { success = false, message = "Gönderilen veri yok!" });

            const int YilBaslangicKodu = 1469;
            var simdi = DateTime.Now;
            int yil = simdi.Year;

            var errors = new List<string>();   // <- Hatalar buraya dolacak

            // --- Sayaç Hesaplama ---
            var sonKayitBuYil = _db.IsEmirleris
                .Where(x => x.Tarih.Year == yil)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            int currentCounter;

            if (sonKayitBuYil != null)
            {
                string sonEvrakNo = sonKayitBuYil.EvrakNo ?? "0";
                string basePart = sonEvrakNo.Contains('-')
                    ? sonEvrakNo.Split('-')[0]
                    : sonEvrakNo;

                if (!int.TryParse(basePart, out currentCounter))
                    currentCounter = YilBaslangicKodu - 1;
            }
            else
            {
                currentCounter = YilBaslangicKodu - 1;
            }

            // --- Tüm satırları tek tek dön ---
            foreach (var item in liste)
            {
                int adet = (int)item.IsEmriMiktar;

                // İş emri miktarı kontrolü
                if (adet <= 0)
                {
                    errors.Add($"{item.StokKodu}: İş emri miktarı 0 olamaz.");
                    continue;   // diğer satırlara devam
                }

                // Stok kontrolü
                var stok = _db.Stoklars.FirstOrDefault(s => s.Kod == item.StokKodu);
                if (stok == null)
                {
                    errors.Add($"{item.StokKodu}: Stok kartı bulunamadı.");
                    continue;
                }

                string yyKodu = (stok.OzelKod05 ?? "00").Trim();

                // Reçete evrak no kontrolü
                var receteEvrakNo = _db.Recetelers
                    .Where(r => r.StokKod == item.StokKodu)
                    .Select(r => r.EvrakNo)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(receteEvrakNo))
                {
                    errors.Add($"{item.StokKodu}: Reçete bulunamadı.");
                    continue;
                }

                var receteMiktar = _db.Recetelers
                    .Where(r => r.StokKod == item.StokKodu)
                    .Select(r => r.StokMiktar)
                    .FirstOrDefault();

                if (receteMiktar <= 0)
                {
                    errors.Add($"{item.StokKodu}: Reçete stok miktarı 0.");
                    continue;
                }

                var receteKalemleri = _db.ReceteKalemleris
                    .Where(rk => rk.EvrakNo == receteEvrakNo)
                    .OrderBy(rk => rk.SiraNumarasi)
                    .ToList();

                // --- Artık bu satır geçerli → adet kadar iş emri oluştur ---
                for (int i = 1; i <= adet; i++)
                {
                    currentCounter++;
                    string baseText = currentCounter.ToString("D6");
                    string evrakNo = $"{baseText}-{yyKodu}";

                    var model1 = new IsEmirleri
                    {
                        EvrakNo = evrakNo,
                        StokKod = item.StokKodu,
                        StokBirim = item.Birim,
                        UretimPlani = "P1",
                        PlanlananMiktar = 1,
                        BakiyeMiktar = 1,
                        AcikKapali = "A",
                        CariSiparisEvrakno = item.EvrakNo,
                        CariSiparisKalemKodu = item.KalemKodu,
                        OlusturmaTarihi = simdi,
                        OlusturanKullanici = "ENTEGRASYON_01",
                        Tarih = simdi,
                        Durum = 1,
                        Notlar = item.IsEmriNot
                    };
                    _db.IsEmirleris.Add(model1);

                    int kksayac = 1;
                    foreach (var rk in receteKalemleri)
                    {
                        var model2 = new IsEmriKalemleri
                        {
                            EvrakNo = evrakNo,
                            KalemKodu = kksayac,
                            SiraNumarasi = kksayac,
                            KaynakTipi = rk.KaynakTipi,
                            KaynakKod = rk.KaynakKod,
                            OperasyonKod = rk.OperasyonKod,
                            PlanlananMiktar = (rk.KullanimMiktar / receteMiktar),
                            KalanMiktar = (rk.KullanimMiktar / receteMiktar),
                            OlusturmaTarihi = simdi,
                            OlusturanKullanici = "ENTEGRASYON_01",
                            EkAlan2 = rk.EkAlan2,
                            EkAlan3 = rk.EkAlan3,
                            Durum = 1
                        };
                        _db.IsEmriKalemleris.Add(model2);
                        kksayac++;
                    }
                }
            }

            try
            {
                _db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "İş emirleri oluşturuldu.",
                    warnings = errors // <- hata olanlar geri döner
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Kayıt sırasında hata oluştu: " + ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult PythonCalistir()
        {
            string pythonExe = @"C:\Users\KIRPI\AppData\Local\Programs\Python\Python313\python.exe";

            string scriptPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "App_PythonScripts",
                "stoklar.py"
            );

            var psi = new ProcessStartInfo();
            psi.FileName = pythonExe;
            psi.Arguments = $"\"{scriptPath}\"";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            var process = Process.Start(psi);

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
                return Content("HATA: " + error);

            return Content(output);
        }


        public class IsEmriGelenDto
        {
            public string EvrakNo { get; set; }
            public string KalemKodu { get; set; }
            public string StokKodu { get; set; }
            public string StokAdi { get; set; }
            public string Birim { get; set; }
            public decimal Miktar { get; set; }
            public decimal IsEmriMiktar { get; set; }
            public string IsEmriNot { get; set; }
        }





    }
}
