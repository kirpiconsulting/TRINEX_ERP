using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class IsEmirleri
{
    public int Id { get; set; }

    public string EvrakNo { get; set; } = null!;

    public string? GrupEvrakNo { get; set; }

    public string StokKod { get; set; } = null!;

    public string? StokBirim { get; set; }

    public string? UretimPlani { get; set; }

    public decimal PlanlananMiktar { get; set; }

    public decimal UretilenMiktar { get; set; }

    public decimal BakiyeMiktar { get; set; }

    public decimal FireMiktar { get; set; }

    public DateTime? CariTeslimTarihi { get; set; }

    public DateTime? UretimdenTeslimTarihi { get; set; }

    public DateTime? RevizeTeslimTarihi { get; set; }

    public DateTime? KapanisTarihi { get; set; }

    public DateTime? GecerlilikTarihi { get; set; }

    public int? Durum { get; set; }

    public string AcikKapali { get; set; } = null!;

    public string? Oncelik { get; set; }

    public string? CariKod { get; set; }

    public string? CariSiparisEvrakno { get; set; }

    public string? CariSiparisKalemKodu { get; set; }

    public string? KaynakIsEmriEvrakno { get; set; }

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

    public DateTime Tarih { get; set; }
}
