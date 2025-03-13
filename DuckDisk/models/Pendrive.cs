namespace DuckDisk.models;

public class Pendrive
{
    public string Name { get; set; } = String.Empty;
    public long Size { get; set; }
    public string Path { get; set; } = String.Empty;
    public bool IsBootable { get; set; }
}