namespace DuckDisk.models;

public class Drive
{
    public string Name { get; set; } = String.Empty;
    public long Size { get; init; }
    public string Type { get; set; } = String.Empty;
    public string Format { get; set; } = String.Empty; 
    public string Path { get; init; } = String.Empty;
    public bool IsBootable { get; set; }
}