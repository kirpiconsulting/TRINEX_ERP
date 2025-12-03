using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class IsEmriKalemleri
{
    public int Id { get; set; }

    public string EvrakNo { get; set; } = null!;

    public int KalemKodu { get; set; }

    public int SiraNumarasi { get; set; }

    public int? KaynakTipi { get; set; }

    public string KaynakKod { get; set; } = null!;

    public string? OperasyonKod { get; set; }

    public string? KalipKodu { get; set; }

    public decimal PlanlananMiktar { get; set; }

    public decimal GerceklesenMiktar { get; set; }

    public decimal KalanMiktar { get; set; }

    public decimal KayipMiktari { get; set; }

    public decimal KayipYuzdesi { get; set; }

    public DateTime? BaslangicTarihi { get; set; }

    public DateTime? BitisTarihi { get; set; }

    public DateTime? GerceklesenBaslama { get; set; }

    public DateTime? GerceklesenBitis { get; set; }

    public decimal? HazirlikSuresi { get; set; }

    public decimal? IslemSuresi { get; set; }

    public int Durum { get; set; }

    public int Oncelik { get; set; }

    public bool AktifMi { get; set; }

    public string? MusteriKodu { get; set; }

    public decimal? TransferPartiBuyuklugu { get; set; }

    public decimal? OperatörSayisi { get; set; }

    public bool DemontajMi { get; set; }

    public string? Notlar { get; set; }

    public string? EkAlan1 { get; set; }

    public string? EkAlan2 { get; set; }

    public string? EkAlan3 { get; set; }

    public string? EkAlan4 { get; set; }

    public string? EkAlan5 { get; set; }

    public string? EkAlan6 { get; set; }

    public string? EkAlan7 { get; set; }

    public string? EkAlan8 { get; set; }

    public string? EkAlan9 { get; set; }

    public string? EkAlan10 { get; set; }

    public DateTime OlusturmaTarihi { get; set; }

    public string? OlusturanKullanici { get; set; }

    public DateTime? GuncellemeTarihi { get; set; }

    public string? GuncelleyenKullanici { get; set; }
}
