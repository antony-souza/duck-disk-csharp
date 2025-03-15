using DuckDisk.models;

namespace DuckDisk.interfaces;

public interface IIso
{
    Task<IsoModel> GetIsoDetails(string isoPath);
}