using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class SysAlanAciklamalari
{
    public int Id { get; set; }

    public string Evrakno { get; set; } = null!;

    public string? Aciklama1 { get; set; }

    public string? Aciklama2 { get; set; }

    public string? Aciklama3 { get; set; }

    public string? Aciklama4 { get; set; }

    public string? Aciklama5 { get; set; }

    public string? ReferansMetin1 { get; set; }

    public string? ReferansMetin2 { get; set; }

    public string? ReferansMetin3 { get; set; }

    public string? ReferansMetin4 { get; set; }

    public string? ReferansMetin5 { get; set; }

    public decimal? ReferansSayi1 { get; set; }

    public decimal? ReferansSayi2 { get; set; }

    public decimal? ReferansSayi3 { get; set; }

    public decimal? ReferansSayi4 { get; set; }

    public decimal? ReferansSayi5 { get; set; }

    public DateTime? ReferansTarih1 { get; set; }

    public DateTime? ReferansTarih2 { get; set; }

    public DateTime? ReferansTarih3 { get; set; }

    public DateTime? ReferansTarih4 { get; set; }

    public DateTime? ReferansTarih5 { get; set; }

    public DateTime? OlusturmaTarihi { get; set; }

    public string? OlusturanKullanici { get; set; }

    public DateTime? GuncellemeTarihi { get; set; }

    public string? GuncelleyenKullanici { get; set; }
}
