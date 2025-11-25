using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class Stoklar
{
    public int Id { get; set; }

    public string Kod { get; set; } = null!;

    public string Ad { get; set; } = null!;

    public string? Ad2 { get; set; }

    public int? StokTuru { get; set; }

    public string? AnaBirim { get; set; }

    public string? Barkod { get; set; }

    public string? TedarikciKodu { get; set; }

    public decimal? SatisFiyati1 { get; set; }

    public decimal? AlisFiyati { get; set; }

    public decimal? KdvOrani { get; set; }

    public decimal? En { get; set; }

    public decimal? Boy { get; set; }

    public decimal? Yukseklik { get; set; }

    public decimal? Agirlik { get; set; }

    public decimal? Hacim { get; set; }

    public string? TeknikAciklama { get; set; }

    public string? GenelAciklama { get; set; }

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
}
