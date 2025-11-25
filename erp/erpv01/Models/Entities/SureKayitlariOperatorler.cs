using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class SureKayitlariOperatorler
{
    public int Id { get; set; }

    public string EvrakNo { get; set; } = null!;

    public int SiraNumarasi { get; set; }

    public string OperatorKod { get; set; } = null!;

    public DateTime? Tarih { get; set; }

    public decimal ToplamSure { get; set; }

    public decimal ToplamDakika { get; set; }

    public string? IsEmriNo { get; set; }

    public string? JobNo { get; set; }

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

    public bool AktifMi { get; set; }

    public DateTime OlusturmaTarihi { get; set; }

    public string? OlusturanKullanici { get; set; }

    public DateTime? GuncellemeTarihi { get; set; }

    public string? GuncelleyenKullanici { get; set; }

    public int KalemKodu { get; set; }
}
