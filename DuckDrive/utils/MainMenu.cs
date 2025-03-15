using DuckDisk.services;
using Figgle;

namespace DuckDisk.utils;

public class MainMenu
{

    public static void ShowMainMenu()
    {
        DriveService driveService = new DriveService();
        IsoService isoService = new IsoService();
        BootableDriveService bootableDriveService = new BootableDriveService();
        while (true)
        {
            Console.Clear();
            DuckDriveTitle.DisplayTitle();
            Console.WriteLine("----- Menu do Duck Drive -----");
            Console.WriteLine();
            Console.WriteLine("1. Criação do Drive Bootável");
            Console.WriteLine("2. Sair");
            Console.WriteLine();
            Console.Write("Escolha uma opção: ");

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    bootableDriveService.MenuBootableDrive();

                    break;
                case "2":
                    Console.WriteLine("Saindo...");
                    return;

                default:
                    Console.WriteLine("\nOpção inválida. Tente novamente.");
                    break;
            }

            Console.WriteLine("\nPressione qualquer tecla para voltar ao Menu Principal...");
            Console.ReadKey();
        }
    }
}