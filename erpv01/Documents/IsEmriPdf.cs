using erpv01.Controllers;
using NetBarcode;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;

namespace erpv01.Documents
{
    public static class IsEmriPdf
    {
        // =================================================================================
        // AYARLAR (Yükseklikler)
        // =================================================================================
        private static readonly float H1_Header = 45f;
        private static readonly float H2_Bilgi = 140f;
        private static readonly float H3_Govde = 285f;
        private static readonly float H4_Footer = 95f;

        // =================================================================================
        // ANA METOT
        // =================================================================================
        public static byte[] Olustur(List<IsEmriPdfModel> veriler)
        {
            

            var logoYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "firmalogo.png");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(0.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Content().Column(col =>
                    {
                        col.Spacing(0);

                        for (int i = 0; i < veriler.Count; i++)
                        {

                            var veri = veriler[i];
                            var evrakNo =  veri.EvrakNo ;

                            col.Item().Column(inner =>
                            {
                                inner.Spacing(0);

                                Bolum1_Header(inner, evrakNo, logoYolu);
                                Bolum2_Bilgi(inner, veri);
                                Bolum3_Govde(inner, veri);
                                Bolum4_Alt(inner, veri.Isemrinot);   // <-- REVİZE BURADA YAPILDI
                            });

                            if (i != veriler.Count - 1)
                                col.Item().PageBreak();
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }

        // =================================================================================
        // BÖLÜM 1: HEADER
        // =================================================================================
        private static void Bolum1_Header(ColumnDescriptor col, string evrakNo, string logoYolu)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(1.5f); c.RelativeColumn(3f); c.RelativeColumn(2f); c.RelativeColumn(1f); });

                var cellLogo = table.Cell().Height(H1_Header).Element(Stil_Standart);
                if (File.Exists(logoYolu)) cellLogo.Image(logoYolu).FitArea();
                else cellLogo.Text("Logo Yok");

                table.Cell().Height(H1_Header).Element(Stil_Standart).Text($"PROSES İŞ EMRİ ({evrakNo})").Bold().FontSize(14);

                var barcode = new Barcode(evrakNo, NetBarcode.Type.Code128, showLabel: false, width: 1500, height: 300);
                table.Cell().Height(H1_Header).Element(Stil_Standart).Padding(5).Image(barcode.GetByteArray()).FitArea();

                table.Cell().Height(H1_Header).Element(Stil_Standart).Text(evrakNo).Bold().FontSize(18);
            });
        }

        private static void Bolum2_Bilgi(ColumnDescriptor col, IsEmriPdfModel veri)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); });

                // 1. Müşteri
                table.Cell().Height(H2_Bilgi).Element(Stil_UstCizgisiz).Padding(2).Table(miniTable => {
                    miniTable.ColumnsDefinition(mc => { mc.ConstantColumn(75); mc.RelativeColumn(); });
                    YazMiniSatir(miniTable, "MÜŞTERİ", veri.Musteri);
                    YazMiniSatir(miniTable, "ÜLKE", veri.Ulke);
                    YazMiniSatir(miniTable, "MARKA", veri.Marka);
                    YazMiniSatir(miniTable, "MODEL", veri.Model);
                    YazMiniSatir(miniTable, "YIL", veri.Yil);
                    YazMiniSatir(miniTable, "ÜRÜN ADI", veri.UrunAdi);
                    YazMiniSatir(miniTable, "SEVİYE/KALINLIK", veri.Seviye);
                    YazMiniSatir(miniTable, "ÖLÇÜ (mm)", veri.Olcu);
                    YazMiniSatir(miniTable, "ALAN (m²)", veri.Alan);
                });

                // 2. Özellikler
                table.Cell().Height(H2_Bilgi).Element(Stil_UstCizgisiz).Padding(2).Table(miniTable => {
                    miniTable.ColumnsDefinition(mc => { mc.ConstantColumn(75); mc.RelativeColumn(); });
                    YazMiniSatir(miniTable, "RENK", veri.Renk);
                    YazMiniSatir(miniTable, "LOGO", veri.Logo);
                    YazMiniSatir(miniTable, "FORM TİPİ", veri.FormTipi);
                    YazMiniSatir(miniTable, "TRİM", veri.Trim);
                    YazMiniSatir(miniTable, "OFFSET", veri.Offset);
                    YazMiniSatir(miniTable, "KUŞAK", veri.Kusak);
                    YazMiniSatir(miniTable, "GUNPORT", veri.Gunport);
                    YazMiniSatir(miniTable, "DELİK ÇAPI", veri.DelikCapi);
                    YazMiniSatir(miniTable, "PAKET YÜZEYİ", veri.PaketYuzeyi);
                });

                // 3. Serigrafi
                table.Cell().Height(H2_Bilgi).Element(Stil_UstCizgisiz).Padding(2).Table(miniTable => {
                    miniTable.ColumnsDefinition(mc => { mc.ConstantColumn(85); mc.RelativeColumn(); });
                    YazMiniSatir(miniTable, "SERİGRAFİ", veri.Serigrafi);
                    YazMiniSatir(miniTable, "AYNA BOŞLUĞU", veri.AynaBoslugu);
                    YazMiniSatir(miniTable, "VIN KUTUSU", veri.VinKutusu);
                    YazMiniSatir(miniTable, "YAĞMUR SENSÖRÜ", veri.YagmurSensoru);
                    YazMiniSatir(miniTable, "GÜMÜŞ BOYA", veri.GumusBoya);
                    YazMiniSatir(miniTable, "ANTEN", veri.Anten);
                    YazMiniSatir(miniTable, "BOYALI CAM RO", veri.BoyaliCamRo);
                    YazMiniSatir(miniTable, "NOKTA", veri.Nokta);
                });

                // 4. Ürün Detay
                table.Cell().Height(H2_Bilgi).Element(Stil_UstCizgisiz).Padding(2).Table(miniTable => {
                    miniTable.ColumnsDefinition(mc => { mc.ConstantColumn(85); mc.RelativeColumn(); });
                    YazMiniSatir(miniTable, "ÜRÜN NO", veri.Urunno);
                    YazMiniSatir(miniTable, "ÇELİK", veri.Celik);
                    YazMiniSatir(miniTable, "PAH (CHAFLAN)", veri.Pah);
                    YazMiniSatir(miniTable, "DIŞ OFFSET", veri.DisOffset);
                    YazMiniSatir(miniTable, "SOLAR KONTROL", veri.SolarKontrol);
                    YazMiniSatir(miniTable, "BAĞLANTI KABLOSU", veri.BaglantiKablosu);
                    YazMiniSatir(miniTable, "MONTAJ AÇISI", veri.MontajAcisi);
                });

                // 5. Sipariş
                table.Cell().Height(H2_Bilgi).Element(Stil_UstCizgisiz).Padding(2).Table(miniTable => {
                    miniTable.ColumnsDefinition(mc => { mc.ConstantColumn(95); mc.RelativeColumn(); });
                    YazMiniSatir(miniTable, "SİPARİŞ NO", veri.SiparisNo);
                    YazMiniSatir(miniTable, "SİPARİŞ TİPİ", veri.SiparisTipi);
                    YazMiniSatir(miniTable, "SİPARİŞ TARİHİ", veri.SiparisTarihi);
                    YazMiniSatir(miniTable, "SEVK TARİHİ", veri.SevkTarihi);
                    YazMiniSatir(miniTable, "CNC KESİM PRG", veri.CncKesimPrg);
                    YazMiniSatir(miniTable, "CNC ROUTER PRG", veri.CncRouterPrg);
                    YazMiniSatir(miniTable, "WATERJET PRG", veri.WaterjetPrg);
                    YazMiniSatir(miniTable, "PROSES AKIŞI", veri.ProsesAkisi);
                });
            });
        }

        // =================================================================================
        // BÖLÜM 3: GÖVDE
        // =================================================================================
        // ARTIK MODELİN TAMAMINI ALIYORUZ Kİ REÇETE LİSTESİNE ERİŞELİM
        private static void Bolum3_Govde(ColumnDescriptor col, IsEmriPdfModel veri)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c => { c.RelativeColumn(1.2f); c.RelativeColumn(1f); });

                // 3.1 SOL HÜCRE (RESİM + ONAY) - AYNI KALDI
                table.Cell().Height(H3_Govde).Element(Stil_UstCizgisiz).AlignTop().Column(c =>
                {
                    // A) RESİM ALANI
                    float hResim = H3_Govde * 0.65f;
                    c.Item().Height(hResim).Padding(10).Element(imgContainer =>
                    {
                        byte[] resimBytes = ResimIndir(veri.ResimUrl);
                        if (resimBytes != null && resimBytes.Length > 0) imgContainer.Image(resimBytes).FitArea();
                        else imgContainer.Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().AlignMiddle().Text("RESİM YOK").FontColor(Colors.Red.Medium);
                    });

                    // B) ONAY TABLOSU
                    float hOnayToplam = H3_Govde * 0.35f;
                    float hBaslik = 15f;
                    float hKalan = hOnayToplam - hBaslik;
                    float hSatir = hKalan / 2f;

                    c.Item().Height(hOnayToplam).BorderTop(0.5f).BorderColor(Colors.Black).Table(t =>
                    {
                        t.ColumnsDefinition(cols => { cols.RelativeColumn(1); cols.RelativeColumn(1); cols.RelativeColumn(1.5f); cols.RelativeColumn(1.5f); });
                        t.Cell().Height(hBaslik).BorderRight(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black).AlignCenter().AlignMiddle().Text("LOGO SAYDAM").Bold().FontSize(7);
                        t.Cell().Height(hBaslik).BorderRight(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black).AlignCenter().AlignMiddle().Text("LOGO SİYAH").Bold().FontSize(7);
                        t.Cell().ColumnSpan(2).Height(hBaslik).BorderBottom(0.5f).BorderColor(Colors.Black).AlignCenter().AlignMiddle().Text("ÜRETİM ONAYLARI").Bold().FontSize(7);

                        t.Cell().RowSpan(2).Height(hKalan).BorderColor(Colors.Black);
                        t.Cell().RowSpan(2).Height(hKalan).BorderRight(0.5f).BorderColor(Colors.Black);

                        t.Cell().Height(hSatir).BorderRight(0.5f).BorderBottom(0.5f).BorderColor(Colors.Black).AlignCenter().AlignTop().Padding(2).Text("ÜRÜN DİZAYN/FORMASYON").FontSize(6);
                        t.Cell().Height(hSatir).BorderBottom(0.5f).BorderColor(Colors.Black).AlignCenter().AlignTop().Padding(2).Text("ÜRETİM MÜDÜRÜ").FontSize(6);
                        t.Cell().Height(hSatir).BorderRight(0.5f).BorderColor(Colors.Black).AlignCenter().AlignTop().Padding(2).Text("KALİTE").FontSize(6);
                        t.Cell().Height(hSatir).AlignCenter().AlignTop().Padding(2).Text("GENEL MÜDÜR").FontSize(6);
                    });
                });

                // 3.2 SAĞ HÜCRE (REÇETE TABLOSU) - GÜNCELLENDİ
                table.Cell().Height(H3_Govde).Element(Stil_UstCizgisiz).AlignTop().Padding(5).Column(c =>
                {
                    c.Item().Table(t =>
                    {
                        // SÜTUN AYARLARI (YERLİ/İTHAL BİRLEŞTİ)
                        t.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(25);      // SIRA
                            cols.RelativeColumn(3.5f);    // HAM. KODU
                            cols.RelativeColumn(5f);      // HAM. ADI
                            cols.RelativeColumn(1.5f);    // RENK
                            cols.RelativeColumn(1.5f);    // KALINLIK
                            cols.RelativeColumn(2f);      // YERLİ/İTHAL (Birleşti)
                            cols.RelativeColumn(1.5f);    // AÇIKLAMA
                            cols.RelativeColumn(1.5f);    // KONTROL
                        });

                        // BAŞLIKLAR
                        void BaslikYaz(string txt) => t.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(2).AlignCenter().AlignMiddle().Text(txt).Bold().FontSize(6);

                        BaslikYaz("SIRA");
                        BaslikYaz("HAM. KODU");
                        BaslikYaz("HAM. ADI");
                        BaslikYaz("RENK");
                        BaslikYaz("KALINLIK");
                        BaslikYaz("YERLİ/İTHAL"); // <-- TEK SÜTUN
                        BaslikYaz("AÇIKLAMA");
                        BaslikYaz("KONTROL");

                        // VERİLERİ DÖKÜYORUZ
                        int toplamSatir = veri.ReceteListesi.Count; // Tablo yüksekliği sabit kalsın diye
                        var liste = veri.ReceteListesi;

                        for (int k = 0; k < toplamSatir; k++)
                        {
                            // Yardımcı fonksiyon
                            void VeriYaz(string txt) => t.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).Padding(2).AlignCenter().AlignMiddle().Text(txt).FontSize(6);

                            if (k < liste.Count)
                            {
                                // Gerçek Veri Varsa
                                var satir = liste[k];
                                VeriYaz((k + 1).ToString());
                                VeriYaz(satir.HammaddeKodu);
                                VeriYaz(satir.HammaddeAdi); // Uzun gelirse sığsın diye font küçük
                                VeriYaz(satir.Renk);
                                VeriYaz(satir.Kalinlik);
                                VeriYaz(satir.YerliIthal);
                                VeriYaz(satir.Aciklama);
                                VeriYaz(satir.Kontrol);
                            }
                            else
                            {
                                // Veri bittiyse boş satır bas (Çizgiler devam etsin)
                                VeriYaz(""); VeriYaz(""); VeriYaz(""); VeriYaz("");
                                VeriYaz(""); VeriYaz(""); VeriYaz(""); VeriYaz("");
                            }
                        }
                    });

                    c.Item().PaddingTop(5).AlignRight()
             .Text($"CAM KALINLIK: {veri.CamKalinlik}") // <-- BURASI DEĞİŞTİ
             .Bold().FontSize(9);
                });
            });
        }
        // =================================================================================
        // BÖLÜM 4: ALT (Footer) - TOLERANS KISMI DÜZELTİLDİ
        // =================================================================================
        private static void Bolum4_Alt(ColumnDescriptor col, string isEmriNot)
        {
            col.Item().Height(H4_Footer).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(10); // 1
                    c.RelativeColumn(5);  // 2
                    c.RelativeColumn(5);  // 3
                    c.RelativeColumn(10); // 4
                    c.RelativeColumn(5);  // 5
                    c.RelativeColumn(5);  // 6
                    c.RelativeColumn(50); // 7 (Açıklama)
                    c.RelativeColumn(10); // 8 (Tolerans)
                });

                // --- BAŞLIKLAR ---
                table.Cell().BorderLeft(0.5f).BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).AlignCenter().AlignMiddle().Text("TAKIM ADI").Bold().FontSize(5);
                table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).AlignCenter().AlignMiddle().Text("YER").Bold().FontSize(5);
                table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).AlignCenter().AlignMiddle().Text("KOD").Bold().FontSize(5);

                table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).AlignCenter().AlignMiddle().Text("TAKIM ADI").Bold().FontSize(5);
                table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).AlignCenter().AlignMiddle().Text("YER").Bold().FontSize(5);
                table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).AlignCenter().AlignMiddle().Text("KOD").Bold().FontSize(5);

                table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).AlignCenter().AlignMiddle().Text("AÇIKLAMA").Bold().FontSize(5);

                // Sağ tarafa BorderRight eklendi
                table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).AlignCenter().AlignMiddle().Text("GENEL TOLERANS").Bold().FontSize(5);

                // --- VERİLER ---
                string[] solListe = { "ORJİNAL RESİM", "TEKNİK RESİM", "KESME ŞABLONU", "OFFSET ŞABLONU", "ÇELİK ŞABLON", "SERİGRAFİ KULLANIMI" };
                string[] sagListe = { "ANTEN FİLMİ", "SERİGRAFİ KALIBI", "ANTEN KALIBI", "KAVİSLEME KALIBI", "KAVİS KONTROL ŞABLON", "" };

                // 6 SATIR DÖNÜYORUZ
                for (int i = 0; i < 6; i++)
                {
                    // 1. Grup (Sol)
                    table.Cell().BorderLeft(0.5f).BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).Text(solListe[i]).FontSize(4.5f);
                    table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).Text("");
                    table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).Text("");

                    // 2. Grup (Sağ)
                    string sagYazi = !string.IsNullOrEmpty(sagListe[i]) ? sagListe[i] : "";
                    table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).Text(sagYazi).FontSize(4.5f);
                    table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).Text("");
                    table.Cell().BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black).Padding(1).Text("");

                    // 3. Açıklama (SÜTUN 7)
                    if (i == 0)
                    {
                        table.Cell().RowSpan(6).BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black)
                             .Padding(2).AlignTop().AlignLeft()
                             .Text(isEmriNot).FontSize(5).Italic();
                    }

                    // 4. Tolerans (SÜTUN 8) - İŞTE BURASI DÜZELDİ
                    // Tablo 6 satırlık. Biz bunu 3'er 3'er bölüyoruz.

                    // A) İLK YARI (i=0 iken 3 satır birleştir) -> "-2 mm"
                    if (i == 0)
                    {
                        table.Cell().RowSpan(3).BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black)
                             .Padding(2).AlignRight().AlignMiddle()
                             .Text("-2 mm").FontSize(5);
                    }

                    // B) İKİNCİ YARI (i=3 iken 3 satır birleştir) -> "2 mm"
                    else if (i == 3)
                    {
                        table.Cell().RowSpan(3).BorderBottom(0.5f).BorderRight(0.5f).BorderColor(Colors.Black)
                             .Padding(2).AlignRight().AlignMiddle()
                             .Text("2 mm").FontSize(5);
                    }

                    // i = 1, 2, 4, 5 için buraya hücre eklemiyoruz, RowSpan onları kapsıyor.
                }
            });
        }

        // =================================================================================
        // YARDIMCILAR
        // =================================================================================
        private static IContainer Stil_Standart(IContainer c) =>
            c.Border(0.5f).BorderColor(Colors.Black).Padding(2).AlignMiddle().AlignCenter();

        private static IContainer Stil_UstCizgisiz(IContainer c) =>
            c.Border(0.5f).BorderTop(0).BorderColor(Colors.Black).Padding(0).AlignMiddle().AlignCenter();

        private static void YazMiniSatir(TableDescriptor t, string baslik, string deger)
        {
            t.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(1).Text(baslik).Bold().FontSize(7);
            t.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(1).AlignRight().Text(deger).Bold().FontSize(7);
        }


        private static byte[] ResimIndir(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    return client.GetByteArrayAsync(url).Result;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}