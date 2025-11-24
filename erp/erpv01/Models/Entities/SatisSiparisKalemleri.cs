using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class SatisSiparisKalemleri
{
    public int Id { get; set; }

    public string EvrakNo { get; set; } = null!;

    public string KalemKodu { get; set; } = null!;

    public int SiraNumarasi { get; set; }

    public string? StokKod { get; set; }

    public string? StokBirim { get; set; }

    public DateTime? TeslimTarihi { get; set; }

    public decimal Miktar { get; set; }

    public string? Notlar { get; set; }

    public decimal BirimFiyat { get; set; }

    public decimal ToplamTutar { get; set; }

    public decimal IskontoOrani { get; set; }

    public decimal IskontoTutar { get; set; }

    public decimal KdvOrani { get; set; }

    public decimal KdvTutar { get; set; }

    public string? AcikKapaliDurum { get; set; }

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
