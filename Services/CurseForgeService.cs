using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ModsChecksum.Models.CurseForge;

namespace ModsChecksum.Services
{
    public class CurseForgeService
    {
        private readonly HttpClient _client;

        public CurseForgeService(HttpClient client)
        {
            _client = client;
            _client.DefaultRequestHeaders.Add("User-Agent", "ModsChecksumApp/1.0");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<(HashSet<long>? matchedFingerprints, bool success)> GetMatchedFingerprintsAsync(IEnumerable<long> fingerprints)
        {
            var requestBody = new CurseForgeRequest { Fingerprints = fingerprints.ToList() };
            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            string apiUrl = "https://api.curse.tools/v1/cf/fingerprints";

            try
            {
                HttpResponseMessage response = await _client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    var curseForgeResponse = JsonSerializer.Deserialize<CurseForgeResponse>(responseBody);
                    if (curseForgeResponse?.Data?.ExactFingerprints != null)
                    {
                        return (new HashSet<long>(curseForgeResponse.Data.ExactFingerprints), true);
                    }
                    return (new HashSet<long>(), true);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Ошибка во время вызова API CurseForge. Статус: {response.StatusCode}");
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ответ: {errorBody}");
                    Console.ResetColor();
                    return (null, false);
                }
            }
            catch (HttpRequestException e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Ошибка запроса к CurseForge: {e.Message}");
                Console.ResetColor();
                return (null, false);
            }
        }
    }
} 