using DuckDisk.models;
using DuckDisk.Models;

namespace DuckDisk.interfaces;

public interface IDriveService
{
    Task<List<Drive>> GetAllDriveAsync();
    Task<Drive> GetDriveDetailsAsync(string path);
    Task<bool> FormatDriveAsync(Drive drive, FileSystemType fileSystem);
}