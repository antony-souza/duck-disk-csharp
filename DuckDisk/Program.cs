using DuckDisk.models;
using DuckDisk.services;

namespace DuckDisk;

class Program
{
    static void Main()
    {
        DriveService driveService = new DriveService();
        driveService.MenuDrives();
    }
}