using DuckDisk.models;

namespace DuckDisk.interfaces;

public interface IIsoService
{
    Task<Iso> LoadIsoAsync(string isoPath);
    Task<bool> ValidateIsoAsync(string isoPath);
}