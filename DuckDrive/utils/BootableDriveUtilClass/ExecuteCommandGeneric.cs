using System.Diagnostics;

namespace DuckDisk.utils.BootableDriveUtilClass;

public class ExecuteCommandGeneric
{
    public async Task<bool> ExecuteCommand(string executableProcess, string commands)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = executableProcess,
            Arguments = commands,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (var process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Erro ao executar comando: {error}");
                    return false;
                }

                Console.WriteLine(output);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao executar processo: {ex.Message}");
            return false;
        }
    }
}