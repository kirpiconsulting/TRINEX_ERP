using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class SatisSiparisleri
{
    public int Id { get; set; }

    public string EvrakNo { get; set; } = null!;

    public string CariKod { get; set; } = null!;

    public DateTime Tarih { get; set; }

    public TimeOnly? IslemSaati { get; set; }

    public string? IkinciCariKod { get; set; }

    public string? SiparisVeren { get; set; }

    public string? ParaBirimi { get; set; }

    public string? FiyatSekli { get; set; }

    public string? FiyatListesi { get; set; }

    public decimal? ToplamTutar { get; set; }

    public decimal? IskontoToplami { get; set; }

    public decimal? KdvToplami { get; set; }

    public decimal? GenelToplam { get; set; }

    public string? SevkAdresi { get; set; }

    public string? SevkCariKod { get; set; }

    public string? NakliyeciFirma { get; set; }

    public string? NakliyeKosullari { get; set; }

    public string? OdemeKosullari { get; set; }

    public short? OdemeGunu { get; set; }

    public string? AcikKapali { get; set; }

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
