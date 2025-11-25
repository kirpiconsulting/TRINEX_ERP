using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class UretimFisiTuketimler
{
    public int Id { get; set; }

    public string EvrakNo { get; set; } = null!;

    public string? KalemKodu { get; set; }

    public int SiraNumarasi { get; set; }

    public string StokKod { get; set; } = null!;

    public string? LotNo { get; set; }

    public string? SeriNo { get; set; }

    public decimal Miktar { get; set; }

    public string? Birim { get; set; }

    public string? IsMerkeziKod { get; set; }

    public string? Konum1 { get; set; }

    public DateTime? Tarih { get; set; }

    public string? KarsiStokKod { get; set; }

    public string? KarsiLotNo { get; set; }

    public string? KarsiSeriNo { get; set; }

    public decimal KarsiMiktar { get; set; }

    public string? KarsiIsMerkeziKod { get; set; }

    public string? KarsiKonum1 { get; set; }

    public string? Nitelik { get; set; }

    public string? FasonNormal { get; set; }

    public bool FireDagitimdanMi { get; set; }

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
