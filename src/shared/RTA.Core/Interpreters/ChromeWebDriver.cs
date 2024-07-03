using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RTA.Core.Interpreters;

public class ChromeWebDriver(Interpreters.Config config)
{
    private Process? _process = null;
        
    public void StartServer()
    {
        var psi = new ProcessStartInfo {
            FileName = config.WebDriverPath,
            UseShellExecute = false,
            Arguments = $"--port={config.WebDriverPort}",
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };
        try {
            _process = Process.Start(psi);
        } catch {
            psi.UseShellExecute = true;
            _process = Process.Start(psi);
        }
    }

    public void CloseServer()
    {
        if (ServerRunning())
        {
            _process?.Close();
            _process = null;
        }
    }
    
    
    /// <summary>
    /// Check if there is a Chrome web driver running that was spawqned by this proccess
    /// </summary>
    /// <returns></returns>
    public bool ServerRunning()
    {
        return (_process != null);
    }
}