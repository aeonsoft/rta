using RTA.Core.Tests;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RTA.Core.Parser;

public class YamlParser
{
    public Test Parse(string yml)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        
        return deserializer.Deserialize<Test>(yml);
    }
}