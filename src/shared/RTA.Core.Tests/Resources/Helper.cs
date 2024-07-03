using System.Reflection;

namespace RTA.Core.Tests.Resources;

internal static class Helper
{
    public static async Task<string> GetFileAsync(string resourceName)
    {
        var resourcePath = $"RTA.Core.Tests.Resources.{resourceName}";
        var assembly = Assembly.GetExecutingAssembly();
        await using var stream = assembly.GetManifestResourceStream(resourcePath);

        if (stream is null)
            throw new ArgumentException($"{resourcePath} not found on {assembly.FullName}");

        using StreamReader reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}