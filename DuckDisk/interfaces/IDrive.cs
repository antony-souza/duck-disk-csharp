using DuckDisk.models;

namespace DuckDisk.interfaces;

public interface IDrive
{
    Task<List<Drive>> GetAllDriveAsync();
    Task<Drive> GetDriveDetailsAsync(string path);
    Task<bool> FormatDriveAsync(Drive drive);
    void MenuDrives();
}