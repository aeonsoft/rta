using System.Collections;
using Microsoft.Extensions.Logging;
using RTA.Core.Functions;
using RTA.Core.Tests;

namespace RTA.Core.Interpreters;

public class BasicInterpreter(ILogger<BasicInterpreter> logger) : IInterpreter
{
    private readonly Dictionary<string, Function> _arrange = new Dictionary<string, Function>();
    private readonly Dictionary<string, Function> _act = new Dictionary<string, Function>();
    private readonly Dictionary<string, Function> _assert = new Dictionary<string, Function>();

    
    public void Run(Test test)
    {
        logger.LogInformation($"Running test '{test.Name}'...");
        RunArrangeSection(test);
        RunActSection(test);
        RunAssertSection(test);
    }



    public void Register(Test.Section section, Function function) {
        switch (section){
            case Test.Section.Act: 
                if (!_act.TryAdd(function.Name, function))
                    _act[function.Name] = function;
            break;
            case Test.Section.Arrange: 
                if (!_arrange.TryAdd(function.Name, function))
                    _arrange[function.Name] = function;
            break;
            case Test.Section.Assert:     
                if (!_assert.TryAdd(function.Name, function))
                    _assert[function.Name] = function;
            break;
        }

    }

    public (bool result, string[]? errors) IsValidSession(Test.Section section, Test test) {
        return section switch
        {
            Test.Section.Act => IsActSessionValid(test),
            Test.Section.Assert => IsAssertSessionValid(test),
            Test.Section.Arrange => IsArrangeSessionValid(test),
            _ => throw new InvalidOperationException("Unknown section")
        };
    }

    private (bool, string[]?) IsArrangeSessionValid(Test test) {
        var errors = new List<string>();

        foreach(var func in test.Assert.Keys) {
            if (!_arrange.ContainsKey(func)) {
                errors.Add($"Function {func} is unknown");
            }
        }    
        return (errors.Count == 0)
            ? (true, null)
            : (false, errors.ToArray());        
    }

    private (bool, string[]?) IsAssertSessionValid(Test test) {
        var errors = new List<string>();
        if (test.Assert.Count == 0)
            errors.Add("Assert session is required and must contain at least one function");

        foreach(var func in test.Assert.Keys) {
            if (!_assert.ContainsKey(func)) {
                errors.Add($"Function {func} is unknown");
            }
        }            

        return (errors.Count == 0)
            ? (true, null)
            : (false, errors.ToArray());        
    }

    private (bool, string[]?) IsActSessionValid(Test test) {
        var errors = new List<string>();
        if (test.Act.Count == 0)
            errors.Add("Act session is required and must contain at least one function");

        foreach(var func in test.Act.Keys) {
            if (!_act.ContainsKey(func)) {
                errors.Add($"Function {func} is unknown");
            }
        }            

        return (errors.Count == 0)
            ? (true, null)
            : (false, errors.ToArray());        
    }
    

    private bool RunArrangeSection(Test test)
    {
        if (test.Arrange == null) return false;

        using (logger.BeginScope("arrange section"))
        {
            foreach (var key in test.Arrange.Keys)
            {
                logger.LogDebug($"running {key}...");
            }
        }

        return true;
    }

    private bool RunActSection(Test test)
    {
        throw new NotImplementedException();
    }

    private bool RunAssertSection(Test test)
    {
        throw new NotImplementedException();
    }
}