using Microsoft.Extensions.Logging;
using RTA.Core.Interpreters;

namespace RTA.Core.Functions;

public class Foo(ILogger<Foo> logger): Function
{
    public override (bool sucess, string[]? errors) Run(State? context)
    {
        logger.LogTrace($"Running {Name}");
        logger.LogInformation("foo... bar.");
        return (true, null);
    }
}