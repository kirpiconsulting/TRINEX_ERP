using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using erpv01.Models.Entities;

namespace erpv01.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cariler> Carilers { get; set; }

    public virtual DbSet<Depolar> Depolars { get; set; }

    public virtual DbSet<EntegrasyonTanimlari> EntegrasyonTanimlaris { get; set; }

    public virtual DbSet<IsEmirleri> IsEmirleris { get; set; }

    public virtual DbSet<IsEmriKalemleri> IsEmriKalemleris { get; set; }

    public virtual DbSet<LotSeriTanimlari> LotSeriTanimlaris { get; set; }

    public virtual DbSet<Operasyonlar> Operasyonlars { get; set; }

    public virtual DbSet<Operatorler> Operatorlers { get; set; }

    public virtual DbSet<ReceteKalemleri> ReceteKalemleris { get; set; }

    public virtual DbSet<Receteler> Recetelers { get; set; }

    public virtual DbSet<SatinalmaSiparisKalemleri> SatinalmaSiparisKalemleris { get; set; }

    public virtual DbSet<SatinalmaSiparisleri> SatinalmaSiparisleris { get; set; }

    public virtual DbSet<SatisSiparisKalemleri> SatisSiparisKalemleris { get; set; }

    public virtual DbSet<SatisSiparisleri> SatisSiparisleris { get; set; }

    public virtual DbSet<StokMiktarlari> StokMiktarlaris { get; set; }

    public virtual DbSet<Stoklar> Stoklars { get; set; }

    public virtual DbSet<SureKayitlari> SureKayitlaris { get; set; }

    public virtual DbSet<SureKayitlariKalemleri> SureKayitlariKalemleris { get; set; }

    public virtual DbSet<SureKayitlariOperatorler> SureKayitlariOperatorlers { get; set; }

    public virtual DbSet<SysAlanAciklamalari> SysAlanAciklamalaris { get; set; }

    public virtual DbSet<SysAlanAciklamalariKalemleri> SysAlanAciklamalariKalemleris { get; set; }

    public virtual DbSet<Tezgahlar> Tezgahlars { get; set; }

    public virtual DbSet<UretimFisiKalemleri> UretimFisiKalemleris { get; set; }

    public virtual DbSet<UretimFisiTuketimler> UretimFisiTuketimlers { get; set; }

    public virtual DbSet<UretimFisleri> UretimFisleris { get; set; }

    public virtual DbSet<VeriSetiKalemleri> VeriSetiKalemleris { get; set; }

    public virtual DbSet<VeriSetleri> VeriSetleris { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=89.252.181.138\\MSSQLSERVER2022;Database=hednovae_DB_TRINEX;User Id=hednovae_trinex;Password=F~8Fvwq1Pwntoo%4;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("hednovae_trinex");

        modelBuilder.Entity<Cariler>(entity =>
        {
            entity.ToTable("cariler", "dbo");

            entity.HasIndex(e => e.Kod, "UQ_cariler_kod").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcikHesapLimiti)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("acik_hesap_limiti");
            entity.Property(e => e.Ad)
                .HasMaxLength(250)
                .HasColumnName("ad");
            entity.Property(e => e.Ad2)
                .HasMaxLength(250)
                .HasColumnName("ad2");
            entity.Property(e => e.Adres1)
                .HasMaxLength(100)
                .HasColumnName("adres_1");
            entity.Property(e => e.Adres2)
                .HasMaxLength(100)
                .HasColumnName("adres_2");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.CariTipi)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cari_tipi");
            entity.Property(e => e.EPosta)
                .HasMaxLength(100)
                .HasColumnName("e_posta");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.Faks)
                .HasMaxLength(50)
                .HasColumnName("faks");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.Kod)
                .HasMaxLength(100)
                .HasColumnName("kod");
            entity.Property(e => e.MuhasebeKodu)
                .HasMaxLength(100)
                .HasColumnName("muhasebe_kodu");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.PostaKodu)
                .HasMaxLength(20)
                .HasColumnName("posta_kodu");
            entity.Property(e => e.RiskLimiti)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("risk_limiti");
            entity.Property(e => e.Sehir)
                .HasMaxLength(50)
                .HasColumnName("sehir");
            entity.Property(e => e.Tckn)
                .HasMaxLength(20)
                .HasColumnName("tckn");
            entity.Property(e => e.Telefon)
                .HasMaxLength(50)
                .HasColumnName("telefon");
            entity.Property(e => e.UlkeKodu)
                .HasMaxLength(10)
                .HasColumnName("ulke_kodu");
            entity.Property(e => e.VarsayilanVadeGun)
                .HasDefaultValue((short)0)
                .HasColumnName("varsayilan_vade_gun");
            entity.Property(e => e.VergiDairesi)
                .HasMaxLength(100)
                .HasColumnName("vergi_dairesi");
            entity.Property(e => e.VergiKimlikNo)
                .HasMaxLength(20)
                .HasColumnName("vergi_kimlik_no");
            entity.Property(e => e.WebSitesi)
                .HasMaxLength(100)
                .HasColumnName("web_sitesi");
        });

        modelBuilder.Entity<Depolar>(entity =>
        {
            entity.ToTable("depolar", "dbo");

            entity.HasIndex(e => e.Kod, "UQ_depolar_kod").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ad)
                .HasMaxLength(250)
                .HasColumnName("ad");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.Kod)
                .HasMaxLength(100)
                .HasColumnName("kod");
            entity.Property(e => e.MuhasebeKodu)
                .HasMaxLength(100)
                .HasColumnName("muhasebe_kodu");
            entity.Property(e => e.MuhasebeKodu2)
                .HasMaxLength(100)
                .HasColumnName("muhasebe_kodu_2");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.TesisKodu)
                .HasMaxLength(100)
                .HasColumnName("tesis_kodu");
        });

        modelBuilder.Entity<EntegrasyonTanimlari>(entity =>
        {
            entity.ToTable("entegrasyon_tanimlari", "dbo");

            entity.HasIndex(e => e.Kod, "UQ_entegrasyon_tanimlari_kod").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aciklama)
                .HasMaxLength(250)
                .HasColumnName("aciklama");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.Deger1).HasColumnName("deger_1");
            entity.Property(e => e.Deger10).HasColumnName("deger_10");
            entity.Property(e => e.Deger2).HasColumnName("deger_2");
            entity.Property(e => e.Deger3).HasColumnName("deger_3");
            entity.Property(e => e.Deger4).HasColumnName("deger_4");
            entity.Property(e => e.Deger5).HasColumnName("deger_5");
            entity.Property(e => e.Deger6).HasColumnName("deger_6");
            entity.Property(e => e.Deger7).HasColumnName("deger_7");
            entity.Property(e => e.Deger8).HasColumnName("deger_8");
            entity.Property(e => e.Deger9).HasColumnName("deger_9");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.Kod)
                .HasMaxLength(100)
                .HasColumnName("kod");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
        });

        modelBuilder.Entity<IsEmirleri>(entity =>
        {
            entity.ToTable("is_emirleri", "dbo");

            entity.HasIndex(e => e.EvrakNo, "UQ_is_emirleri_evrak_no").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcikKapali)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength()
                .HasColumnName("acik_kapali");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.BakiyeMiktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("bakiye_miktar");
            entity.Property(e => e.CariKod)
                .HasMaxLength(100)
                .HasColumnName("cari_kod");
            entity.Property(e => e.CariSiparisEvrakno)
                .HasMaxLength(100)
                .HasColumnName("cari_siparis_evrakno");
            entity.Property(e => e.CariSiparisKalemKodu)
                .HasMaxLength(100)
                .HasColumnName("cari_siparis_kalem_kodu");
            entity.Property(e => e.CariTeslimTarihi).HasColumnName("cari_teslim_tarihi");
            entity.Property(e => e.Durum).HasColumnName("durum");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.FireMiktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("fire_miktar");
            entity.Property(e => e.GecerlilikTarihi).HasColumnName("gecerlilik_tarihi");
            entity.Property(e => e.GrupEvrakNo)
                .HasMaxLength(100)
                .HasColumnName("grup_evrak_no");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.KapanisTarihi).HasColumnName("kapanis_tarihi");
            entity.Property(e => e.KaynakIsEmriEvrakno)
                .HasMaxLength(100)
                .HasColumnName("kaynak_is_emri_evrakno");
            entity.Property(e => e.Notlar)
                .HasMaxLength(250)
                .HasColumnName("notlar");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.Oncelik)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("oncelik");
            entity.Property(e => e.PlanlananMiktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("planlanan_miktar");
            entity.Property(e => e.RevizeTeslimTarihi).HasColumnName("revize_teslim_tarihi");
            entity.Property(e => e.StokBirim)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("stok_birim");
            entity.Property(e => e.StokKod)
                .HasMaxLength(100)
                .HasColumnName("stok_kod");
            entity.Property(e => e.Tarih).HasColumnName("tarih");
            entity.Property(e => e.UretilenMiktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("uretilen_miktar");
            entity.Property(e => e.UretimPlani)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("uretim_plani");
            entity.Property(e => e.UretimdenTeslimTarihi).HasColumnName("uretimden_teslim_tarihi");
        });

        modelBuilder.Entity<IsEmriKalemleri>(entity =>
        {
            entity.ToTable("is_emri_kalemleri", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.BaslangicTarihi).HasColumnName("baslangic_tarihi");
            entity.Property(e => e.BitisTarihi).HasColumnName("bitis_tarihi");
            entity.Property(e => e.DemontajMi).HasColumnName("demontaj_mi");
            entity.Property(e => e.Durum).HasColumnName("durum");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.GerceklesenBaslama).HasColumnName("gerceklesen_baslama");
            entity.Property(e => e.GerceklesenBitis).HasColumnName("gerceklesen_bitis");
            entity.Property(e => e.GerceklesenMiktar)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("gerceklesen_miktar");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.HazirlikSuresi)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("hazirlik_suresi");
            entity.Property(e => e.IslemSuresi)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("islem_suresi");
            entity.Property(e => e.KalanMiktar)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("kalan_miktar");
            entity.Property(e => e.KalemKodu).HasColumnName("kalem_kodu");
            entity.Property(e => e.KalipKodu)
                .HasMaxLength(100)
                .HasColumnName("kalip_kodu");
            entity.Property(e => e.KayipMiktari)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("kayip_miktari");
            entity.Property(e => e.KayipYuzdesi)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("kayip_yuzdesi");
            entity.Property(e => e.KaynakKod)
                .HasMaxLength(100)
                .HasColumnName("kaynak_kod");
            entity.Property(e => e.KaynakTipi).HasColumnName("kaynak_tipi");
            entity.Property(e => e.MusteriKodu)
                .HasMaxLength(100)
                .HasColumnName("musteri_kodu");
            entity.Property(e => e.Notlar)
                .HasMaxLength(250)
                .HasColumnName("notlar");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.Oncelik).HasColumnName("oncelik");
            entity.Property(e => e.OperasyonKod)
                .HasMaxLength(100)
                .HasColumnName("operasyon_kod");
            entity.Property(e => e.OperatörSayisi)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("operatör_sayisi");
            entity.Property(e => e.PlanlananMiktar)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("planlanan_miktar");
            entity.Property(e => e.SiraNumarasi).HasColumnName("sira_numarasi");
            entity.Property(e => e.TransferPartiBuyuklugu)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("transfer_parti_buyuklugu");
        });

        modelBuilder.Entity<LotSeriTanimlari>(entity =>
        {
            entity.ToTable("lot_seri_tanimlari", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.DepoKod)
                .HasMaxLength(100)
                .HasColumnName("depo_kod");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.Kod)
                .HasMaxLength(100)
                .HasColumnName("kod");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.StokBirim)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("stok_birim");
            entity.Property(e => e.StokKod)
                .HasMaxLength(100)
                .HasColumnName("stok_kod");
            entity.Property(e => e.StokMiktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("stok_miktar");
            entity.Property(e => e.Tarih1).HasColumnName("tarih_1");
            entity.Property(e => e.Tarih10).HasColumnName("tarih_10");
            entity.Property(e => e.Tarih2).HasColumnName("tarih_2");
            entity.Property(e => e.Tarih3).HasColumnName("tarih_3");
            entity.Property(e => e.Tarih4).HasColumnName("tarih_4");
            entity.Property(e => e.Tarih5).HasColumnName("tarih_5");
            entity.Property(e => e.Tarih6).HasColumnName("tarih_6");
            entity.Property(e => e.Tarih7).HasColumnName("tarih_7");
            entity.Property(e => e.Tarih8).HasColumnName("tarih_8");
            entity.Property(e => e.Tarih9).HasColumnName("tarih_9");
            entity.Property(e => e.Tip)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tip");
        });

        modelBuilder.Entity<Operasyonlar>(entity =>
        {
            entity.ToTable("operasyonlar", "dbo");

            entity.HasIndex(e => e.Kod, "UQ_operasyonlar_kod").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ad)
                .HasMaxLength(250)
                .HasColumnName("ad");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.Kod)
                .HasMaxLength(100)
                .HasColumnName("kod");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.Tip).HasColumnName("tip");
        });

        modelBuilder.Entity<Operatorler>(entity =>
        {
            entity.ToTable("operatorler", "dbo");

            entity.HasIndex(e => e.Kod, "UQ_operatorler_kod").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ad)
                .HasMaxLength(100)
                .HasColumnName("ad");
            entity.Property(e => e.Adres)
                .HasMaxLength(250)
                .HasColumnName("adres");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.CalisabilecegiTezgahlar).HasColumnName("calisabilecegi_tezgahlar");
            entity.Property(e => e.EPosta)
                .HasMaxLength(100)
                .HasColumnName("e_posta");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.Faks)
                .HasMaxLength(50)
                .HasColumnName("faks");
            entity.Property(e => e.GsmNo)
                .HasMaxLength(50)
                .HasColumnName("gsm_no");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.IlgiliCariKod)
                .HasMaxLength(100)
                .HasColumnName("ilgili_cari_kod");
            entity.Property(e => e.Kod)
                .HasMaxLength(100)
                .HasColumnName("kod");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.PostaKodu)
                .HasMaxLength(20)
                .HasColumnName("posta_kodu");
            entity.Property(e => e.Sehir)
                .HasMaxLength(50)
                .HasColumnName("sehir");
            entity.Property(e => e.Telefon)
                .HasMaxLength(50)
                .HasColumnName("telefon");
            entity.Property(e => e.Turu).HasColumnName("turu");
        });

        modelBuilder.Entity<ReceteKalemleri>(entity =>
        {
            entity.ToTable("recete_kalemleri", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.DemontajMi).HasColumnName("demontaj_mi");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.KalemKodu)
                .HasMaxLength(100)
                .HasColumnName("kalem_kodu");
            entity.Property(e => e.KalipKodu)
                .HasMaxLength(100)
                .HasColumnName("kalip_kodu");
            entity.Property(e => e.KayipYuzdesi)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("kayip_yuzdesi");
            entity.Property(e => e.KaynakKod)
                .HasMaxLength(100)
                .HasColumnName("kaynak_kod");
            entity.Property(e => e.KaynakTipi).HasColumnName("kaynak_tipi");
            entity.Property(e => e.KullanimMiktar)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("kullanim_miktar");
            entity.Property(e => e.Notlar)
                .HasMaxLength(250)
                .HasColumnName("notlar");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.OperasyonKod)
                .HasMaxLength(100)
                .HasColumnName("operasyon_kod");
            entity.Property(e => e.OperatörSayisi)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("operatör_sayisi");
            entity.Property(e => e.SiraNumarasi).HasColumnName("sira_numarasi");
            entity.Property(e => e.TransferPartiBuyuklugu)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("transfer_parti_buyuklugu");
        });

        modelBuilder.Entity<Receteler>(entity =>
        {
            entity.ToTable("receteler", "dbo");

            entity.HasIndex(e => e.EvrakNo, "UQ_receteler_evrak_no").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aciklama)
                .HasMaxLength(250)
                .HasColumnName("aciklama");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.DemontajMi).HasColumnName("demontaj_mi");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.StokBirim)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("stok_birim");
            entity.Property(e => e.StokKod)
                .HasMaxLength(100)
                .HasColumnName("stok_kod");
            entity.Property(e => e.StokMiktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("stok_miktar");
        });

        modelBuilder.Entity<SatinalmaSiparisKalemleri>(entity =>
        {
            entity.ToTable("satinalma_siparis_kalemleri", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcikKapaliDurum)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("acik_kapali_durum");
            entity.Property(e => e.BirimFiyat)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("birim_fiyat");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.IskontoOrani)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("iskonto_orani");
            entity.Property(e => e.IskontoTutar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("iskonto_tutar");
            entity.Property(e => e.KalemKodu)
                .HasMaxLength(100)
                .HasColumnName("kalem_kodu");
            entity.Property(e => e.KdvOrani)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("kdv_orani");
            entity.Property(e => e.KdvTutar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("kdv_tutar");
            entity.Property(e => e.Miktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("miktar");
            entity.Property(e => e.Notlar)
                .HasMaxLength(250)
                .HasColumnName("notlar");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.SiraNumarasi).HasColumnName("sira_numarasi");
            entity.Property(e => e.StokBirim)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("stok_birim");
            entity.Property(e => e.StokKod)
                .HasMaxLength(100)
                .HasColumnName("stok_kod");
            entity.Property(e => e.TeslimTarihi).HasColumnName("teslim_tarihi");
            entity.Property(e => e.ToplamTutar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("toplam_tutar");
        });

        modelBuilder.Entity<SatinalmaSiparisleri>(entity =>
        {
            entity.ToTable("satinalma_siparisleri", "dbo");

            entity.HasIndex(e => e.EvrakNo, "UQ_satinalma_siparisleri_evrak_no").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcikKapali)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("acik_kapali");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.CariKod)
                .HasMaxLength(100)
                .HasColumnName("cari_kod");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.FiyatListesi)
                .HasMaxLength(100)
                .HasColumnName("fiyat_listesi");
            entity.Property(e => e.FiyatSekli)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("fiyat_sekli");
            entity.Property(e => e.GenelToplam)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("genel_toplam");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.IkinciCariKod)
                .HasMaxLength(100)
                .HasColumnName("ikinci_cari_kod");
            entity.Property(e => e.IskontoToplami)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("iskonto_toplami");
            entity.Property(e => e.IslemSaati).HasColumnName("islem_saati");
            entity.Property(e => e.KdvToplami)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("kdv_toplami");
            entity.Property(e => e.NakliyeKosullari)
                .HasMaxLength(70)
                .HasColumnName("nakliye_kosullari");
            entity.Property(e => e.NakliyeciFirma)
                .HasMaxLength(70)
                .HasColumnName("nakliyeci_firma");
            entity.Property(e => e.OdemeGunu)
                .HasDefaultValue((short)0)
                .HasColumnName("odeme_gunu");
            entity.Property(e => e.OdemeKosullari)
                .HasMaxLength(150)
                .HasColumnName("odeme_kosullari");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.ParaBirimi)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("para_birimi");
            entity.Property(e => e.SevkAdresi)
                .HasMaxLength(250)
                .HasColumnName("sevk_adresi");
            entity.Property(e => e.SevkCariKod)
                .HasMaxLength(50)
                .HasColumnName("sevk_cari_kod");
            entity.Property(e => e.SiparisVeren)
                .HasMaxLength(100)
                .HasColumnName("siparis_veren");
            entity.Property(e => e.Tarih).HasColumnName("tarih");
            entity.Property(e => e.ToplamTutar)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("toplam_tutar");
        });

        modelBuilder.Entity<SatisSiparisKalemleri>(entity =>
        {
            entity.ToTable("satis_siparis_kalemleri", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcikKapaliDurum)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("acik_kapali_durum");
            entity.Property(e => e.BirimFiyat)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("birim_fiyat");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.IskontoOrani)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("iskonto_orani");
            entity.Property(e => e.IskontoTutar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("iskonto_tutar");
            entity.Property(e => e.KalemKodu)
                .HasMaxLength(100)
                .HasColumnName("kalem_kodu");
            entity.Property(e => e.KdvOrani)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("kdv_orani");
            entity.Property(e => e.KdvTutar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("kdv_tutar");
            entity.Property(e => e.Miktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("miktar");
            entity.Property(e => e.Notlar)
                .HasMaxLength(250)
                .HasColumnName("notlar");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.SiraNumarasi).HasColumnName("sira_numarasi");
            entity.Property(e => e.StokBirim)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("stok_birim");
            entity.Property(e => e.StokKod)
                .HasMaxLength(100)
                .HasColumnName("stok_kod");
            entity.Property(e => e.TeslimTarihi).HasColumnName("teslim_tarihi");
            entity.Property(e => e.ToplamTutar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("toplam_tutar");
        });

        modelBuilder.Entity<SatisSiparisleri>(entity =>
        {
            entity.ToTable("satis_siparisleri", "dbo");

            entity.HasIndex(e => e.EvrakNo, "UQ_satis_siparisleri_evrak_no").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcikKapali)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("acik_kapali");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.CariKod)
                .HasMaxLength(100)
                .HasColumnName("cari_kod");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.FiyatListesi)
                .HasMaxLength(100)
                .HasColumnName("fiyat_listesi");
            entity.Property(e => e.FiyatSekli)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("fiyat_sekli");
            entity.Property(e => e.GenelToplam)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("genel_toplam");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.IkinciCariKod)
                .HasMaxLength(100)
                .HasColumnName("ikinci_cari_kod");
            entity.Property(e => e.IskontoToplami)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("iskonto_toplami");
            entity.Property(e => e.IslemSaati).HasColumnName("islem_saati");
            entity.Property(e => e.KdvToplami)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("kdv_toplami");
            entity.Property(e => e.NakliyeKosullari)
                .HasMaxLength(70)
                .HasColumnName("nakliye_kosullari");
            entity.Property(e => e.NakliyeciFirma)
                .HasMaxLength(70)
                .HasColumnName("nakliyeci_firma");
            entity.Property(e => e.OdemeGunu)
                .HasDefaultValue((short)0)
                .HasColumnName("odeme_gunu");
            entity.Property(e => e.OdemeKosullari)
                .HasMaxLength(150)
                .HasColumnName("odeme_kosullari");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.ParaBirimi)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("para_birimi");
            entity.Property(e => e.SevkAdresi)
                .HasMaxLength(250)
                .HasColumnName("sevk_adresi");
            entity.Property(e => e.SevkCariKod)
                .HasMaxLength(50)
                .HasColumnName("sevk_cari_kod");
            entity.Property(e => e.SiparisVeren)
                .HasMaxLength(100)
                .HasColumnName("siparis_veren");
            entity.Property(e => e.Tarih).HasColumnName("tarih");
            entity.Property(e => e.ToplamTutar)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("toplam_tutar");
        });

        modelBuilder.Entity<StokMiktarlari>(entity =>
        {
            entity.ToTable("stok_miktarlari", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AktifMi).HasColumnName("aktif_mi");
            entity.Property(e => e.DepoKod)
                .HasMaxLength(100)
                .HasColumnName("depo_kod");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi).HasColumnName("olusturma_tarihi");
            entity.Property(e => e.StokBirim)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("stok_birim");
            entity.Property(e => e.StokKod)
                .HasMaxLength(100)
                .HasColumnName("stok_kod");
            entity.Property(e => e.StokMiktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("stok_miktar");
            entity.Property(e => e.Tarih1).HasColumnName("tarih_1");
            entity.Property(e => e.Tarih10).HasColumnName("tarih_10");
            entity.Property(e => e.Tarih2).HasColumnName("tarih_2");
            entity.Property(e => e.Tarih3).HasColumnName("tarih_3");
            entity.Property(e => e.Tarih4).HasColumnName("tarih_4");
            entity.Property(e => e.Tarih5).HasColumnName("tarih_5");
            entity.Property(e => e.Tarih6).HasColumnName("tarih_6");
            entity.Property(e => e.Tarih7).HasColumnName("tarih_7");
            entity.Property(e => e.Tarih8).HasColumnName("tarih_8");
            entity.Property(e => e.Tarih9).HasColumnName("tarih_9");
        });

        modelBuilder.Entity<Stoklar>(entity =>
        {
            entity.ToTable("stoklar", "dbo");

            entity.HasIndex(e => e.Kod, "UQ_stoklar_kod").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ad)
                .HasMaxLength(250)
                .HasColumnName("ad");
            entity.Property(e => e.Ad2)
                .HasMaxLength(250)
                .HasColumnName("ad2");
            entity.Property(e => e.Agirlik)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("agirlik");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.AlisFiyati)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("alis_fiyati");
            entity.Property(e => e.AnaBirim)
                .HasMaxLength(10)
                .HasColumnName("ana_birim");
            entity.Property(e => e.Barkod)
                .HasMaxLength(50)
                .HasColumnName("barkod");
            entity.Property(e => e.Boy)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("boy");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.En)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("en");
            entity.Property(e => e.GenelAciklama).HasColumnName("genel_aciklama");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.Hacim)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("hacim");
            entity.Property(e => e.KdvOrani)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("kdv_orani");
            entity.Property(e => e.Kod)
                .HasMaxLength(100)
                .HasColumnName("kod");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.SatisFiyati1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("satis_fiyati_1");
            entity.Property(e => e.StokTuru).HasColumnName("stok_turu");
            entity.Property(e => e.TedarikciKodu)
                .HasMaxLength(50)
                .HasColumnName("tedarikci_kodu");
            entity.Property(e => e.TeknikAciklama)
                .HasMaxLength(500)
                .HasColumnName("teknik_aciklama");
            entity.Property(e => e.Yukseklik)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("yukseklik");
        });

        modelBuilder.Entity<SureKayitlari>(entity =>
        {
            entity.ToTable("sure_kayitlari", "dbo");

            entity.HasIndex(e => e.EvrakNo, "UQ_sure_kayitlari_evrak_no").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aksiyon).HasColumnName("aksiyon");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.AyarSuresi)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("ayar_suresi");
            entity.Property(e => e.DurmaSebebi).HasColumnName("durma_sebebi");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.HedefIsMerkezi)
                .HasMaxLength(100)
                .HasColumnName("hedef_is_merkezi");
            entity.Property(e => e.HedefOperator)
                .HasMaxLength(100)
                .HasColumnName("hedef_operator");
            entity.Property(e => e.IsEmriKalemKodu).HasColumnName("is_emri_kalem_kodu");
            entity.Property(e => e.IsEmriNo)
                .HasMaxLength(100)
                .HasColumnName("is_emri_no");
            entity.Property(e => e.IsMerkeziKod)
                .HasMaxLength(100)
                .HasColumnName("is_merkezi_kod");
            entity.Property(e => e.IslemKodu).HasColumnName("islem_kodu");
            entity.Property(e => e.KayitBaslangic).HasColumnName("kayit_baslangic");
            entity.Property(e => e.KayitBitis).HasColumnName("kayit_bitis");
            entity.Property(e => e.KayitTipi).HasColumnName("kayit_tipi");
            entity.Property(e => e.LotNo)
                .HasMaxLength(100)
                .HasColumnName("lot_no");
            entity.Property(e => e.Notlar)
                .HasMaxLength(250)
                .HasColumnName("notlar");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.OperasyonKod)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("operasyon_kod");
            entity.Property(e => e.SeriNo)
                .HasMaxLength(100)
                .HasColumnName("seri_no");
            entity.Property(e => e.StokKod)
                .HasMaxLength(100)
                .HasColumnName("stok_kod");
            entity.Property(e => e.Sure)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("sure");
            entity.Property(e => e.Tarih).HasColumnName("tarih");
            entity.Property(e => e.Vardiya).HasColumnName("vardiya");
        });

        modelBuilder.Entity<SureKayitlariKalemleri>(entity =>
        {
            entity.ToTable("sure_kayitlari_kalemleri", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.FireDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("fire_deposu_kodu");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.IsMerkeziKod)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("is_merkezi_kod");
            entity.Property(e => e.KalemKodu).HasColumnName("kalem_kodu");
            entity.Property(e => e.KalipKodu)
                .HasMaxLength(100)
                .HasColumnName("kalip_kodu");
            entity.Property(e => e.Konum1)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("konum_1");
            entity.Property(e => e.LotNo)
                .HasMaxLength(100)
                .HasColumnName("lot_no");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.ReceteKod)
                .HasMaxLength(100)
                .HasColumnName("recete_kod");
            entity.Property(e => e.SeriNo)
                .HasMaxLength(100)
                .HasColumnName("seri_no");
            entity.Property(e => e.SiraNumarasi).HasColumnName("sira_numarasi");
            entity.Property(e => e.StokBirim)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("stok_birim");
            entity.Property(e => e.StokKod)
                .HasMaxLength(100)
                .HasColumnName("stok_kod");
            entity.Property(e => e.StokMiktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("stok_miktar");
            entity.Property(e => e.Tarih).HasColumnName("tarih");
            entity.Property(e => e.TuketimDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tuketim_deposu_kodu");
            entity.Property(e => e.UretimDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("uretim_deposu_kodu");
            entity.Property(e => e.YanUrunDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("yan_urun_deposu_kodu");
            entity.Property(e => e.YarimamulKod)
                .HasMaxLength(100)
                .HasColumnName("yarimamul_kod");
        });

        modelBuilder.Entity<SureKayitlariOperatorler>(entity =>
        {
            entity.ToTable("sure_kayitlari_operatorler", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.IsEmriNo)
                .HasMaxLength(100)
                .HasColumnName("is_emri_no");
            entity.Property(e => e.JobNo)
                .HasMaxLength(100)
                .HasColumnName("job_no");
            entity.Property(e => e.KalemKodu).HasColumnName("kalem_kodu");
            entity.Property(e => e.Notlar)
                .HasMaxLength(250)
                .HasColumnName("notlar");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.OperatorKod)
                .HasMaxLength(100)
                .HasColumnName("operator_kod");
            entity.Property(e => e.SiraNumarasi).HasColumnName("sira_numarasi");
            entity.Property(e => e.Tarih).HasColumnName("tarih");
            entity.Property(e => e.ToplamDakika)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("toplam_dakika");
            entity.Property(e => e.ToplamSure)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("toplam_sure");
        });

        modelBuilder.Entity<SysAlanAciklamalari>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sys_alan__3213E83FBCC24A34");

            entity.ToTable("sys_alan_aciklamalari", "dbo");

            entity.HasIndex(e => e.Evrakno, "UX_sys_alan_aciklamalari_evrakno").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aciklama1)
                .HasMaxLength(250)
                .HasColumnName("aciklama1");
            entity.Property(e => e.Aciklama2)
                .HasMaxLength(250)
                .HasColumnName("aciklama2");
            entity.Property(e => e.Aciklama3)
                .HasMaxLength(250)
                .HasColumnName("aciklama3");
            entity.Property(e => e.Aciklama4)
                .HasMaxLength(250)
                .HasColumnName("aciklama4");
            entity.Property(e => e.Aciklama5)
                .HasMaxLength(250)
                .HasColumnName("aciklama5");
            entity.Property(e => e.Evrakno)
                .HasMaxLength(50)
                .HasColumnName("evrakno");
            entity.Property(e => e.GuncellemeTarihi)
                .HasColumnType("datetime")
                .HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(50)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(50)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.ReferansMetin1)
                .HasMaxLength(250)
                .HasColumnName("referans_metin1");
            entity.Property(e => e.ReferansMetin2)
                .HasMaxLength(250)
                .HasColumnName("referans_metin2");
            entity.Property(e => e.ReferansMetin3)
                .HasMaxLength(250)
                .HasColumnName("referans_metin3");
            entity.Property(e => e.ReferansMetin4)
                .HasMaxLength(250)
                .HasColumnName("referans_metin4");
            entity.Property(e => e.ReferansMetin5)
                .HasMaxLength(250)
                .HasColumnName("referans_metin5");
            entity.Property(e => e.ReferansSayi1)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("referans_sayi1");
            entity.Property(e => e.ReferansSayi2)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("referans_sayi2");
            entity.Property(e => e.ReferansSayi3)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("referans_sayi3");
            entity.Property(e => e.ReferansSayi4)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("referans_sayi4");
            entity.Property(e => e.ReferansSayi5)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("referans_sayi5");
            entity.Property(e => e.ReferansTarih1)
                .HasColumnType("datetime")
                .HasColumnName("referans_tarih1");
            entity.Property(e => e.ReferansTarih2)
                .HasColumnType("datetime")
                .HasColumnName("referans_tarih2");
            entity.Property(e => e.ReferansTarih3)
                .HasColumnType("datetime")
                .HasColumnName("referans_tarih3");
            entity.Property(e => e.ReferansTarih4)
                .HasColumnType("datetime")
                .HasColumnName("referans_tarih4");
            entity.Property(e => e.ReferansTarih5)
                .HasColumnType("datetime")
                .HasColumnName("referans_tarih5");
        });

        modelBuilder.Entity<SysAlanAciklamalariKalemleri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__sys_alan__3213E83FCEA0BC3F");

            entity.ToTable("sys_alan_aciklamalari_kalemleri", "dbo");

            entity.HasIndex(e => e.Evrakno, "IX_sys_alan_aciklamalari_kalemleri_evrakno");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aciklama)
                .HasMaxLength(500)
                .HasColumnName("aciklama");
            entity.Property(e => e.Evrakno)
                .HasMaxLength(50)
                .HasColumnName("evrakno");
            entity.Property(e => e.FieldAdi)
                .HasMaxLength(150)
                .HasColumnName("field_adi");
            entity.Property(e => e.GuncellemeTarihi)
                .HasColumnType("datetime")
                .HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(50)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.MaxUzunluk).HasColumnName("max_uzunluk");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(50)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.VarsayilanDeger)
                .HasMaxLength(200)
                .HasColumnName("varsayilan_deger");
            entity.Property(e => e.VeriTuru)
                .HasMaxLength(50)
                .HasColumnName("veri_turu");
            entity.Property(e => e.Zorunlu)
                .HasDefaultValue(false)
                .HasColumnName("zorunlu");
        });

        modelBuilder.Entity<Tezgahlar>(entity =>
        {
            entity.ToTable("tezgahlar", "dbo");

            entity.HasIndex(e => e.Kod, "UQ_tezgahlar_kod").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ad)
                .HasMaxLength(250)
                .HasColumnName("ad");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.Birim)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("birim");
            entity.Property(e => e.Daire)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("daire");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.HariciCariKod)
                .HasMaxLength(100)
                .HasColumnName("harici_cari_kod");
            entity.Property(e => e.IscilikSaati)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("iscilik_saati");
            entity.Property(e => e.Kapasite)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("kapasite");
            entity.Property(e => e.Kod)
                .HasMaxLength(100)
                .HasColumnName("kod");
            entity.Property(e => e.MakineSayisi)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("makine_sayisi");
            entity.Property(e => e.OeeOrani)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("oee_orani");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.OperasyonTipi)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("operasyon_tipi");
            entity.Property(e => e.PlanlananCalismaYuzdesi)
                .HasDefaultValue(100m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("planlanan_calisma_yuzdesi");
            entity.Property(e => e.SetupOperasyonTipi)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("setup_operasyon_tipi");
            entity.Property(e => e.SetupSuresi)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("setup_suresi");
            entity.Property(e => e.TakvimKodu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("takvim_kodu");
            entity.Property(e => e.Tip).HasColumnName("tip");
        });

        modelBuilder.Entity<UretimFisiKalemleri>(entity =>
        {
            entity.ToTable("uretim_fisi_kalemleri", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.Birim)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("birim");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.FireDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("fire_deposu_kodu");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.KalemKodu)
                .HasMaxLength(100)
                .HasColumnName("kalem_kodu");
            entity.Property(e => e.LotNo)
                .HasMaxLength(100)
                .HasColumnName("lot_no");
            entity.Property(e => e.Miktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("miktar");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.SeriNo)
                .HasMaxLength(100)
                .HasColumnName("seri_no");
            entity.Property(e => e.SiraNumarasi).HasColumnName("sira_numarasi");
            entity.Property(e => e.StokKod)
                .HasMaxLength(100)
                .HasColumnName("stok_kod");
            entity.Property(e => e.TuketimDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tuketim_deposu_kodu");
            entity.Property(e => e.UretimDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("uretim_deposu_kodu");
            entity.Property(e => e.UretimLotNo)
                .HasMaxLength(50)
                .HasColumnName("uretim_lot_no");
            entity.Property(e => e.YanUrunDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("yan_urun_deposu_kodu");
        });

        modelBuilder.Entity<UretimFisiTuketimler>(entity =>
        {
            entity.ToTable("uretim_fisi_tuketimler", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.Birim)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("birim");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.FasonNormal)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("fason_normal");
            entity.Property(e => e.FireDagitimdanMi).HasColumnName("fire_dagitimdan_mi");
            entity.Property(e => e.FireDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("fire_deposu_kodu");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.IsMerkeziKod)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("is_merkezi_kod");
            entity.Property(e => e.KalemKodu)
                .HasMaxLength(100)
                .HasColumnName("kalem_kodu");
            entity.Property(e => e.KarsiIsMerkeziKod)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("karsi_is_merkezi_kod");
            entity.Property(e => e.KarsiKonum1)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("karsi_konum_1");
            entity.Property(e => e.KarsiLotNo)
                .HasMaxLength(100)
                .HasColumnName("karsi_lot_no");
            entity.Property(e => e.KarsiMiktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("karsi_miktar");
            entity.Property(e => e.KarsiSeriNo)
                .HasMaxLength(100)
                .HasColumnName("karsi_seri_no");
            entity.Property(e => e.KarsiStokKod)
                .HasMaxLength(100)
                .HasColumnName("karsi_stok_kod");
            entity.Property(e => e.Konum1)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("konum_1");
            entity.Property(e => e.LotNo)
                .HasMaxLength(100)
                .HasColumnName("lot_no");
            entity.Property(e => e.Miktar)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("miktar");
            entity.Property(e => e.Nitelik)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nitelik");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.SeriNo)
                .HasMaxLength(100)
                .HasColumnName("seri_no");
            entity.Property(e => e.SiraNumarasi).HasColumnName("sira_numarasi");
            entity.Property(e => e.StokKod)
                .HasMaxLength(100)
                .HasColumnName("stok_kod");
            entity.Property(e => e.Tarih).HasColumnName("tarih");
            entity.Property(e => e.TuketimDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tuketim_deposu_kodu");
            entity.Property(e => e.UretimDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("uretim_deposu_kodu");
            entity.Property(e => e.YanUrunDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("yan_urun_deposu_kodu");
        });

        modelBuilder.Entity<UretimFisleri>(entity =>
        {
            entity.ToTable("uretim_fisleri", "dbo");

            entity.HasIndex(e => e.EvrakNo, "UQ_uretim_fisleri_evrak_no").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.EvrakNo)
                .HasMaxLength(100)
                .HasColumnName("evrak_no");
            entity.Property(e => e.FasonNormal)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("fason_normal");
            entity.Property(e => e.FireDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("fire_deposu_kodu");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.Nitelik)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("nitelik");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.OnayliMi).HasColumnName("onayli_mi");
            entity.Property(e => e.OzelFlag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ozel_flag");
            entity.Property(e => e.RezervasyonYapildiMi).HasColumnName("rezervasyon_yapildi_mi");
            entity.Property(e => e.Tarih).HasColumnName("tarih");
            entity.Property(e => e.TeslimAlan)
                .HasMaxLength(100)
                .HasColumnName("teslim_alan");
            entity.Property(e => e.TuketimDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tuketim_deposu_kodu");
            entity.Property(e => e.UretimDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("uretim_deposu_kodu");
            entity.Property(e => e.YanUrunDeposuKodu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("yan_urun_deposu_kodu");
        });

        modelBuilder.Entity<VeriSetiKalemleri>(entity =>
        {
            entity.ToTable("veri_seti_kalemleri", "dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ad)
                .HasMaxLength(250)
                .HasColumnName("ad");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.Kod)
                .HasMaxLength(100)
                .HasColumnName("kod");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
            entity.Property(e => e.SiraNumarasi).HasColumnName("sira_numarasi");
            entity.Property(e => e.VeriSetiKod)
                .HasMaxLength(100)
                .HasColumnName("veri_seti_kod");
        });

        modelBuilder.Entity<VeriSetleri>(entity =>
        {
            entity.ToTable("veri_setleri", "dbo");

            entity.HasIndex(e => e.Kod, "UQ_veri_setleri_kod").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ad)
                .HasMaxLength(250)
                .HasColumnName("ad");
            entity.Property(e => e.AktifMi)
                .HasDefaultValue(true)
                .HasColumnName("aktif_mi");
            entity.Property(e => e.EkAlan1).HasColumnName("ek_alan_1");
            entity.Property(e => e.EkAlan10).HasColumnName("ek_alan_10");
            entity.Property(e => e.EkAlan2).HasColumnName("ek_alan_2");
            entity.Property(e => e.EkAlan3).HasColumnName("ek_alan_3");
            entity.Property(e => e.EkAlan4).HasColumnName("ek_alan_4");
            entity.Property(e => e.EkAlan5).HasColumnName("ek_alan_5");
            entity.Property(e => e.EkAlan6).HasColumnName("ek_alan_6");
            entity.Property(e => e.EkAlan7).HasColumnName("ek_alan_7");
            entity.Property(e => e.EkAlan8).HasColumnName("ek_alan_8");
            entity.Property(e => e.EkAlan9).HasColumnName("ek_alan_9");
            entity.Property(e => e.GuncellemeTarihi).HasColumnName("guncelleme_tarihi");
            entity.Property(e => e.GuncelleyenKullanici)
                .HasMaxLength(100)
                .HasColumnName("guncelleyen_kullanici");
            entity.Property(e => e.Kod)
                .HasMaxLength(100)
                .HasColumnName("kod");
            entity.Property(e => e.OlusturanKullanici)
                .HasMaxLength(100)
                .HasColumnName("olusturan_kullanici");
            entity.Property(e => e.OlusturmaTarihi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("olusturma_tarihi");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
