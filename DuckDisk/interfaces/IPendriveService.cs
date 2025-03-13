using DuckDisk.models;
using DuckDisk.Models;

namespace DuckDisk.interfaces;

public interface IPendriveService
{
    Task<List<Pendrive>> GetAllPendriveAsync();
    Task<Pendrive> GetPendriveDetailsAsync(string path);
    Task<bool> FormatPendriveAsync(Pendrive pendrive, FileSystemType fileSystem);
}