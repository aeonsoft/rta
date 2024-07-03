using Microsoft.Extensions.Logging;
using RTA.Core.Interpreters;

namespace RTA.Core.Functions;

/// <summary>
/// Start a New Webdriver Session.
/// If the function succeed, a "session_id" variable is placed on context vars
/// </summary>
public class NewSession(ILogger<NewSession> logger) : Function
{    
    public override (bool sucess, string[]? errors) Run(State? context)
    {
        logger.LogTrace($"Running {Name}");
        if (context is null)
            return (false, new string[] { "Context is required" });

        try
        {
            context.Data["session_id"] = SpawnNewSession();
        }
        catch (Exception e)
        {
            return (false, new string[] { e.Message });
        }

        return (true, null);
    }

    private string SpawnNewSession()
    {
        throw new NotImplementedException();
    }
}
