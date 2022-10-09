using System.Text.Json.Serialization;

namespace OpenDataViewer.Models
{
    public class RegObjStat
    {
        [JsonPropertyName("_id")]
        public int? id { get; set; }
        public int? Gads { get; set; }
        [JsonPropertyName("Mēnesis")]
        public string? Menesis { get; set; }
        [JsonPropertyName("Pilsētas")]
        public int? Pilsetas { get; set; }
        public int? Novadi { get; set; }
        public int? Pagasti { get; set; }
        public int? Ciemi { get; set; }
        public int? Mazciemi { get; set; }
        public int? Ielas { get; set; }
        [JsonPropertyName("Ēkas un apbuvei paredzētas zemes vienības")]
        public int? EkasUnZemesVienibas { get; set; }
        [JsonPropertyName("Telpu grupas")]
        public int? TelpuGrupas { get; set; }
    }
}
