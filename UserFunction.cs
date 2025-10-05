using MiniSharp.ast;

namespace MiniSharp;

public class UserFunction(List<string> parameters, StatementsNode body) : ICallable
{
    public List<string> Parameters { get; } = parameters;
    public StatementsNode Body { get; } = body;

    public object? Call(Context context, List<object?> args) {
        if (args.Count != Parameters.Count)
            throw new Exception($"Expected {Parameters.Count} args, got {args.Count}");

        var fnContext = new Context(context);
        for (int i = 0; i < Parameters.Count; i++) {
            fnContext.Define(Parameters[i], args[i]);
        }

        try
        {
            return Body.Execute(fnContext);
        }
        catch (ReturnSignal e)
        {
            return e.Value;
        }
    }
}