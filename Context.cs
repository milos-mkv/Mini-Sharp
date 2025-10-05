namespace MiniSharp;

public class Context(Context? parent = null)
{
    public IDebugger? Debugger { get; set; }
    public HashSet<int> Breakpoints { get; } = new();
    public int LastBreakLine { get; set; } = -1;

    public Dictionary<string, object?> Variables { get; set; } = new Dictionary<string, object?>();
    public Dictionary<string, ICallable> Functions { get; set; } = new();
    public Context? Parent { get; set; } = parent;
    
    public string? Name { get; set; }

    public IDebugger GetDebugger()
    {
        if (Debugger != null) return Debugger;
        if (parent != null) return parent.GetDebugger();
        return null;
    }

    public HashSet<int> GetBreakpoints()
    {
        if (parent != null) return parent.GetBreakpoints();
        return Breakpoints;
    }
    public void Define(string name, object? value) => Variables[name] = value;

    public object? Get(string name) {
        if (Variables.TryGetValue(name, out var v)) return v;
        if (Parent != null) return Parent.Get(name);
        throw new Exception($"Undefined variable: {name}");
    }

    public void Set(string name, object? value) {
        if (Variables.ContainsKey(name)) {
            Variables[name] = value;
        } else if (Parent != null) {
            Parent.Set(name, value);
        } else {
            throw new Exception($"Undefined variable: {name}");
        }
    }

    public void PrintVars()
    {
        if (parent != null)
        {
            parent.PrintVars();
        }
        foreach (var kv in Variables)
        {
            Console.WriteLine($"  {kv.Key} = {kv.Value}");
        }
    }
    
    
    // Functions
    public void DefineFunction(string name, ICallable fn) => Functions[name] = fn;
    public ICallable GetFunction(string name) =>
        Functions.TryGetValue(name, out var f) ? f :
            parent?.GetFunction(name) ?? throw new Exception($"Undefined function: {name}");
}