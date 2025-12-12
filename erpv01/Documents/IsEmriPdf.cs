using erpv01.Controllers;
using NetBarcode;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace erpv01.Documents
{
    public static class IsEmriPdf
    {
        /* ============================================================
           1) OTOMATİK SCALE HESAPLAMA HELPER
        ============================================================ */
        public static class PdfScaleHelper
        {
            private const float PtPerCm = 28.346f;

            public static float CmToPt(float cm) => cm * PtPerCm;

            /// <summary>
            /// Tüm layout cm değerlerine göre A4 yatay için otomatik scale hesaplar.
            /// </summary>
            public static float HesaplaScale(
              float toplamGenislikCm,
              float toplamYukseklikCm,
              float marginPt = 20f)
            {
                float a4WidthPt = PageSizes.A4.Landscape().Width - (marginPt * 2);
                float a4HeightPt = PageSizes.A4.Landscape().Height - (marginPt * 2);

                float gerekliWidthPt = CmToPt(toplamGenislikCm);
                float gerekliHeightPt = CmToPt(toplamYukseklikCm);

                float scaleWidth = a4WidthPt / gerekliWidthPt;
                float scaleHeight = a4HeightPt / gerekliHeightPt;

                float scale = Math.Min(scaleWidth, scaleHeight);

                if (scale > 1f) scale = 1f;
                if (scale < 0.45f) scale = 0.45f;

                return scale;
            }
        }

        /* ============================================================
           2) GLOBAL SCALE (otomatik hesaplanıyor)
        ============================================================ */
        private static float Scale = 0.55f;
        private static float Cm(float cm) => cm * 28.346f * Scale;

        /* ============================================================
           3) PDF OLUŞTURMA
        ============================================================ */
        public static byte[] Olustur(List<IsEmriPdfModel> veriler)
        {
            // TÜM BLOKLARIN TOPLAM GERÇEK YÜKSEKLİĞİ (CM)
            float toplamYukseklik =
                1.04f +      // header
                4.15f +      // üst bloklar
                9.88f +      // teknik resim
                1.56f +      // notlar
                3.12f +      // tolerans/logo/onay
                0.52f;       // cam kalınlık

            // GENİŞLİK = Sol blok (16.53 cm) + Sağ blok (12.47 cm)
            float toplamGenislik = 16.53f + 12.47f;

            // SCALE OTOMATİK HESAPLANIYOR
            Scale = PdfScaleHelper.HesaplaScale(
                toplamGenislikCm: toplamGenislik,
                toplamYukseklikCm: toplamYukseklik,
                marginPt: 20f
            );

            Console.WriteLine("📌 Otomatik SCALE = " + Scale);

            var logoYolu = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot", "images", "firmalogo.png");

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(10);
                    page.DefaultTextStyle(x => x.FontFamily("Calibri").FontSize(10));

                    page.Content().Column(content =>
                    {
                        content.Item().Element(e => Sayfa(e, veriler[0], logoYolu));
                    });
                });
            });

            return doc.GeneratePdf();
        }

        /* ============================================================
           4) ANA SAYFA
        ============================================================ */
        private static void Sayfa(IContainer container, IsEmriPdfModel v, string logo)
        {
            container.Column(col =>
            {
                //col.Spacing(3);
                col.Spacing(0);

                col.Item().Element(e => Bolum1_Header(e, v.EvrakNo, logo));
                col.Item().Element(e => Bolum2_UstBloklar(e, v));
                col.Item().Element(e => Bolum3_TeknikVeBilesme(e, v));
                col.Item().Element(e => Bolum4_CamKalinlik(e, v));
            });
        }

        /* ============================================================
           5) BÖLÜM 1 - HEADER
        ============================================================ */
        private static void Bolum1_Header(IContainer container, string evrakNo, string logo)
        {
            container
                .Height(Cm(1.2f))
                .Border(1)
                .Padding(3)
                .Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(Cm(5));
                        c.RelativeColumn();
                        c.ConstantColumn(Cm(5));
                    });

                    // LOGO
                    var cellLogo = t.Cell().AlignLeft().AlignMiddle();
                    if (File.Exists(logo))
                        cellLogo.Image(logo).FitHeight();
                    else
                        cellLogo.Text("Logo Yok");

                    // BAŞLIK
                    t.Cell().AlignCenter().AlignMiddle()
                        .Text(t =>
                        {
                            t.Span("PROSES İŞ EMRİ ").FontSize(20);
                            t.Span("(" + evrakNo + ")").FontSize(22).Bold();
                        });

                    // BARKOD
                    var bc = new Barcode(evrakNo, NetBarcode.Type.Code128, false, 800, 200);
                    t.Cell().AlignRight().AlignMiddle()
                       .Image(bc.GetByteArray()).FitHeight();
                });
        }

        /* ============================================================
           6) BÖLÜM 2 - ÜST BLOKLAR
        ============================================================ */
        private static void Bolum2_UstBloklar(IContainer container, IsEmriPdfModel v)
        {
            container
                .MinHeight(Cm(3f))
                .Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        float blokGenislik = Cm(6f);

                        row.RelativeItem().Width(blokGenislik)
                           .Element(e => Blok(e, new (string, string)[]{
                               ("SİPARİŞ NO", v.CariSiparisEvrakNo),
                               ("SİPARİŞ TARİHİ", v.SiparisTarih?.ToString("dd.MM.yyyy")),
                               ("TERMİN TARİHİ", v.TerminTarihi?.ToString("dd.MM.yyyy")),
                               ("MÜŞTERİ ADI", v.MusteriAdi),
                               ("M. ÜLKESİ", v.MusteriUlkesi),
                               ("STOK KODU", v.StokKodu)
                           }));

                        row.RelativeItem().Width(blokGenislik)
                           .Element(e => Blok(e, new[]{
                               ("MARKA", v.Marka),
                               ("MODEL", v.Model),
                               ("MODEL YILI", v.ModelYil),
                               ("CAM ADI", v.CamAdi),
                               ("SEV.KALINLIK", v.Seviye + " - " + v.Kalinlik),
                               ("ÖLÇÜ (mm)", v.Olcu),
                               ("ALAN", v.Alan.ToString())
                           }));

                        row.RelativeItem().Width(blokGenislik)
                           .Element(e => Blok(e, new[]{
                               ("RENK", v.Renk),
                               ("FORM TİPİ", v.FormTipi),
                               ("OFFSET", v.Offset),
                               ("KUŞAK", v.Kusak),
                               ("REZİSTANS", v.Rezistans),
                               ("GUNPORT DELİĞİ", v.GunportDeligi),
                               ("KENAR BANT", v.Kenarbant)
                           }));

                        row.RelativeItem().Width(blokGenislik)
                           .Element(e => Blok(e, new[]{
                               ("BOY.CAM RODAJ", v.BoyaliCamRodaj),
                               ("PAH", v.Pah),
                               ("TRAM", v.Tram),
                               ("TRİM", v.Trim),
                               ("ÇELİK SAC", v.CelikSac),
                               ("BOYA TİPİ", v.BoyaTipi)
                           }));

                        row.RelativeItem().BorderRight(1).Width(blokGenislik)
                           .Element(e => Blok(e, new[]{
                               ("CNC KESİM NO", v.CncKesimNo),
                               ("CNC ROUTER NO", v.CncRouterNo),
                               ("SU JETİ NO", v.SuJetiNo)
                           }));
                    });

                    col.Item().Height(Cm(0.50f))
                       .Border(1)
                       .AlignLeft()
                       .AlignMiddle()
                       .Text("ROTA : ");
                });
        }

        private static void Blok(IContainer c, (string, string)[] data)
        {
            c.Border(0.5f).Padding(2).Table(t =>
            {
                t.ColumnsDefinition(cd =>
                {
                    cd.ConstantColumn(Cm(2.2f));
                    cd.RelativeColumn();
                });

                foreach (var x in data)
                {
                    t.Cell().BorderBottom(0.5f).Text(x.Item1).Bold().FontSize(8);
                    t.Cell().BorderBottom(0.5f).Text(": " + (x.Item2 ?? "")).FontSize(8);
                }
            });
        }

        /* ============================================================
           7) BÖLÜM 3 — TEKNİK RESİM + BİLEŞME
        ============================================================ */
        private static void Bolum3_TeknikVeBilesme(IContainer container, IsEmriPdfModel v)
        {
            container.BorderLeft(1).BorderRight(1).BorderBottom(1).BorderTop(1).Row(row =>
            {
                row.ConstantItem(Cm(16.53f)).Element(e => e.AlignTop())
                .Column(sol =>
                {
                    sol.Spacing(0);

                    sol.Item().Height(Cm(0.56f))
                   .AlignCenter()
                   .AlignMiddle()
                   .Text("TEKNİK RESİM").Bold();
                    // TEKNİK RESİM
                    sol.Item().Height(Cm(10.9f)).Border(0.5f).Element(img =>
                    {
                        var b = ResimIndir(v.ResimUrl);
                        if (b != null) img.Image(b).FitArea();
                        else img.AlignCenter().AlignMiddle().Text("RESİM YOK");
                    });

                    // NOTLAR
                    sol.Item().Height(Cm(2f))
                        .Border(1f)
                        .Text(t =>
                        {
                            t.Span("NOTLAR: ").Bold();
                            t.Span(v.Isemrinot ?? "");
                        });

                    // TOLERANS / LOGO / ONAY
                    sol.Item().Height(Cm(3.17f)).Table(t =>
                    {
                        t.ColumnsDefinition(cd =>
                        {
                            cd.ConstantColumn(Cm(4.93f));
                            cd.ConstantColumn(Cm(7.25f));
                            cd.ConstantColumn(Cm(4.35f));
                        });

                        // Tolerans
                        t.Cell().Border(0.5f).Column(c =>
                        {
                            c.Item().Text("TOLERANSLAR").Bold();
                            c.Item().Text("ÖLÇÜ: -2/+0 mm");
                            c.Item().Text("OFFSET: -0/+2 mm");
                            c.Item().Text("KAVİS: ±3 mm");
                            c.Item().Text("BOYA: -3/+5 mm");
                            c.Item().Text("CAM: ±5%");
                        });

                        // Logo
                        t.Cell().Border(1f).AlignLeft().AlignTop().Border(0.5f).Text("  LOGO  ").Bold();

                        // Onay
                        t.Cell().Border(1f).AlignCenter().AlignTop().BorderBottom(0.5f).Text("ONAY").Bold();
                    });
                });

                // SAĞ BLOK – BİLEŞME
                row.ConstantItem(Cm(13.47f)).Border(1).Column(sag =>
                {
                    sag.Item().AlignCenter().Text("BİLEŞME ŞEKLİ").Bold();

                    sag.Item().Height(Cm(15.56f)).Table(t =>
                    {
                        t.ColumnsDefinition(cd =>
                        {
                            cd.ConstantColumn(Cm(1f)); //sıra
                            cd.RelativeColumn(); //hm kodu
                            cd.RelativeColumn(); //hm adı
                            cd.ConstantColumn(Cm(1.5f)); //renk
                            cd.ConstantColumn(Cm(2f));
                            cd.ConstantColumn(Cm(2f));
                        });

                        // Başlıklar
                        void B(string s) =>
                            t.Cell().Border(0.5f).Background(Colors.Grey.Lighten3)
                              .AlignCenter().Text(s).Bold();

                        B("SIRA");
                        B("HM KODU");
                        B("HAMMADDE ADI");
                        B("RENK");
                        B("KALINLIK (mm)");
                        B("AÇIKLAMA");

                        

                        int rows = Math.Max(v.ReceteListesi.Count, 24);
                        for (int i = 0; i < rows; i++)
                        {
                            void C(string s, string bgColor = null)
                            {
                                var cell = t.Cell();

                                cell.Element(e =>
                                {
                                    if (!string.IsNullOrEmpty(bgColor))
                                        e = e.Background(bgColor);

                                    e = e.Border(0.5f).PaddingVertical(2).AlignCenter();
                                    e.Text(s);
                                    return e;
                                });

                            }

                            if (i < v.ReceteListesi.Count)
                            {

                                var r = v.ReceteListesi[i];

                                // 🔥 1) Tip H01.001 ise satır komple soft yeşil
                                string rowColor = r.Tip.Trim() == "H01.001" ? "#D0F2CF" : null;

                                // 🔥 2) Açıklama hücresinde 'logo' geçiyorsa sadece o hücre sarı
                                string aciklamaColor =
                                    (!string.IsNullOrEmpty(r.Aciklama) && r.Aciklama.ToLower().Contains("logo"))
                                    ? "#FFF2A8"
                                    : rowColor;

                                C((i + 1).ToString(), rowColor);
                                C(r.HammaddeKodu, rowColor);
                                C(r.HammaddeAdi, rowColor);
                                C(r.Tip, rowColor);
                                C(r.Kalinlik, rowColor);
                                C(r.Aciklama, aciklamaColor); // özel durum burada!
                            }
                            else
                            {
                                C(i.ToString()); 
                                C(""); 
                                C(""); 
                                C(""); 
                                C(""); 
                                C("");
                                //C("TEST", "#FF0000");
                            }
                        }

                       
                    });

                    sag.Item().AlignLeft().AlignBottom().Text("TOPLAM CAM KALINLIĞI: " + v.CamKalinlik).Bold();
                });


            });
        }

        /* ============================================================
           8) BÖLÜM 4 — CAM KALINLIK
        ============================================================ */
        private static void Bolum4_CamKalinlik(IContainer container, IsEmriPdfModel v)
        {
            //container.Height(Cm(0.45f))
            //    .AlignRight()
            //    .AlignMiddle()
            //    .Text("TOPLAM CAM KALINLIĞI: " + v.CamKalinlik)
            //    .Bold();
        }

        /* ============================================================
           9) RESİM İNDİRME
        ============================================================ */
        private static byte[] ResimIndir(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return null;
                using var c = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };
                return c.GetByteArrayAsync(url).Result;
            }
            catch
            {
                return null;
            }
        }
    }
}
