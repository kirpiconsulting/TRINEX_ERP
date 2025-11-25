using erpv01.Data;
using erpv01.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace erpv01.Controllers
{
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

            var sonNumara1 = _db.IsEmirleris
                .OrderByDescending(x => x.EvrakNo)
                .Select(x => x.EvrakNo)
                .FirstOrDefault();

            int yeniNumara1 = (int.TryParse(sonNumara1, out int num) ? num : 0) + 1;

            var Tarih = DateTime.Now;

            foreach (var item in liste)
            {
                // İş emri miktarı örn: 5
                int adet = (int)item.IsEmriMiktar;

                if (adet <= 0)
                {
                    Console.WriteLine($"❌ {item.EvrakNo} / {item.KalemKodu} için geçersiz iş emri miktarı!");
                    continue;
                }

                var receteEvrakNo = _db.Recetelers
                    .Where(r => r.StokKod == item.StokKodu)
                    .Select(r => r.EvrakNo)
                    .FirstOrDefault();

                var receteMiktar = _db.Recetelers
                    .Where(r => r.StokKod == item.StokKodu)
                    .Select(r => r.StokMiktar)
                    .FirstOrDefault();


                var receteEkAlan2 = _db.Recetelers
                    .Where(r => r.StokKod == item.StokKodu)
                    .Select(r => r.EkAlan2)
                    .FirstOrDefault();

                if (receteMiktar <= 0)
                {
                    Console.WriteLine($"❌ Reçete miktarı 0");
                    continue;
                }


                if (string.IsNullOrWhiteSpace(receteEvrakNo))
                {
                    Console.WriteLine($"❌ {item.StokKodu} stok kodu için reçete bulunamadı — atlanıyor.");
                    continue;
                }

                var receteKalemleri = _db.ReceteKalemleris
                    .Where(rk => rk.EvrakNo == receteEvrakNo)
                    .ToList();

               

                // İç döngü
                for (int i = 1; i <= adet; i++)
                {

                    var model1 = new IsEmirleri
                    {
                        EvrakNo = yeniNumara1.ToString(),
                        StokKod =  item.StokKodu ,
                        StokBirim = item.Birim,
                        UretimPlani = "P1",
                        PlanlananMiktar = 1,
                        BakiyeMiktar = 1,
                        AcikKapali = "A",
                        CariSiparisEvrakno = item.EvrakNo,
                        CariSiparisKalemKodu = item.KalemKodu,
                        OlusturmaTarihi = Tarih,
                        OlusturanKullanici = "ENTEGRASYON_01",
                        Tarih = Tarih,
                        Durum = 1,
                        EkAlan2 = receteEkAlan2
                    };

                    _db.IsEmirleris.Add(model1);


                    int kksayac = 1;

                    foreach (var rk in receteKalemleri)
                    {
                        var model2 = new IsEmriKalemleri
                        {
                            EvrakNo = yeniNumara1.ToString(),
                            KalemKodu = kksayac,
                            SiraNumarasi = kksayac ,
                            KaynakTipi = rk.KaynakTipi,
                            KaynakKod = rk.KaynakKod,
                            OperasyonKod = rk.OperasyonKod,
                            PlanlananMiktar = (rk.KullanimMiktar/receteMiktar)*1,
                            KalanMiktar = (rk.KullanimMiktar / receteMiktar) * 1,
                            OlusturmaTarihi = Tarih,
                            OlusturanKullanici = "ENTEGRASYON_01",
                            EkAlan2 = rk.EkAlan2,
                            EkAlan3 = rk.EkAlan3,
                            Durum = 1
                        };
                        _db.IsEmriKalemleris.Add(model2);
                        kksayac++;
                    }


                    yeniNumara1++;

                }
            }

            try
            {
                _db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "İş emirleri başarıyla oluşturuldu."
                });
            }
            catch (Exception ex)
            {
                // HATAYI LOG'A YAZ
                Console.WriteLine("❌ HATA: " + ex.Message);

                return Json(new
                {
                    success = false,
                    message = "Kayıt sırasında hata oluştu: " + ex.Message
                });
            }

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
        }





    }
}
