using Microsoft.Extensions.Logging;

namespace RTA.Core.Functions;

public class Function
{
    public required string Name { get; init; }

    public Arguments? Arguments { get; } = new Arguments();

    /// <summary>
    /// A 'function run' have access to "arguments" and may receive a context dictionary
    /// </summary>
    /// <param name="context">Environment context variables</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual (bool success, string[]? errors) Run(IDictionary<string, object>? context)
    {
        throw new NotImplementedException();
    }
}
