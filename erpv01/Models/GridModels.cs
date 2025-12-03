using System.ComponentModel.DataAnnotations;

namespace erpv01.Models.Grid
{
    public class GridColumnDef
    {
        public string header { get; set; }
        public string name { get; set; }
        public int? width { get; set; }
        public int? minWidth { get; set; }
        public string align { get; set; }
        public bool? sortable { get; set; }
        public object filter { get; set; }
    }

    // Ekrana veri basmak için ortak ViewModel
    public class GridViewModel
    {
        public string DataJson { get; set; }      // Kayıtların JSON’u
        public string ColumnsJson { get; set; }   // Kolonların JSON’u
        public string Baslik { get; set; }        // Sayfa başlığı
    }

    public static class GridHelper
    {
        // Model tipine göre kolonları otomatik üret
        public static List<GridColumnDef> BuildColumnsFromType<T>()
        {
            var list = new List<GridColumnDef>();
            var props = typeof(T).GetProperties();

            foreach (var p in props)
            {
                // İstersen bazı property’leri gizlemek için şart koyabilirsin:
                // if (p.Name == "Id") continue;

                var display = p.GetCustomAttributes(typeof(DisplayAttribute), true)
                               .Cast<DisplayAttribute>()
                               .FirstOrDefault();

                var headerText = display?.Name ?? p.Name;

                // JS tarafı camelCase kullanıyor
                var nameCamel = char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1);

                var col = new GridColumnDef
                {
                    header = headerText,
                    name = nameCamel,
                    minWidth = 120,
                    align = "left",
                    sortable = true,
                    filter = "select"
                };

                // Sayısal alanları sağa hizala
                var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (t == typeof(decimal) || t == typeof(double) || t == typeof(float) ||
                    t == typeof(int) || t == typeof(long))
                {
                    col.align = "right";
                }

                list.Add(col);
            }

            return list;
        }
    }
}
