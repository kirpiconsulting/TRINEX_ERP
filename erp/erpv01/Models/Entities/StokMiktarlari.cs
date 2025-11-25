using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class StokMiktarlari
{
    public int Id { get; set; }

    public string DepoKod { get; set; } = null!;

    public string StokKod { get; set; } = null!;

    public decimal StokMiktar { get; set; }

    public string? StokBirim { get; set; }

    public DateTime? Tarih1 { get; set; }

    public DateTime? Tarih2 { get; set; }

    public DateTime? Tarih3 { get; set; }

    public DateTime? Tarih4 { get; set; }

    public DateTime? Tarih5 { get; set; }

    public DateTime? Tarih6 { get; set; }

    public DateTime? Tarih7 { get; set; }

    public DateTime? Tarih8 { get; set; }

    public DateTime? Tarih9 { get; set; }

    public DateTime? Tarih10 { get; set; }

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
