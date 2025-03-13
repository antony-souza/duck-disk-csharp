namespace DuckDisk.Models;

public enum FileSystemTypeEnum
{
    FAT32,
    NTFS,
    exFAT
}

public class FileSystemType
{
    public FileSystemTypeEnum Type { get; set; }
    public int ClusterSize { get; set; }

    public FileSystemType(FileSystemTypeEnum type, int clusterSize)
    {
        Type = type;
        ClusterSize = clusterSize;
    }

    public static FileSystemType FAT32 => new(FileSystemTypeEnum.FAT32, 4096);
    public static FileSystemType NTFS => new(FileSystemTypeEnum.NTFS, 4096);
    public static FileSystemType exFAT => new(FileSystemTypeEnum.exFAT, 8192);
}