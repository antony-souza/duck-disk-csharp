using System.Diagnostics;
using DuckDisk.interfaces;
using DuckDisk.models;
using DuckDisk.utils;
using DuckDisk.utils.BootableDriveUtilClass;

namespace DuckDisk.services;

public class BootableDriveService : IBootableDrive
{
    private readonly ExecuteCommandGeneric _executeCommandGeneric;
    private readonly DriveService _driveService;
    private readonly CreateTempPathScriptGeneric _createTempPathScriptGeneric;
    private readonly CompyFilesFromIsoForDriveGeneric _copyFilesFromIsoForDriveGeneric;

    public BootableDriveService()
    {
        _driveService = new DriveService();
        _executeCommandGeneric = new ExecuteCommandGeneric();
        _createTempPathScriptGeneric = new CreateTempPathScriptGeneric();
        _copyFilesFromIsoForDriveGeneric = new CompyFilesFromIsoForDriveGeneric();
    }

    public async Task<bool> FormatDriveAsync(string drivePath)
    {
        var drive = await _driveService.GetDriveDetailsAsync(drivePath);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Você está prestes a formatar o drive: ");
        Console.WriteLine();
        Console.WriteLine(
            $"Path: {drive.Path}, " +
            $"Name: {drive.Name}, " +
            $"Size: {FormatBytes.Format(drive.Size)}, " +
            $"Free: {FormatBytes.Format(drive.FreeSpace)}, " +
            $"Type: {drive.Type}, " +
            $"Format: {drive.Format}");
        Console.WriteLine();
        Console.Write("Deseja realmente formatar? sim(s)/não(n): ");
        Console.ResetColor();
        string resposta = Console.ReadLine()?.ToLower();

        if (resposta != "sim" && resposta != "s")
        {
            Console.WriteLine("Formatação cancelada.");
            return false;
        }

        Console.WriteLine($"Formatando o drive {drive.Name} - {drive.Name}... :)");

        string scriptFormatPath = "format_script.txt";

        string[] commandsScriptFormatPath =
        [
            $"select volume {drivePath.Replace("\\", "")}",
            "clean",
            "create partition primary",
            "format fs=ntfs quick label=DuckDrive",
            "assign",
            "exit"
        ];

        await _createTempPathScriptGeneric.CreateTempPathScript(scriptFormatPath, commandsScriptFormatPath);

        await _executeCommandGeneric.ExecuteCommand("diskpart", $"/s {scriptFormatPath}");

        return true;
    }

    public async Task<bool> CreateBootableDriveAsync(BootableDriveModel driveModel)
    {
        if (!File.Exists(driveModel.IsoPath))
        {
            Console.WriteLine("Erro: Arquivo ISO não encontrado.");
            return false;
        }

        var drive = new DriveInfo(driveModel.DrivePath);
        if (!drive.IsReady)
        {
            Console.WriteLine("Erro: O drive não foi encontrado ou não está acessível.");
            return false;
        }

        Console.WriteLine("Configurando partição ativa...");

        string createPathScriptPartitionActiveBoot = "format_script.txt";

        string[] commandsCreatePartitionActiveBoot =
        [
            $"select volume {driveModel.DrivePath.Replace("\\", "")}",
            "active",
            "exit"
        ];

        await _createTempPathScriptGeneric.CreateTempPathScript(createPathScriptPartitionActiveBoot,
            commandsCreatePartitionActiveBoot);

        Console.WriteLine("Montando a ISO e copiando arquivos...");
        if (!await _copyFilesFromIsoForDriveGeneric.CopyFiles(driveModel.IsoPath, driveModel.DrivePath))
        {
            Console.WriteLine("Erro ao copiar arquivos da ISO.");
            return false;
        }

        Console.WriteLine("Configurando boot...");

        string bootCommand = $"bcdboot {driveModel.DrivePath}\\Windows /s {driveModel.DrivePath} /f ALL";
        if (!await _executeCommandGeneric.ExecuteCommand("cmd.exe", $"/c {bootCommand}"))
        {
            Console.WriteLine("Erro ao configurar boot.");
            return false;
        }

        Console.WriteLine("Drive bootável criado com sucesso!");

        return true;
    }

    public void MenuBootableDrive()
    {
        while (true)
        {
            Console.Clear();
            DuckDriveTitle.DisplayTitle();
            Console.WriteLine("===== Menu de Drives Bootáveis =====");
            Console.WriteLine();
            Console.WriteLine("1. Criar um Drive Bootável(Formata Automaticamente)");
            Console.WriteLine("2. Sair");
            Console.WriteLine();
            Console.Write("Escolha uma opção: ");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    _driveService.GetAllDriveAsync().Wait();
                    Console.WriteLine();
                    Console.Write("Digite o caminho do drive a ser formatado (ex: G:): ");
                    string pathDriveFormated = Console.ReadLine();
                    if (!Directory.Exists(pathDriveFormated))
                    {
                        Console.WriteLine("Caminho do drive não encontrado!");
                        break;
                    }

                    FormatDriveAsync(pathDriveFormated).Wait();
                    _driveService.GetAllDriveAsync().Wait();

                    Console.WriteLine();
                    Console.Write("Digite o caminho completo do drive de criação bootável(ex: F:): ");
                    string drivePath = Console.ReadLine();
                    if (!Directory.Exists(drivePath))
                    {
                        Console.WriteLine("Caminho do drive não encontrado!");
                        break;
                    }

                    Console.Write(
                        "Digite o caminho completo do arquivo ISO(ex: C:\\Users\\user\\Downloads\\windows.iso): ");
                    string isoPath = Console.ReadLine();

                    if (!File.Exists(isoPath))
                    {
                        Console.WriteLine("Caminho para arquivo ISO não existe!");
                        break;
                    }

                    BootableDriveModel bootableDrive = new BootableDriveModel
                    {
                        DrivePath = drivePath,
                        IsoPath = isoPath
                    };

                    CreateBootableDriveAsync(bootableDrive).Wait();
                    break;

                case "2":
                    return;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
}