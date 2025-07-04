using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModsChecksum.Services;
using ModsChecksum.Utils;

namespace ModsChecksum.Logic
{
    public class ModProcessor
    {
        private readonly CurseForgeService _curseForgeService;
        private readonly ModrinthService _modrinthService;

        public ModProcessor(CurseForgeService curseForgeService, ModrinthService modrinthService)
        {
            _curseForgeService = curseForgeService;
            _modrinthService = modrinthService;
        }

        public async Task ProcessModsAsync(string modsPath)
        {
            if (!Directory.Exists(modsPath))
            {
                Console.WriteLine("Указанная папка не существует.");
                return;
            }

            string[] modFiles = Directory.GetFiles(modsPath, "*.jar");

            if (modFiles.Length == 0)
            {
                Console.WriteLine("В указанной папке не найдено модов (.jar файлов).");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nНайдено {modFiles.Length} модов. Начинаю проверку...");
            Console.ResetColor();

            var modFingerprints = new Dictionary<string, long>();
            foreach (var modFile in modFiles)
            {
                try
                {
                    var fingerprint = FingerprintCalculator.CalculateCurseForgeFingerprint(modFile);
                    modFingerprints.Add(modFile, fingerprint);
                }
                catch (IOException ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Ошибка чтения файла {Path.GetFileName(modFile)}: {ex.Message}");
                    Console.ResetColor();
                }
            }

            if (modFingerprints.Count == 0)
            {
                Console.WriteLine("Не удалось вычислить цифровые отпечатки. Прерываю.");
                return;
            }

            var (matchedFingerprints, success) = await _curseForgeService.GetMatchedFingerprintsAsync(modFingerprints.Values);

            if (!success || matchedFingerprints == null)
            {
                Console.WriteLine("Не удалось получить ответ от CurseForge API. Проверка прервана.");
                return;
            }
            
            int curseForgeCount = 0;
            int modrinthCount = 0;
            int notFoundCount = 0;
            int errorCount = 0;

            Console.WriteLine("\nРезультаты проверки:");
            Console.WriteLine("-------------------");

            foreach (var mod in modFingerprints)
            {
                string modFile = mod.Key;
                string fileName = Path.GetFileName(modFile);

                if (matchedFingerprints.Contains(mod.Value))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("  [+] ");
                    Console.ResetColor();
                    Console.WriteLine($"{fileName} (CurseForge)");
                    curseForgeCount++;
                }
                else
                {
                    var modrinthResult = await _modrinthService.CheckModAsync(modFile);
                    switch (modrinthResult)
                    {
                        case ModrinthCheckResult.Valid:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("  [+] ");
                            Console.ResetColor();
                            Console.WriteLine($"{fileName} (Modrinth)");
                            modrinthCount++;
                            break;
                        case ModrinthCheckResult.NotFound:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("  [-] ");
                            Console.ResetColor();
                            Console.WriteLine($"{fileName} (Не найден)");
                            notFoundCount++;
                            break;
                        case ModrinthCheckResult.Error:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("  [!] ");
                            Console.ResetColor();
                            Console.WriteLine($"{fileName} (Ошибка проверки)");
                            errorCount++;
                            break;
                    }
                }
            }
            
            Console.WriteLine("\n-------------------");
            Console.WriteLine("Сводка:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  - Найдено на CurseForge: {curseForgeCount}");
            Console.WriteLine($"  - Найдено на Modrinth:   {modrinthCount}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  - Не найдено:            {notFoundCount}");
            if (errorCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  - Ошибки:              {errorCount}");
            }
            Console.ResetColor();
        }
    }
} 