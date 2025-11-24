using erpv01.Data;
using erpv01.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace erpv01.Controllers
{
    public class UretimController : Controller
    {
        private readonly AppDbContext _db;

        public UretimController(AppDbContext db)
        {
            _db = db;
        }

        // ------------ DTO ----------------
        public class UretimListeDto
        {
            public string EvrakNo { get; set; }
            public DateTime Tarih { get; set; }
            public string StokKod { get; set; }
            public string StokAdi { get; set; }
            public decimal PlanlananMiktar { get; set; }
            public string Birim { get; set; }
            public int? Durum { get; set; }        // ← EKLENDİ
            public string DurumAciklama { get; set; }
            public string AcikKapali { get; set; }
            public string CariEvrakNo { get; set; }
            public string CariKalemKodu { get; set; }
            public string StokOzellikler { get; set; }
        }

        // ------------ LİSTE1 -------------
        public IActionResult Liste1()
        {
            var liste = _db.IsEmirleris
                .Select(i => new UretimListeDto
                {
                    EvrakNo = i.EvrakNo,
                    Tarih = i.Tarih,
                    StokKod = i.StokKod,

                    // stok adı
                    StokAdi = _db.Stoklars
                                .Where(s => s.Kod == i.StokKod)
                                .Select(s => s.Ad)
                                .FirstOrDefault(),

                    // planlanan miktar
                    PlanlananMiktar = i.PlanlananMiktar,
                    Birim = i.StokBirim,
                    Durum = i.Durum,

                    // CASE WHEN karşılığı
                    DurumAciklama =
                        i.Durum == 1 ? "İş emri oluşturuldu" :
                        i.Durum == 2 ? "Üretime gönderildi" :
                        i.Durum == 3 ? "Üretim tamamlandı" :
                        "Bilinmiyor",

                    AcikKapali = i.AcikKapali,
                    CariEvrakNo = i.CariSiparisEvrakno,
                    CariKalemKodu = i.CariSiparisKalemKodu,

                    // stok özellik (ek_alan_3)
                    StokOzellikler = _db.Stoklars
                                        .Where(s => s.Kod == i.StokKod)
                                        .Select(s => s.EkAlan3)
                                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.Tarih)
                .ToList();

            return View(liste);
        }


        public class UretimGonderDto
        {
            public string EvrakNo { get; set; }
            public string StokKod { get; set; }
            public string StokAdi { get; set; }
            public decimal PlanlananMiktar { get; set; }
            public string Birim { get; set; }
            public int Durum { get; set; }
        }


        [HttpPost]
        public IActionResult UretimeGonder([FromBody] List<UretimGonderDto> liste)
        {
            if (liste == null || liste.Count == 0)
            {
                return Json(new { success = false, message = "Gönderilen veri yok!" });
            }

            var Tarih = DateTime.Now;


            var sonNumara1 = _db.SureKayitlaris
                .OrderByDescending(x => x.EvrakNo)
                .Select(x => x.EvrakNo)
                .FirstOrDefault();

            int yeniNumara1 = (int.TryParse(sonNumara1, out int num) ? num : 0) + 1;

            foreach (var item in liste)
            {
                var isEmri = _db.IsEmirleris
                    .FirstOrDefault(x => x.EvrakNo == item.EvrakNo);

                // Eğer DB’de yoksa → atla
                if (isEmri == null)
                {
                    Console.WriteLine($"❌ EvrakNo {item.EvrakNo} için iş emri bulunamadı — atlanıyor.");
                    continue;
                }

                isEmri.Durum = 2;
                isEmri.GuncellemeTarihi = Tarih;
                isEmri.GuncelleyenKullanici = "ENTEGRASYON_01";

                var ilkKalem = _db.IsEmriKalemleris
                    .Where(k => k.EvrakNo == item.EvrakNo && k.KaynakTipi == 0)
                    .OrderBy(k => k.SiraNumarasi)
                    .FirstOrDefault();

                if (ilkKalem == null)
                {
                    Console.WriteLine($"❌ Evrak {item.EvrakNo} için uygun iş emri kalemi yok.");
                    continue;
                }

                ilkKalem.BaslangicTarihi = Tarih;
                ilkKalem.GerceklesenBaslama = Tarih;
                ilkKalem.Durum = 2;
                ilkKalem.GuncellemeTarihi = Tarih;
                ilkKalem.GuncelleyenKullanici = "ENTEGRASYON_01";


                

                var model1 = new SureKayitlari
                {
                    EvrakNo = yeniNumara1.ToString(),
                    Tarih = Tarih,
                    IsEmriNo = ilkKalem.EvrakNo,
                    IsEmriKalemKodu = ilkKalem.KalemKodu,
                    OperasyonKod = ilkKalem.OperasyonKod,
                    KayitTipi = 1,
                    IslemKodu = 1,
                    StokKod = isEmri.StokKod,
                    KayitBaslangic = Tarih,
                    OlusturmaTarihi = Tarih,
                    OlusturanKullanici = "ENTEGRASYON_01"
                };

                

                _db.SureKayitlaris.Add(model1);
                



                yeniNumara1++;

            }

            try
            {
                _db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "İş emirleri başarıyla üretime gönderildi."
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



        public class OperatorDto
        {
            public string Kod { get; set; }
            public string Ad { get; set; }
            public string CalisabilecegiTezgahlar { get; set; }
        }



        public IActionResult UretimBildirimi1()
        {
            var operatorler = _db.Operatorlers
                .Select(o => new OperatorDto
                {
                    Kod = o.Kod,
                    Ad = o.Ad,
                    CalisabilecegiTezgahlar = o.CalisabilecegiTezgahlar
                })
                .ToList();

            return View(operatorler);
        }


        [HttpPost]
        public IActionResult BekleyenIsEmirleri([FromBody] BekleyenIsEmriRequest dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.OperatorKod))
            {
                return Json(new { success = false, message = "Operatör bilgisi gelmedi!" });
            }

            if (dto.Tezgahlar == null || dto.Tezgahlar.Count == 0)
            {
                return Json(new { success = false, message = "Operatörün çalışabileceği tezgah bulunamadı!" });
            }

            var tezgahListesi = dto.Tezgahlar.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

            if (tezgahListesi.Count == 0)
                return Json(new { success = false, message = "Geçerli tezgah bulunamadı!" });

            // -----------------------
            // 📌 BEKLEYEN İŞ EMİRLERİ
            // -----------------------
            var liste = (
                from kal in _db.IsEmriKalemleris
                where kal.KaynakTipi == 0
                      && kal.Durum == 2                                  // bekleyen
                      && tezgahListesi.Contains(kal.KaynakKod)           // operatörün çalışabileceği tezgah
                orderby kal.EvrakNo, kal.SiraNumarasi
                select new
                {
                    isEmriNo = kal.EvrakNo,
                    kalemKodu = kal.KalemKodu,
                    kaynakKod = kal.KaynakKod,
                    kaynakAdi = _db.Tezgahlars
                        .Where(t => t.Kod == kal.KaynakKod)
                        .Select(t => t.Ad)
                        .FirstOrDefault(),

                    mamulKod = _db.IsEmirleris
                        .Where(i => i.EvrakNo == kal.EvrakNo)
                        .Select(i => i.StokKod)
                        .FirstOrDefault(),

                    mamulAdi = _db.Stoklars
                        .Where(s => s.Kod == (
                            _db.IsEmirleris
                                .Where(i => i.EvrakNo == kal.EvrakNo)
                                .Select(i => i.StokKod)
                                .FirstOrDefault()
                        ))
                        .Select(s => s.Ad)
                        .FirstOrDefault(),

                    mamulBirim = _db.IsEmirleris
                        .Where(i => i.EvrakNo == kal.EvrakNo)
                        .Select(i => i.StokBirim)
                        .FirstOrDefault(),

                    mamulOzellik = _db.Stoklars
                        .Where(s => s.Kod == (
                            _db.IsEmirleris
                                .Where(i => i.EvrakNo == kal.EvrakNo)
                                .Select(i => i.StokKod)
                                .FirstOrDefault()
                        ))
                        .Select(s => s.EkAlan3)
                        .FirstOrDefault()
                }
            ).ToList();


            return Json(new
            {
                success = true,
                data = liste
            });
        }

        public class BekleyenIsEmriRequest
        {
            public string OperatorKod { get; set; }
            public List<string> Tezgahlar { get; set; }
        }












    }
}
