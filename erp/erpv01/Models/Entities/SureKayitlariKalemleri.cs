using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class SureKayitlariKalemleri
{
    public int Id { get; set; }

    public string EvrakNo { get; set; } = null!;

    public int KalemKodu { get; set; }

    public int SiraNumarasi { get; set; }

    public string StokKod { get; set; } = null!;

    public decimal StokMiktar { get; set; }

    public string? StokBirim { get; set; }

    public string? LotNo { get; set; }

    public string? SeriNo { get; set; }

    public string? IsMerkeziKod { get; set; }

    public string? Konum1 { get; set; }

    public DateTime? Tarih { get; set; }

    public string? YarimamulKod { get; set; }

    public string? ReceteKod { get; set; }

    public string? KalipKodu { get; set; }

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

    public string? UretimDeposuKodu { get; set; }

    public string? TuketimDeposuKodu { get; set; }

    public string? YanUrunDeposuKodu { get; set; }

    public string? FireDeposuKodu { get; set; }
}
