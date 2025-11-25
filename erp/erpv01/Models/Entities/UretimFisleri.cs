using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class UretimFisleri
{
    public int Id { get; set; }

    public string EvrakNo { get; set; } = null!;

    public DateTime Tarih { get; set; }

    public string? UretimDeposuKodu { get; set; }

    public string? TuketimDeposuKodu { get; set; }

    public string? YanUrunDeposuKodu { get; set; }

    public string? FireDeposuKodu { get; set; }

    public string? TeslimAlan { get; set; }

    public string? Nitelik { get; set; }

    public string? FasonNormal { get; set; }

    public string? OzelFlag { get; set; }

    public bool OnayliMi { get; set; }

    public bool RezervasyonYapildiMi { get; set; }

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
