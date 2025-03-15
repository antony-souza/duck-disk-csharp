namespace DuckDisk.models;

public class DriveModel
{
    public string Name { get; init; } = String.Empty;
    public long Size { get; init; }
    public long FreeSpace { get; init; }
    public string Type { get; init; } = String.Empty;
    public string Format { get; init; } = String.Empty; 
    public string Path { get; init; } = String.Empty;
}