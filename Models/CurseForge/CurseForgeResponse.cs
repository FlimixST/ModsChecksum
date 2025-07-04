using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ModsChecksum.Models.CurseForge
{
    public class CurseForgeResponse
    {
        [JsonPropertyName("data")]
        public Data? Data { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("exactFingerprints")]
        public List<long> ExactFingerprints { get; set; } = new();
    }
} 