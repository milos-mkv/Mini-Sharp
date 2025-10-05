using MiniSharp.ast;

namespace MiniSharp;

public class AnonymousFunction : ICallable
{
    public List<string> Parameters { get; }
    public StatementsNode Body { get; }
    public Context Closure { get; }

    public AnonymousFunction(List<string> parameters, StatementsNode body, Context closure)
    {
        Parameters = parameters;
        Body = body;
        Closure = closure;
    }

    public object? Call(Context callerContext, List<object?> args)
    {
        // Create a new local context inheriting the closure
        var local = new Context(Closure);

        // Bind parameters
        for (int i = 0; i < Parameters.Count; i++)
        {
            local.Define(Parameters[i], i < args.Count ? args[i] : null);
        }

        // Execute the body
        try
        {
            return Body.Execute(local);
        }
        catch (ReturnSignal e)
        {
            return e.Value;
        }

        // Handle return propagation

    }
}
