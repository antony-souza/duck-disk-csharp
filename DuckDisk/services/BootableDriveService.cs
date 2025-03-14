using System.Diagnostics;
using System.Security.Principal;
using DuckDisk.interfaces;
using DuckDisk.models;
using DuckDisk.utils;

namespace DuckDisk.services;

public class BootableDriveService : IBootableDrive
{

    public async Task<bool> FormatDriveAsync(string drivePath)
    {
        if (string.IsNullOrEmpty(drivePath) || !Path.IsPathRooted(drivePath) || drivePath.Length < 2 ||
            drivePath[1] != ':')
        {
            Console.WriteLine("Caminho inválido! O path deve ser no formato de uma letra de unidade (ex: C:).");
            return false;
        }
        
        var drive = new DriveInfo(drivePath);

        if (!drive.IsReady || !Directory.Exists(drivePath))
        {
            Console.WriteLine("Erro: O drive não foi encontrado ou não está acessível!");
            return false;
        }

        Console.WriteLine();
        Console.WriteLine("Você está prestes a formatar o drive: ");
        Console.WriteLine();
        Console.WriteLine($"Caminho: {drive.Name}");
        Console.WriteLine($"Nome: {drive.VolumeLabel}");
        Console.WriteLine($"Tamanho: {FormatBytes.Format(drive.TotalSize)}");
        Console.WriteLine($"Tipo: {drive.DriveType}");
        Console.WriteLine($"Formato: {drive.DriveFormat}");
        Console.WriteLine();
        Console.Write("Deseja realmente formatar? (sim(s)/não(n): ");
        string resposta = Console.ReadLine()?.ToLower();

        if (resposta != "sim" && resposta != "s")
        {
            Console.WriteLine("Formatação cancelada.");
            return false;
        }

        Console.WriteLine($"Formatando o drive {drive.Name} - ({drive.VolumeLabel})...");

        string scriptFormatPath = Path.Combine(Path.GetTempPath(), "format_script.txt");

        string[] commandsScriptFormatPath =
        {
            $"select volume {drivePath.Replace("\\", "")}",
            "clean",
            "create partition primary",
            "format fs=ntfs quick",
            "assign",
            "exit"
        };

        await File.WriteAllLinesAsync(scriptFormatPath, commandsScriptFormatPath);

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "diskpart",
            Verb = "runas",
            Arguments = $"/s {scriptFormatPath}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        try
        {
            using (var process = new Process { StartInfo = psi })
            {
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Erro ao formatar o drive. Erro: {error}");
                    return false;
                }

                Console.WriteLine("Formatação concluída com sucesso!");
                Console.WriteLine($"Saída: {output}");

                if (process.ExitCode == 0)
                {
                    File.Delete(scriptFormatPath);
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao iniciar o processo: {ex.Message}");
            return false;
        }
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
        {
            $"select volume {driveModel.DrivePath.Replace("\\", "")}",
            "active",
            "exit"
        };
        await CreateTempPathScript(createPathScriptPartitionActiveBoot, commandsCreatePartitionActiveBoot);

        Console.WriteLine("Montando a ISO e copiando arquivos...");
        if (!await CopyFilesFromIsoForDrive(driveModel.IsoPath, driveModel.DrivePath))
        {
            Console.WriteLine("Erro ao copiar arquivos da ISO.");
            return false;
        }

        Console.WriteLine("Configurando boot...");
        string bootCommand = $"bcdboot {driveModel.DrivePath}\\Windows /s {driveModel.DrivePath} /f ALL";
        if (!await ExecuteCommand("cmd.exe", $"/c {bootCommand}"))
        {
            Console.WriteLine("Erro ao configurar boot.");
            return false;
        }

        Console.WriteLine("Drive bootável criado com sucesso!");
        return true;
    }

    private async Task<bool> CopyFilesFromIsoForDrive(string isoPath, string drivePath)
    {
        string mountCommand = $"powershell Mount-DiskImage -ImagePath \"{isoPath}\"";
        if (!await ExecuteCommand("powershell", $"-Command \"{mountCommand}\""))
            return false;

        DriveInfo mountedDrive = DriveInfo.GetDrives().FirstOrDefault(d => d.DriveFormat == "UDF");
        if (mountedDrive == null)
        {
            Console.WriteLine("Erro: Não foi possível montar a ISO.");
            return false;
        }

        string sourcePath = mountedDrive.RootDirectory.FullName;
        string destinationPath = drivePath;

        Console.WriteLine($"Copiando arquivos de {sourcePath} para {destinationPath}...");
        return await ExecuteCommand("xcopy", $"\"{sourcePath}\" \"{destinationPath}\" /E /H /C /Y");
    }

    private async Task<bool> CreateTempPathScript(string scriptPath, string[] commands)
    {
        string createPathCommand = Path.Combine(Path.GetTempPath(), scriptPath);
        //Aqui é o runCreateScriptPath
        await File.WriteAllLinesAsync(createPathCommand, commands);

        return await ExecuteCommand("diskpart", $"/s {createPathCommand}");
    }

    private async Task<bool> ExecuteCommand(string executableProcess, string commands)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = executableProcess,
            Arguments = commands,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (var process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Erro ao executar comando: {error}");
                    return false;
                }

                Console.WriteLine(output);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao executar processo: {ex.Message}");
            return false;
        }
    }

    public void MenuBootableDrive()
    {
        DriveService driveService = new DriveService();
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
                    driveService.GetAllDriveAsync().Wait();
                    Console.WriteLine();
                    Console.Write("Digite o caminho do drive a ser formatado (ex: G:): ");
                    string pathDriveFormated = Console.ReadLine();
                    if (!Directory.Exists(pathDriveFormated))
                    {
                        Console.WriteLine("Caminho do drive não encontrado!");
                        break;
                    }
                    FormatDriveAsync(pathDriveFormated).Wait();
                    driveService.GetAllDriveAsync().Wait();
                    
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