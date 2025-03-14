using DuckDisk.models;

namespace DuckDisk.interfaces;

public interface IBootableDrive
{
    Task<bool> FormatDriveAsync(string drivePath);
    Task<bool> CreateBootableDriveAsync(BootableDriveModel driveModel);
}