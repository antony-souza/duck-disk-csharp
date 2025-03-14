using DuckDisk.models;

namespace DuckDisk.interfaces;

public interface IDrive
{
    Task<List<DriveModel>> GetAllDriveAsync();
    Task<DriveModel> GetDriveDetailsAsync(string path);
}