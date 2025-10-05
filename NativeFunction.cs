namespace MiniSharp;

public class NativeFunction : ICallable
{
    private readonly Func<List<object?>, object?> impl;

    public NativeFunction(Func<List<object?>, object?> impl) 
    {
        this.impl = impl;
    }

    public object? Call(Context context, List<object> args)
    {
        return impl(args);
    }
}