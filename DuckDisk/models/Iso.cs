﻿namespace DuckDisk.models;

public class Iso
{
    public string Path { get; set; } = String.Empty;
    public long Size { get; set; }
    public string VolumeLabel { get; set; } = String.Empty;
}