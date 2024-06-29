using RTA.Core.Functions;
using RTA.Core.Tests;

namespace RTA.Core.Interpreters;



public interface IInterpreter
{
    void Run(Test test);
    
    void Register(Test.Section section, Function function);

    (bool result, string[]? errors) IsValidSession(Test.Section section, Test test);
}