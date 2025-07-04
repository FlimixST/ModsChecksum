using System;
using System.Net.Http;
using System.Threading.Tasks;
using ModsChecksum.Logic;
using ModsChecksum.Services;
using ModsChecksum.UI;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ConsoleHelper.PrintIntro();

        string? modsPath = ConsoleHelper.ShowFolderDialog();

        if (string.IsNullOrEmpty(modsPath))
        {
            return;
        }

        ProcessModsAsync(modsPath).GetAwaiter().GetResult();
        
        ConsoleHelper.PrintOutro();
    }

    static async Task ProcessModsAsync(string modsPath)
    {
        using (var client = new HttpClient())
        {
            var curseForgeService = new CurseForgeService(client);
            var modrinthService = new ModrinthService(client);
            var modProcessor = new ModProcessor(curseForgeService, modrinthService);

            await modProcessor.ProcessModsAsync(modsPath);
        }
    }
}
