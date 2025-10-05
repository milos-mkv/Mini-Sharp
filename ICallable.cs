namespace MiniSharp;

public interface ICallable
{
    object? Call(Context context, List<object> args);
}