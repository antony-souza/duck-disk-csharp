namespace DuckDisk.utils.BootableDriveUtilClass;

class CompyFilesFromIsoForDriveGeneric
{
   private readonly ExecuteCommandGeneric _executeCommandGeneric = new ExecuteCommandGeneric();
    public async Task<bool> CopyFiles(string isoPath, string drivePath)
    {

        string mountCommand = $"powershell Mount-DiskImage -ImagePath \"{isoPath}\"";
        if (!await _executeCommandGeneric.ExecuteCommand("powershell", $"-Command \"{mountCommand}\""))
        {
            return false;
        }

        DriveInfo mountedDrive = DriveInfo.GetDrives().FirstOrDefault(d => d.DriveFormat == "UDF");
        if (mountedDrive == null)
        {
            Console.WriteLine("Erro: Não foi possível montar a ISO.");
            return false;
        }

        string sourcePath = mountedDrive.RootDirectory.FullName;
        string destinationPath = drivePath;

        Console.WriteLine($"Copiando arquivos de {sourcePath} para {destinationPath}...");
        return await _executeCommandGeneric.ExecuteCommand("xcopy",
            $"\"{sourcePath}\" \"{destinationPath}\" /E /H /C /Y");
    }
}