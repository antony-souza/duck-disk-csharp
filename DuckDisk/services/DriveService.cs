using DuckDisk.interfaces;
using DuckDisk.models;
using DuckDisk.utils;

namespace DuckDisk.services;

public class DriveService : IDrive
{
    
    public async Task<List<DriveModel>> GetAllDriveAsync()
    {
        return await Task.Run(() =>
        {
            List<DriveModel> drives = new List<DriveModel>();

            DriveInfo[] allDrivesArray = DriveInfo.GetDrives();
            
            foreach (DriveInfo drive in allDrivesArray)
            {
                //Filtro por pendrive(Removable)
                if (drive.DriveType == DriveType.Removable)
                {
                    if (!drive.IsReady)
                    {
                        Console.WriteLine("Drives removíveis com problema!");
                    }
                    drives.Add(new DriveModel
                    {
                        Path = drive.Name,
                        Name = drive.VolumeLabel,
                        Size = drive.TotalSize,
                        Type = drive.DriveType.ToString(),
                        Format = drive.DriveFormat.ToString(),
                    });
                }
            }

            Console.Clear();
            DuckDriveTitle.DisplayTitle();
            Console.WriteLine("Drives Removíveis Disponíveis:");
            Console.WriteLine();
            foreach (var drive in drives)
            {
                var formatSize = FormatBytes.Format(drive.Size);
                
                Console.WriteLine(
                    $"Path: {drive.Path}, " +
                    $"Name: {drive.Name}, " +
                    $"Size: {formatSize}, " +
                    $"Type: {drive.Type}, " +
                    $"Format: {drive.Format}");
            }

            return drives;
        });
    }

    public async Task<DriveModel> GetDriveDetailsAsync(string path)
    {
        if (string.IsNullOrEmpty(path) || !Path.IsPathRooted(path) || path.Length < 2 || path[1] != ':')
        {
            Console.WriteLine("Caminho inválido! O path deve ser no formato de uma letra de unidade (ex: C:).");
            return new DriveModel();
        }

        var drive = new DriveInfo(path);

        if (!drive.IsReady)
        {
            Console.WriteLine("Drive não encontrado ou não está pronto!");
            return new DriveModel();
        }

        return await Task.Run(() =>
        {
            var driveDetails = new DriveModel
            {
                Path = drive.Name,
                Name = drive.VolumeLabel,
                Size = drive.TotalSize,
                Type = drive.DriveType.ToString(),
                Format = drive.DriveFormat.ToString(),
            };
            var formatSize = FormatBytes.Format(driveDetails.Size);
            Console.Clear();
            DuckDriveTitle.DisplayTitle();
            Console.WriteLine("Detalhes do Drive:");
            Console.WriteLine();
            Console.WriteLine(
                $"Path: {driveDetails.Path}, " +
                $"Name: {driveDetails.Name}, " +
                $"Size: {formatSize}, " +
                $"Type: {driveDetails.Type}, " +
                $"Format: {driveDetails.Format}");

            return driveDetails;
        });
    }

    public void MenuDrives()
    {
        while (true)
        {
            Console.Clear();
            DuckDriveTitle.DisplayTitle();
            Console.WriteLine("----- Menu de Drives -----");
            Console.WriteLine();
            Console.WriteLine("1. Listar todos os drives");
            Console.WriteLine("2. Exibir detalhes de um drive");
            Console.WriteLine("3. Sair");
            Console.WriteLine();
            Console.Write("Escolha uma opção: ");
            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    GetAllDriveAsync().Wait();
                    break;

                case "2":
                    Console.Write("Digite o path(ex: F:) do drive: ");
                    var pathLine = Console.ReadLine();
                    GetDriveDetailsAsync(pathLine).Wait();
                    break;
                
                case "3":
                    Console.WriteLine("Saindo...");
                    return;

                default:
                    Console.WriteLine("Opção inválida! Tente novamente.");
                    break;
            }

            Console.WriteLine("Pressione qualquer tecla para voltar ao Menu de Drives...");
            Console.ReadKey();
        }
    }
}