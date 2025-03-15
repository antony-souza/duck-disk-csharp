namespace DuckDisk.utils.BootableDriveUtilClass;

public class CreateTempPathScriptGeneric
{
    private readonly ExecuteCommandGeneric  _executeCommandGeneric = new ExecuteCommandGeneric();
    
    public async Task<bool> CreateTempPathScript(string scriptPath, string[] commands)
    {
        string createPathCommand = Path.Combine(Path.GetTempPath(), scriptPath);
        //Aqui é o responsável por criar o arquivo txt do script e colar os comandos
        await File.WriteAllLinesAsync(createPathCommand, commands);

        return await _executeCommandGeneric.ExecuteCommand("diskpart", $"/s {createPathCommand}");
    }
}