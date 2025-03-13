using DuckDisk.models;
using DuckDisk.services;

namespace DuckDisk;

class Program
{
    static async Task Main()
    {
        DriveService pendriveService = new DriveService();
        List<Drive> drives = await pendriveService.GetAllDriveAsync();
    }
}