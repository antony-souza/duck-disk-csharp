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
        var drive = new DriveInfo(drivePath);

        if (!drive.IsReady)
        {
            Console.WriteLine("Erro: O drive não foi encontrado!");
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

        Console.WriteLine($"Formatando o drive {drive.Name} ({drive.VolumeLabel})...");

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

        ProcessStartInfo psi = new ProcessStartInfo("diskpart")
        {
            //Verb = "runas" solicita ser admin
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


    public Task<bool> CreateBootableDriveAsync(BootableDriveModel driveModel)
    {
        Console.WriteLine("Drive bootável criado com sucesso!");
        return Task.FromResult(true);
    }

    public void MenuBootableDrive()
    {
        while (true)
        {
            string drivePath = String.Empty;
            Console.Clear();
            DuckDriveTitle.DisplayTitle();
            Console.WriteLine("===== Menu de Drives Bootáveis =====");
            Console.WriteLine();
            Console.WriteLine("1. Formatar um Drive");
            Console.WriteLine("2. Criar um Drive Bootável(Não implementado)");
            Console.WriteLine("3. Sair");
            Console.WriteLine();
            Console.Write("Escolha uma opção: ");
            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    Console.Write("Digite o caminho completo do drive a ser formatado (ex: F:): ");
                    drivePath = Console.ReadLine();
                    FormatDriveAsync(drivePath).Wait();
                    break;

                case "2":
                    Console.WriteLine("Não implementado ainda");
                    Console.Write("Digite o caminho completo do drive de criação (ex: F:): ");
                    drivePath = Console.ReadLine();

                    Console.Write(
                        "Digite o caminho completo do arquivo ISO(ex: C:\\Users\\user\\Downloads\\windows.iso): ");
                    string isoPath = Console.ReadLine();

                    BootableDriveModel bootableDrive = new BootableDriveModel
                    {
                        DrivePath = drivePath,
                        IsoPath = isoPath
                    };

                    CreateBootableDriveAsync(bootableDrive).Wait();
                    break;

                case "3":
                    return; // Sai do loop e encerra o menu

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            // Pausar o menu antes de voltar
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
}