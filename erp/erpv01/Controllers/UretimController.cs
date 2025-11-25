using erpv01.Data;
using erpv01.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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




        // ========== DTO'LAR ==========
        public class OperatorDto
        {
            public string Kod { get; set; }
            public string Ad { get; set; }
            public string CalisabilecegiTezgahlar { get; set; }
        }

        public class BekleyenIsEmriRequest
        {
            public string OperatorKod { get; set; }
            public List<string> Tezgahlar { get; set; }
        }

        public class TamamlananIsEmriRequest
        {
            public string OperatorKod { get; set; }
            public List<string> Tezgahlar { get; set; }
        }

        public class LotCozumDto
        {
            public string StokKodu { get; set; }
            public string StokAdi { get; set; }
            public string StokBirim { get; set; }
            public string DepoKodu { get; set; }
            public string DepoAdi { get; set; }
            public string LotKodu { get; set; }
            public decimal Miktar { get; set; }
        }

        public class StokGetirRequest
        {
            public string EvrakNo { get; set; }
            public string TezgahKodu { get; set; }
        }

        public class LotModel
        {
            public string Kod { get; set; }
            public decimal Miktar { get; set; }
        }

        public class UretimBildirimSatirDto
        {
            public string StokKodu { get; set; }
            public string StokAdi { get; set; }
            public string StokBirim { get; set; }
            public string DepoKodu { get; set; }
            public string DepoAdi { get; set; }
            public string LotKodu { get; set; }
            public decimal Miktar { get; set; }
        }

        public class UretimBildirimKayitRequest
        {
            public string IsEmriNo { get; set; }
            public string KalemKodu { get; set; }
            public string TezgahAdi { get; set; }
            public string BaslangicTarihi { get; set; }

            public string UrunStokKodu { get; set; }
            public string UrunStokAdi { get; set; }
            public string UrunBirim { get; set; }
            public string UrunOzellikler { get; set; }

            public string OperatorKod { get; set; }

            public List<UretimBildirimSatirDto> Satirlar { get; set; }
        }

        // ========== 1) SAYFA (GET) ==========
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

        // ========== 2) BEKLEYEN İŞ EMİRLERİ (POST) ==========
        [HttpPost]
        public IActionResult BekleyenIsEmirleri([FromBody] BekleyenIsEmriRequest dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.OperatorKod))
                return Json(new { success = false, message = "Operatör bilgisi gelmedi!" });

            if (dto.Tezgahlar == null || dto.Tezgahlar.Count == 0)
                return Json(new { success = false, message = "Operatörün çalışabileceği tezgah bulunamadı!" });

            var tezgahListesi = dto.Tezgahlar
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

            if (tezgahListesi.Count == 0)
                return Json(new { success = false, message = "Geçerli tezgah bulunamadı!" });

            var liste = (
                from kal in _db.IsEmriKalemleris
                where kal.KaynakTipi == 0
                      && kal.Durum == 2
                      && tezgahListesi.Contains(kal.KaynakKod)
                orderby kal.EvrakNo, kal.SiraNumarasi
                select new
                {
                    isEmriNumarasi = kal.EvrakNo,
                    kalemKodu = kal.KalemKodu,
                    tezgahKodu = kal.KaynakKod,
                    tezgahAdi = _db.Tezgahlars
                        .Where(t => t.Kod == kal.KaynakKod)
                        .Select(t => t.Ad)
                        .FirstOrDefault(),
                    baslangicTarihi = kal.GerceklesenBaslama,
                    stokKodu = _db.IsEmirleris
                        .Where(i => i.EvrakNo == kal.EvrakNo)
                        .Select(i => i.StokKod)
                        .FirstOrDefault(),
                    stokAdi = _db.Stoklars
                        .Where(s => s.Kod == (
                            _db.IsEmirleris
                                .Where(i => i.EvrakNo == kal.EvrakNo)
                                .Select(i => i.StokKod)
                                .FirstOrDefault()
                        ))
                        .Select(s => s.Ad)
                        .FirstOrDefault(),
                    birim = _db.IsEmirleris
                        .Where(i => i.EvrakNo == kal.EvrakNo)
                        .Select(i => i.StokBirim)
                        .FirstOrDefault(),
                    stokOzellikler = _db.Stoklars
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

            return Json(new { success = true, data = liste });
        }




        [HttpPost]
        public IActionResult TamamlananIsEmirleri([FromBody] TamamlananIsEmriRequest dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.OperatorKod))
                return Json(new { success = false, message = "Operatör bilgisi gelmedi!" });

            if (dto.Tezgahlar == null || dto.Tezgahlar.Count == 0)
                return Json(new { success = false, message = "Operatörün çalışabileceği tezgah bulunamadı!" });

            var tezgahListesi = dto.Tezgahlar
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

            if (tezgahListesi.Count == 0)
                return Json(new { success = false, message = "Geçerli tezgah bulunamadı!" });

            var bugun = DateTime.Today;

            var liste = (
                from kal in _db.IsEmriKalemleris
                where kal.KaynakTipi == 0
                      && kal.Durum == 3
                      && tezgahListesi.Contains(kal.KaynakKod)
                      && kal.GerceklesenBitis.HasValue
                      && kal.GerceklesenBitis.Value.Date == bugun
                orderby kal.EvrakNo, kal.SiraNumarasi
                select new
                {
                    isEmriNumarasi = kal.EvrakNo,
                    kalemKodu = kal.KalemKodu,
                    tezgahKodu = kal.KaynakKod,
                    tezgahAdi = _db.Tezgahlars
                        .Where(t => t.Kod == kal.KaynakKod)
                        .Select(t => t.Ad)
                        .FirstOrDefault(),
                    baslangicTarihi = kal.GerceklesenBaslama,
                    bitisTarihi = kal.GerceklesenBitis,
                    stokKodu = _db.IsEmirleris
                        .Where(i => i.EvrakNo == kal.EvrakNo)
                        .Select(i => i.StokKod)
                        .FirstOrDefault(),
                    stokAdi = _db.Stoklars
                        .Where(s => s.Kod == (
                            _db.IsEmirleris
                                .Where(i => i.EvrakNo == kal.EvrakNo)
                                .Select(i => i.StokKod)
                                .FirstOrDefault()
                        ))
                        .Select(s => s.Ad)
                        .FirstOrDefault(),
                    birim = _db.IsEmirleris
                        .Where(i => i.EvrakNo == kal.EvrakNo)
                        .Select(i => i.StokBirim)
                        .FirstOrDefault(),
                    stokOzellikler = _db.Stoklars
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

            return Json(new { success = true, data = liste });
        }




        // ========== 3) STOK + LOT HESABI (POST) ==========
        [HttpPost]
        public IActionResult StoklariGetir([FromBody] StokGetirRequest dto)
        {
            var evrakNo = dto.EvrakNo;
            var tezgahKod = dto.TezgahKodu;

            var kalemler = (
                from k in _db.IsEmriKalemleris
                where k.KaynakTipi == 2
                      && k.EvrakNo == evrakNo
                      && k.EkAlan2 == tezgahKod
                select new
                {
                    StokKodu = k.KaynakKod,
                    Miktar = k.PlanlananMiktar,
                    DepoKodu = k.EkAlan3,
                    StokAdi = _db.Stoklars.Where(s => s.Kod == k.KaynakKod).Select(s => s.Ad).FirstOrDefault(),
                    StokBirim = _db.Stoklars.Where(s => s.Kod == k.KaynakKod).Select(s => s.AnaBirim).FirstOrDefault(),
                    DepoAdi = _db.Depolars.Where(d => d.Kod == k.EkAlan3).Select(d => d.Ad).FirstOrDefault()
                }
            ).ToList();

            var stokDepoGruplari = kalemler
                .Select(x => new { x.StokKodu, x.DepoKodu })
                .Distinct()
                .ToList();

            var lotHavuzu = new Dictionary<string, List<LotModel>>();

            foreach (var grp in stokDepoGruplari)
            {
                var lotlar = _db.LotSeriTanimlaris
                    .Where(l => l.StokKod == grp.StokKodu
                                && l.DepoKod == grp.DepoKodu
                                && l.StokMiktar > 0)
                    .OrderBy(l => l.Tarih1)
                    .ThenBy(l => l.Kod)
                    .Select(l => new LotModel
                    {
                        Kod = l.Kod,
                        Miktar = l.StokMiktar
                    })
                    .ToList();

                string key = $"{grp.StokKodu}|{grp.DepoKodu}";
                lotHavuzu[key] = lotlar;
            }

            var sonucList = new List<LotCozumDto>();

            foreach (var kal in kalemler)
            {
                decimal buSatirIcinLazim = kal.Miktar;
                string key = $"{kal.StokKodu}|{kal.DepoKodu}";

                if (!lotHavuzu.ContainsKey(key))
                {
                    sonucList.Add(new LotCozumDto
                    {
                        StokKodu = kal.StokKodu,
                        StokAdi = kal.StokAdi,
                        StokBirim = kal.StokBirim,
                        DepoKodu = kal.DepoKodu,
                        DepoAdi = kal.DepoAdi,
                        LotKodu = "",
                        Miktar = buSatirIcinLazim
                    });
                    continue;
                }

                var uygunLotlar = lotHavuzu[key];

                while (buSatirIcinLazim > 0)
                {
                    var aktifLot = uygunLotlar.FirstOrDefault(x => x.Miktar > 0);

                    if (aktifLot == null)
                    {
                        sonucList.Add(new LotCozumDto
                        {
                            StokKodu = kal.StokKodu,
                            StokAdi = kal.StokAdi,
                            StokBirim = kal.StokBirim,
                            DepoKodu = kal.DepoKodu,
                            DepoAdi = kal.DepoAdi,
                            LotKodu = "",
                            Miktar = buSatirIcinLazim
                        });
                        buSatirIcinLazim = 0;
                    }
                    else
                    {
                        decimal kullanim = Math.Min(buSatirIcinLazim, aktifLot.Miktar);

                        sonucList.Add(new LotCozumDto
                        {
                            StokKodu = kal.StokKodu,
                            StokAdi = kal.StokAdi,
                            StokBirim = kal.StokBirim,
                            DepoKodu = kal.DepoKodu,
                            DepoAdi = kal.DepoAdi,
                            LotKodu = aktifLot.Kod,
                            Miktar = kullanim
                        });

                        aktifLot.Miktar -= kullanim;
                        buSatirIcinLazim -= kullanim;
                    }
                }
            }

            return Json(new { success = true, data = sonucList });
        }

        // ========== 4) ÜRETİM BİLDİRİMİ KAYDET ==========
        [HttpPost]
        public IActionResult UretimBildirimiKaydet([FromBody] UretimBildirimKayitRequest veri)
        {
            var Tarih = DateTime.Now;

            try
            {
                if (veri == null)
                    return Json(new { success = false, message = "İstek verisi (body) boş geldi (veri null)." });

                if (veri.Satirlar == null)
                    return Json(new { success = false, message = "Satır bilgisi gelmedi." });

                var lotKontrol = LotStokKontrolu(veri.Satirlar);
                if (!lotKontrol.basarili)
                    return Json(new { success = false, message = lotKontrol.mesaj });

                var isemri = _db.IsEmirleris.FirstOrDefault(x => x.EvrakNo == veri.IsEmriNo);
                if (isemri == null)
                    return Json(new { success = false, message = "İş emri bulunamadı." });

                var sureKaydi = _db.SureKayitlaris
                    .FirstOrDefault(x =>
                        x.IsEmriNo == veri.IsEmriNo &&
                        Convert.ToInt32(x.IsEmriKalemKodu) == Convert.ToInt32(veri.KalemKodu) &&
                        x.KayitBaslangic != null &&
                        x.KayitBitis == null);

                if (sureKaydi != null)
                {
                    sureKaydi.KayitBitis = Tarih;
                    sureKaydi.GuncelleyenKullanici = veri.OperatorKod ?? "ENTEGRASYON-01";
                    sureKaydi.GuncellemeTarihi = Tarih;
                }

                var sonNumara1 = _db.SureKayitlaris
                .Where(x => x.EvrakNo != null && x.EvrakNo != "")
                .AsEnumerable() // ---> Burada EF'den çıkıyoruz
                .Where(x => int.TryParse(x.EvrakNo, out _))
                .Select(x => Convert.ToInt32(x.EvrakNo))
                .DefaultIfEmpty(0)
                .Max();

                int yeniNumara1 = sonNumara1;



                int sayac1 = 1;
                foreach (var satir in veri.Satirlar)
                {
                    var model1 = new SureKayitlariKalemleri
                    {
                        EvrakNo = sureKaydi.EvrakNo,
                        KalemKodu = sayac1,
                        SiraNumarasi = sayac1,
                        StokKod = satir.StokKodu,
                        StokMiktar = satir.Miktar,
                        StokBirim = satir.StokBirim,
                        LotNo = satir.LotKodu,
                        Tarih = Tarih,
                        OlusturmaTarihi = Tarih,
                        OlusturanKullanici = veri.OperatorKod ?? "ENTEGRASYON-01",
                        TuketimDeposuKodu = satir.DepoKodu
                    };
                    _db.SureKayitlariKalemleris.Add(model1);
                    sayac1++;
                }

                var model2 = new SureKayitlariOperatorler
                {
                    EvrakNo = sureKaydi.EvrakNo,
                    SiraNumarasi = 1,
                    OperatorKod = veri.OperatorKod,
                    KalemKodu = 1,
                    Tarih = Tarih,
                    OlusturanKullanici = veri.OperatorKod ?? "ENTEGRASYON-01",
                    OlusturmaTarihi = Tarih
                };
                _db.SureKayitlariOperatorlers.Add(model2);

                var kalemler = _db.IsEmriKalemleris
                    .Where(x => x.EvrakNo == veri.IsEmriNo && x.KaynakTipi == 0)
                    .OrderBy(x => x.SiraNumarasi)
                    .ToList();

                if (kalemler.Any())
                {
                    var ilkSira = kalemler.First().SiraNumarasi;
                    var sonSira = kalemler.Last().SiraNumarasi;

                    var aktifSatir = kalemler.FirstOrDefault(x => x.KalemKodu.ToString() == veri.KalemKodu.ToString());

                    if (aktifSatir != null)
                    {
                        aktifSatir.Durum = 3;
                        aktifSatir.BitisTarihi = Tarih;
                        aktifSatir.GerceklesenBitis = Tarih;
                        aktifSatir.GuncellemeTarihi = Tarih;
                        aktifSatir.GuncelleyenKullanici = veri.OperatorKod ?? "ENTEGRASYON-01";

                        if (aktifSatir.SiraNumarasi == ilkSira || aktifSatir.SiraNumarasi != sonSira)
                        {
                            yeniNumara1++;

                            var sonrakiKalem = kalemler
                                .Where(x => x.SiraNumarasi > aktifSatir.SiraNumarasi)
                                .OrderBy(x => x.SiraNumarasi)
                                .FirstOrDefault();

                            sonrakiKalem.Durum = 2;
                            sonrakiKalem.BaslangicTarihi = Tarih;
                            sonrakiKalem.GerceklesenBaslama = Tarih;


                            if (sonrakiKalem != null)
                            {
                                var model3 = new SureKayitlari
                                {
                                    EvrakNo = yeniNumara1.ToString(),
                                    Tarih = Tarih,
                                    IsEmriNo = sonrakiKalem.EvrakNo,
                                    OperasyonKod = sonrakiKalem.OperasyonKod,
                                    KayitTipi = 1,
                                    IslemKodu = 1,
                                    StokKod = isemri.StokKod,
                                    IsEmriKalemKodu = sonrakiKalem.KalemKodu,
                                    KayitBaslangic = Tarih,
                                    OlusturanKullanici = veri.OperatorKod ?? "ENTEGRASYON-01",
                                    OlusturmaTarihi = Tarih
                                };
                                _db.SureKayitlaris.Add(model3);

                                
                            }
                        }
                        else if (aktifSatir.SiraNumarasi == sonSira)
                        {

                            _db.SaveChanges();
                            isemri.UretilenMiktar = 1;
                            isemri.BakiyeMiktar = 0;
                            isemri.KapanisTarihi = Tarih;
                            isemri.Durum = 3;
                            isemri.AcikKapali = "K";
                            isemri.GuncellemeTarihi = Tarih;
                            isemri.GuncelleyenKullanici = veri.OperatorKod ?? "ENTEGRASYON-01";

                            var sonNumara10 = _db.UretimFisleris
                                .OrderByDescending(x => x.EvrakNo)
                                .Select(x => x.EvrakNo)
                                .FirstOrDefault();

                            int yeniNumara10 = (int.TryParse(sonNumara10, out int num10) ? num10 : 0) + 1;

                            var m1 = new UretimFisleri
                            {
                                EvrakNo = yeniNumara10.ToString(),
                                Tarih = Tarih,
                                UretimDeposuKodu = isemri.EkAlan2,
                                OlusturmaTarihi = Tarih,
                                OlusturanKullanici = veri.OperatorKod ?? "ENTEGRASYON-01"
                            };
                            _db.UretimFisleris.Add(m1);

                            var m2 = new UretimFisiKalemleri
                            {
                                EvrakNo = yeniNumara10.ToString(),
                                KalemKodu = "1",
                                SiraNumarasi = 1,
                                StokKod = isemri.StokKod,
                                Miktar = 1,
                                Birim = isemri.StokBirim,
                                OlusturmaTarihi = Tarih,
                                OlusturanKullanici = veri.OperatorKod ?? "ENTEGRASYON-01",
                                UretimDeposuKodu = isemri.EkAlan2
                            };
                            _db.UretimFisiKalemleris.Add(m2);

                            //var kayit = _db.SureKayitlaris.FirstOrDefault(x => x.IsEmriNo == isemri.EvrakNo);

                            var kayitlar = _db.SureKayitlaris
                                .Where(x => x.IsEmriNo == isemri.EvrakNo)
                                .ToList();





                            if (kayitlar != null && kayitlar.Count > 0)
                            {
                                var evrakNolari = kayitlar
                                    .Select(x => x.EvrakNo)
                                    .Distinct()
                                    .ToList();


                                var surekalemleri = _db.SureKayitlariKalemleris
                                    .Where(x => evrakNolari.Contains(x.EvrakNo))
                                    .ToList();




                                int sayac = 1;
                                foreach (var surek in surekalemleri)
                                {
                                    var m3 = new UretimFisiTuketimler
                                    {
                                        EvrakNo = yeniNumara10.ToString(),
                                        KalemKodu = sayac.ToString(),
                                        SiraNumarasi = sayac,
                                        StokKod = surek.StokKod,
                                        LotNo = surek.LotNo,
                                        Miktar = surek.StokMiktar,
                                        Birim = surek.StokBirim,
                                        Tarih = Tarih,
                                        KarsiStokKod = isemri.StokKod,
                                        KarsiMiktar = 1,
                                        OlusturmaTarihi = Tarih,
                                        OlusturanKullanici = veri.OperatorKod ?? "ENTEGRASYON-01",
                                        TuketimDeposuKodu = surek.TuketimDeposuKodu
                                    };
                                    _db.UretimFisiTuketimlers.Add(m3);
                                    sayac++;
                                }
                            }
                        }
                    }
                }

                _db.SaveChanges();

                return Json(new { success = true, message = "Kayıt işlemi başarılı" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Sunucu Hatası: " + ex.Message });
            }
        }



        private (bool basarili, string mesaj) LotStokKontrolu(List<UretimBildirimSatirDto> gelenSatirlar)
        {
            // 1) İstenenleri Grupla
            var grupluListe = gelenSatirlar
                .Where(x => !string.IsNullOrEmpty(x.LotKodu))
                .GroupBy(x => new { x.StokKodu, x.DepoKodu, x.LotKodu })
                .Select(g => new
                {
                    g.Key.StokKodu,
                    g.Key.DepoKodu,
                    g.Key.LotKodu,
                    ToplamIstenen = g.Sum(x => x.Miktar)
                })
                .ToList();

            foreach (var kalem in grupluListe)
            {
                // 2) DB'den veriyi çek
                var dbLotListesi = _db.LotSeriTanimlaris
                    .Where(l => l.StokKod == kalem.StokKodu
                                && l.DepoKod == kalem.DepoKodu
                                && l.Kod == kalem.LotKodu)
                    .ToList();


                foreach (var l in dbLotListesi)
                {
                    Console.WriteLine($"Type: {l.StokMiktar.GetType()} | Value: {l.StokMiktar}");
                }


                decimal dbMevcutStok = 0;

                foreach (var l in dbLotListesi)
                {
                    // HATA DÜZELTİLDİ: 
                    // Senin modelinde StokMiktar zaten 'decimal' olduğu için 
                    // .Value veya .HasValue kullanmıyoruz. Direkt kendisini topluyoruz.
                    dbMevcutStok += l.StokMiktar;
                }

                // --- DEBUG LOGU ---
                // Sayıyı 'Nokta' ile formatlayıp basıyoruz ki gerçek değerini görelim.
                string dbStokStr = dbMevcutStok.ToString(CultureInfo.InvariantCulture);
                string istenenStr = kalem.ToplamIstenen.ToString(CultureInfo.InvariantCulture);

                Console.WriteLine($"--------------------------------------------------");
                Console.WriteLine($"[KONTROL] Lot: {kalem.LotKodu}");
                Console.WriteLine($"   -> DB Stok (Ham): {dbMevcutStok}");
                Console.WriteLine($"   -> DB Stok (Net): {dbStokStr}"); // Burada nokta/virgül karmaşası olmaz
                Console.WriteLine($"   -> İstenen:       {istenenStr}");
                Console.WriteLine($"--------------------------------------------------");

                // 3) Karşılaştırma
                if (dbMevcutStok < kalem.ToplamIstenen)
                {
                    return (false, $"Yetersiz Bakiye! \nLot: {kalem.LotKodu} \nMevcut: {dbMevcutStok:N2} \nİstenen: {kalem.ToplamIstenen:N2}");
                }
            }

            return (true, "Kontrol başarılı");
        }







    }
}
