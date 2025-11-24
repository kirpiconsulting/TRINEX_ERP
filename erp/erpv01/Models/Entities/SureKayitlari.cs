using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class SureKayitlari
{
    public int Id { get; set; }

    public string EvrakNo { get; set; } = null!;

    public DateTime Tarih { get; set; }

    public decimal Sure { get; set; }

    public string? IsEmriNo { get; set; }

    public string? OperasyonKod { get; set; }

    public int? KayitTipi { get; set; }

    public int? IslemKodu { get; set; }

    public int? Aksiyon { get; set; }

    public string? StokKod { get; set; }

    public string? LotNo { get; set; }

    public string? SeriNo { get; set; }

    public string? IsMerkeziKod { get; set; }

    public string? HedefIsMerkezi { get; set; }

    public string? HedefOperator { get; set; }

    public DateTime? KayitBaslangic { get; set; }

    public DateTime? KayitBitis { get; set; }

    public int? Vardiya { get; set; }

    public int? DurmaSebebi { get; set; }

    public string? Notlar { get; set; }

    public decimal? AyarSuresi { get; set; }

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

    public bool AktifMi { get; set; }

    public DateTime OlusturmaTarihi { get; set; }

    public string? OlusturanKullanici { get; set; }

    public DateTime? GuncellemeTarihi { get; set; }

    public string? GuncelleyenKullanici { get; set; }

    public int IsEmriKalemKodu { get; set; }
}
