using DuckDisk.models;
using DuckDisk.services;
using DuckDisk.utils;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;

namespace DuckDisk
{
    class Program
    {
        static void Main()
        {
            bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            
            if (!isAdmin)
            {
                 RestartAsAdmin();
                return;
            }
            
            MainMenu.ShowMainMenu();
        }
        
        private static void RestartAsAdmin()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = System.Reflection.Assembly.GetEntryAssembly().Location,
                Verb = "runas",
                UseShellExecute = true
            };

            try
            {
                Process.Start(startInfo);
                Environment.Exit(0);
            }
            catch
            {
                Console.WriteLine("Erro ao reiniciar como administrador");
            }
        }
    }
}