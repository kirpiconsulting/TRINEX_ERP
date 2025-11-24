using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class Tezgahlar
{
    public int Id { get; set; }

    public string Kod { get; set; } = null!;

    public string Ad { get; set; } = null!;

    public int? Tip { get; set; }

    public string? Daire { get; set; }

    public string? TakvimKodu { get; set; }

    public string? OperasyonTipi { get; set; }

    public string? SetupOperasyonTipi { get; set; }

    public string? HariciCariKod { get; set; }

    public string? Birim { get; set; }

    public decimal? MakineSayisi { get; set; }

    public decimal? IscilikSaati { get; set; }

    public decimal? SetupSuresi { get; set; }

    public decimal? Kapasite { get; set; }

    public decimal? PlanlananCalismaYuzdesi { get; set; }

    public decimal? OeeOrani { get; set; }

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
