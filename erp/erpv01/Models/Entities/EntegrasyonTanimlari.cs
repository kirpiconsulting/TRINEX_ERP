using System;
using System.Collections.Generic;

namespace erpv01.Models.Entities;

public partial class EntegrasyonTanimlari
{
    public int Id { get; set; }

    public string Kod { get; set; } = null!;

    public string? Aciklama { get; set; }

    public string? Deger1 { get; set; }

    public string? Deger2 { get; set; }

    public string? Deger3 { get; set; }

    public string? Deger4 { get; set; }

    public string? Deger5 { get; set; }

    public string? Deger6 { get; set; }

    public string? Deger7 { get; set; }

    public string? Deger8 { get; set; }

    public string? Deger9 { get; set; }

    public string? Deger10 { get; set; }

    public bool AktifMi { get; set; }

    public DateTime OlusturmaTarihi { get; set; }

    public string? OlusturanKullanici { get; set; }

    public DateTime? GuncellemeTarihi { get; set; }

    public string? GuncelleyenKullanici { get; set; }
}
