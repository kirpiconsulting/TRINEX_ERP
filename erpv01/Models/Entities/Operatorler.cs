using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class Operatorler
{
    public int Id { get; set; }

    public string Kod { get; set; } = null!;

    public string Ad { get; set; } = null!;

    public int? Turu { get; set; }

    public string? CalisabilecegiTezgahlar { get; set; }

    public string? Adres { get; set; }

    public string? Sehir { get; set; }

    public string? PostaKodu { get; set; }

    public string? Telefon { get; set; }

    public string? GsmNo { get; set; }

    public string? Faks { get; set; }

    public string? EPosta { get; set; }

    public string? IlgiliCariKod { get; set; }

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
