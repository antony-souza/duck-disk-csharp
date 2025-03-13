using DuckDisk.interfaces;
using DuckDisk.models;
using DuckDisk.Models;

namespace DuckDisk.services;

public class DriveService : IDriveService
{
    public async Task<List<Drive>> GetAllDriveAsync()
    {
        return await Task.Run(() =>
        {
            List<Drive> drives = new List<Drive>();

            DriveInfo[] allDrivesArray = DriveInfo.GetDrives();

            foreach (DriveInfo drive in allDrivesArray)
            {
                //Filtro por pendrive(Removable)
                /*if (pendrive.DriveType == DriveType.Removable){}*/

                drives.Add(new Drive
                {
                    Path = drive.Name,
                    Name = drive.VolumeLabel,
                    Size = drive.TotalSize,
                    Type = drive.DriveType.ToString(),
                    Format = drive.DriveFormat.ToString(),
                });
            }
            
            Console.Clear();
            Console.WriteLine("Drives Encontrados:");

            foreach (var drive in drives)
            {
                Console.WriteLine(
                    $"Path: {drive.Path}, " +
                    $"Name: {drive.Name}, " +
                    $"Size: {drive.Size}, " +
                    $"Type: {drive.Type}, " +
                    $"Format: {drive.Format}");
            }

            return drives;
        });
    }

    public Task<Drive> GetDriveDetailsAsync(string path)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FormatDriveAsync(Drive drive, FileSystemType fileSystem)
    {
        throw new NotImplementedException();
    }
}