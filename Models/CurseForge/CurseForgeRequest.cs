using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ModsChecksum.Models.CurseForge
{
    public class CurseForgeRequest
    {
        [JsonPropertyName("fingerprints")]
        public List<long> Fingerprints { get; set; } = new();
    }
} 