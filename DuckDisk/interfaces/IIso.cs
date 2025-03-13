using DuckDisk.models;

namespace DuckDisk.interfaces;

public interface IIsoService
{
    Task<IsoModel> LoadIsoAsync(string isoPath);
    Task<bool> ValidateIsoAsync(string isoPath);
}