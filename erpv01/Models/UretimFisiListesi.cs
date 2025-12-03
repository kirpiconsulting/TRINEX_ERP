using System.ComponentModel.DataAnnotations;

namespace erpv01.Models.Uretim
{
    public class UretimFisiListeDto
    {
        [Display(Name = "Evrak No")]
        public string EvrakNo { get; set; }

        [Display(Name = "Tarih")]
        public DateTime Tarih { get; set; }

        [Display(Name = "Kod")]
        public string StokKod { get; set; }

        [Display(Name = "Miktar")]
        public decimal Miktar { get; set; }

        [Display(Name = "İş Emri Numarası")]
        public string is_emri_no { get; set; }

        [Display(Name = "Lot Numarası")]
        public string lot_no { get; set; }

        [Display(Name = "Seri Numarası")]
        public string seri_no { get; set; }

        [Display(Name = "Üretim Deposu")]
        public string UretimDeposuKodu { get; set; }

        [Display(Name = "Tüketim Deposu")]
        public string TuketimDeposuKodu { get; set; }

    }
}
