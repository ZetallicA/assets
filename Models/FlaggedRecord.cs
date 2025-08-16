using System.Text.Json;

namespace AssetManagement.Models
{
    public class FlaggedRecord
    {
        public int RowNumber { get; set; }
        public string OathTag { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string Model { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string Category { get; set; } = "";
        public string Unit { get; set; } = "";
        public string Status { get; set; } = "";
        public string Issue { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public Dictionary<string, object> OriginalRowData { get; set; } = new();
        public string ImportType { get; set; } = "";
        public DateTime FlaggedAt { get; set; } = DateTime.UtcNow;

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static FlaggedRecord FromJson(string json)
        {
            return JsonSerializer.Deserialize<FlaggedRecord>(json) ?? new FlaggedRecord();
        }
    }
}
