using System.Reflection;

namespace MiniSharp.ast;

public class SubroutineCallNode : TermNode
{
    public ASTNode Target { get; set; }
    public string FunctionName { get; set; }
    public ExpressionListNode ExpressionList { get; set; }
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "SubroutineCallNode: " + FunctionName + " LINE " + Line);
        if (Target != null)
        Target.Print(indent + "  ");
        ExpressionList.Print(indent + "  ");
    }

    public override object? Execute(Context context)
    {
        Debug(context);

        object? targetValue = null;
        ICallable? func = null;

        // Evaluate the target (e.g., Std, arr[0], ()->{}, etc.)
        if (Target != null)
        {
            targetValue = Target.Execute(context);

            // Case A: the target itself is callable (anonymous or array element)
            if (targetValue is ICallable callableTarget && FunctionName == null)
            {
                var argsAnon = ExpressionList?.Execute(context) as List<object?> ?? new();
                return callableTarget.Call(context, argsAnon);
            }

            // Case B: the target is a script context
            if (targetValue is Context targetCtx && FunctionName != null)
            {
                func = targetCtx.Get(FunctionName) as ICallable;
            }
            // Case C: the target is a dictionary object
            else if (targetValue is Dictionary<string, object?> objDict && FunctionName != null)
            {
                if (objDict.TryGetValue(FunctionName, out var maybeFunc) && maybeFunc is ICallable callable)
                    func = callable;
            }
            // Case D: static .NET method call
            else if (targetValue is Type typeTarget && FunctionName != null)
            {
                var args = ExpressionList?.Execute(context) as List<object?> ?? new();
                var method = typeTarget.GetMethod(
                    FunctionName,
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase
                );

                if (method == null)
                    throw new Exception($"Static method '{FunctionName}' not found on type '{typeTarget.Name}'");

                return InvokeWithReflection(null, method, args);
            }
            // Case E: instance .NET object method call
            else if (targetValue != null && FunctionName != null)
            {
                var args = ExpressionList?.Execute(context) as List<object?> ?? new();
                var type = targetValue.GetType();

                var method = type.GetMethod(
                    FunctionName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase
                );

                if (method != null)
                    return InvokeWithReflection(targetValue, method, args);
            }
        }
        else
        {
            // No explicit target — global interpreter call
            if (FunctionName != null)
                func = context.Get(FunctionName) as ICallable;
        }

        // If still no function found → error
        if (func == null)
            throw new Exception($"Function '{FunctionName ?? "<anonymous>"}' not found or not callable");

        // Evaluate arguments and call
        var argsList = ExpressionList?.Execute(context) as List<object?> ?? new();
        if (targetValue != null)
            argsList.Insert(0, targetValue);

        return func.Call(context, argsList);
    }

private static object? InvokeWithReflection(object? target, MethodInfo method, List<object?> args)
{
    var parameters = method.GetParameters();
    object?[] callArgs;

    // Handle [params] (variable-argument) methods
    if (parameters.Length == 1 && Attribute.IsDefined(parameters[0], typeof(ParamArrayAttribute)))
    {
        var elementType = parameters[0].ParameterType.GetElementType() ?? typeof(object);
        Array arr = Array.CreateInstance(elementType, args.Count);
        for (int i = 0; i < args.Count; i++)
            arr.SetValue(Convert.ChangeType(args[i], elementType), i);
        callArgs = new object?[] { arr };
    }
    else
    {
        if (args.Count != parameters.Length)
            throw new Exception(
                $"Method '{method.Name}' expects {parameters.Length} args, got {args.Count}"
            );

        callArgs = new object?[args.Count];
        for (int i = 0; i < args.Count; i++)
            callArgs[i] = Convert.ChangeType(args[i], parameters[i].ParameterType);
    }

    return method.Invoke(target, callArgs);
}

}