using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ModsChecksum.Utils;

namespace ModsChecksum.Services
{
    public enum ModrinthCheckResult
    {
        Valid,
        NotFound,
        Error
    }

    public class ModrinthService
    {
        private readonly HttpClient _client;

        public ModrinthService(HttpClient client)
        {
            _client = client;
        }

        public async Task<ModrinthCheckResult> CheckModAsync(string modFilePath)
        {
            string sha1Hash = FingerprintCalculator.CalculateSHA1(modFilePath);
            string modrinthApiUrl = $"https://api.modrinth.com/v2/version_file/{sha1Hash}";
            try
            {
                HttpResponseMessage modrinthResponse = await _client.GetAsync(modrinthApiUrl);

                if (modrinthResponse.IsSuccessStatusCode)
                {
                    return ModrinthCheckResult.Valid;
                }
                if (modrinthResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return ModrinthCheckResult.NotFound;
                }
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Ошибка при проверке на Modrinth. Статус: {modrinthResponse.StatusCode}");
                Console.ResetColor();
                return ModrinthCheckResult.Error;
            }
            catch (HttpRequestException e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Ошибка запроса для файла {Path.GetFileName(modFilePath)}: {e.Message}");
                Console.ResetColor();
                return ModrinthCheckResult.Error;
            }
        }
    }
} 