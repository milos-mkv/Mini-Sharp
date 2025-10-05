using System.Reflection;

namespace MiniSharp.ast;

public class MemberAccessNode : TermNode
{
    public TermNode Left { get; set; }
    public string Member { get; set; }
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "MemberAccessNode:" + Member);
        Left.Print(indent + "  ");
    }

    public override object? Execute(Context context)
    {
        var leftValue = Left.Execute(context);

        // Null safety
        if (leftValue == null)
            throw new Exception($"Cannot access member '{Member}' on null target");

        // Language object (Dictionary)
        if (leftValue is Dictionary<string, object?> obj)
        {
            if (obj.TryGetValue(Member, out var value))
                return value;
            throw new Exception($"No member '{Member}' on object (Dictionary)");
        }

        // Script context (variable scope)
        if (leftValue is Context ctx)
        {
            return ctx.Get(Member);
        }

        // Reflection on real .NET object
        var type = leftValue.GetType();

        const BindingFlags flags =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.IgnoreCase |
            BindingFlags.FlattenHierarchy;
        
        // Try constant (literal) fields
        var constField = type.GetField(Member, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
        if (constField != null && constField.IsLiteral && !constField.IsInitOnly)
        {
            return constField.GetRawConstantValue();
        }


        // Try property (readable)
        var prop = type.GetProperty(Member, flags);
        if (prop != null && prop.CanRead)
            return prop.GetValue(leftValue);

        // Try field (public instance)
        var field = type.GetField(Member, flags);
        if (field != null)
            return field.GetValue(leftValue);

        // Try method (zero-arg)
        var method = type.GetMethod(Member, flags);
        if (method != null)
        {
            if (method.GetParameters().Length == 0)
            {
                // Return callable (deferred invocation)
                return new NativeFunction(args => method.Invoke(leftValue, null));
            }
            else
            {
                // Return callable that accepts arguments
                return new NativeFunction(args => method.Invoke(leftValue, args?.ToArray()));
            }
        }

        // Nothing found
        throw new Exception($"Type '{type.Name}' has no member '{Member}'");
    }


}