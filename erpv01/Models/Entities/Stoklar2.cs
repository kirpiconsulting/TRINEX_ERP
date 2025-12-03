using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class Stoklar2
{
    public int Id { get; set; }

    public short? StokTur { get; set; }

    public decimal? AlisKdvo { get; set; }

    public decimal? SatisKdvo { get; set; }

    public decimal? TevkifatOran { get; set; }

    public string? TevkifatKod { get; set; }

    public string StokKod { get; set; } = null!;

    public string? StokAd { get; set; }

    public string? StokBirim { get; set; }

    public string? Fiyat1Pb { get; set; }

    public string? Fiyat2Pb { get; set; }

    public string? Fiyat3Pb { get; set; }

    public string? Fiyat4Pb { get; set; }

    public string? Fiyat5Pb { get; set; }

    public decimal? Fiyat1Kdv { get; set; }

    public decimal? Fiyat2Kdv { get; set; }

    public decimal? Fiyat3Kdv { get; set; }

    public decimal? Fiyat4Kdv { get; set; }

    public decimal? Fiyat5Kdv { get; set; }

    public decimal? SatIskonto { get; set; }

    public string? Grup1 { get; set; }

    public string? Grup2 { get; set; }

    public string? Grup3 { get; set; }

    public string? Grup4 { get; set; }

    public string? Grup5 { get; set; }

    public string? Grup6 { get; set; }

    public string? Grup7 { get; set; }

    public string? Grup8 { get; set; }

    public string? Grup9 { get; set; }

    public string? Grup10 { get; set; }

    public string? StokSinif { get; set; }

    public string? StokTuru { get; set; }

    public short? StokKayitDurumu { get; set; }

    public string? StokDoviz { get; set; }

    public string? MalzemeTur { get; set; }

    public string? StokResim { get; set; }

    public string? StokRaf { get; set; }

    public string? StokEkyuzey { get; set; }

    public decimal? StokMin { get; set; }

    public decimal? StokMax { get; set; }

    public decimal? StokOpt { get; set; }

    public decimal? StokKritik { get; set; }

    public int? TeminSure { get; set; }

    public int? GarantiSure { get; set; }

    public string? Birim1 { get; set; }

    public string? Birim2 { get; set; }

    public string? Birim3 { get; set; }

    public decimal? Birimcarpan1 { get; set; }

    public decimal? Birimcarpan2 { get; set; }

    public decimal? Birimcarpan3 { get; set; }

    public decimal? Fiyat1 { get; set; }

    public decimal? Fiyat2 { get; set; }

    public decimal? Fiyat3 { get; set; }

    public decimal? Fiyat4 { get; set; }

    public decimal? Fiyat5 { get; set; }

    public decimal? GuvenlikMiktar { get; set; }

    public decimal? MinpartiMiktar { get; set; }

    public decimal? MinsiparisMiktar { get; set; }

    public decimal? MaxenvanterMiktar { get; set; }

    public decimal? En { get; set; }

    public decimal? Boy { get; set; }

    public decimal? Yukseklik { get; set; }

    public decimal? Alan { get; set; }

    public decimal? AgirlikNet { get; set; }

    public decimal? AgirlikBrut { get; set; }

    public decimal? HacimNet { get; set; }

    public decimal? HacimBrut { get; set; }

    public bool? Ap { get; set; }

    public string? Gtipno { get; set; }

    public decimal? EkNum1 { get; set; }

    public decimal? EkNum2 { get; set; }

    public decimal? EkNum3 { get; set; }

    public DateTime? EkTarih1 { get; set; }

    public string? EkAlan1 { get; set; }

    public string? EkAlan2 { get; set; }

    public string? EkAlan3 { get; set; }
}
