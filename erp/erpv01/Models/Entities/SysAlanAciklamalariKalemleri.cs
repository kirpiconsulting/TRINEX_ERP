using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class SysAlanAciklamalariKalemleri
{
    public int Id { get; set; }

    public string Evrakno { get; set; } = null!;

    public string FieldAdi { get; set; } = null!;

    public string? Aciklama { get; set; }

    public string? VeriTuru { get; set; }

    public int? MaxUzunluk { get; set; }

    public bool? Zorunlu { get; set; }

    public string? VarsayilanDeger { get; set; }

    public DateTime? OlusturmaTarihi { get; set; }

    public string? OlusturanKullanici { get; set; }

    public DateTime? GuncellemeTarihi { get; set; }

    public string? GuncelleyenKullanici { get; set; }
}
