using System;
using System.Windows.Forms;

namespace ModsChecksum.UI
{
    public static class ConsoleHelper
    {
        public static void PrintIntro()
        {
            Console.Title = "Mods Checksum Verifier";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("======================================");
            Console.WriteLine("   Проверка контрольных сумм модов   ");
            Console.WriteLine("======================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static string? ShowFolderDialog()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку с модами";
                dialog.ShowNewFolderButton = false;

                Console.WriteLine("Пожалуйста, выберите папку с модами в открывшемся окне.");
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    Console.WriteLine($"Выбранная папка: {dialog.SelectedPath}");
                    return dialog.SelectedPath;
                }
                else
                {
                    Console.WriteLine("Папка не выбрана. Выход из приложения.");
                    return null;
                }
            }
        }

        public static void PrintOutro()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
} 