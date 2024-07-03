namespace RTA.Core.Interpreters;

/// <summary>
/// Interpreter state.
/// All functions in a run can change this state.
/// You can think this as a global variables for a interpreter
/// </summary>
public class State
{
    private const string SessionIdKey = "session_id";
    
    public Dictionary<string, object> Data => new Dictionary<string, object>();

    /// <summary>
    /// Web Driver's session id.
    /// </summary>
    public string? SessionId
    {
        get
        {
            if (Data.TryGetValue(SessionIdKey, out var result))
                return (string)result;

            return null;
        }
        set
        {
            if (value is null)
                Data.Remove(SessionIdKey);
            else
                Data[SessionIdKey] = value;
        }
    }
}