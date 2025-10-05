using System.Reflection;

namespace MiniSharp.ast;

public class AssignmentNode : ASTDebug
{
    public ASTNode Target { get; set; }
    public ExpressionNode Expression { get; set; }

    public override void Print(string indent)
    {
        Console.WriteLine(indent + "AssignmentNode: LINE " + Line);
        Target.Print(indent + "  ");
        Expression.Print(indent + "  ");
    }

    public override object? Execute(Context context)
    {
        Debug(context);
        var value = Expression.Execute(context);

        // Handle target types
        switch (Target)
        {
            case VarNameNode varNode:
                context.Set(varNode.Value, value);
                break;

            case MemberAccessNode memberNode:
                SetMember(memberNode, context, value);
                break;

            case ArrayAccessNode arrayNode:
                SetArrayValue(arrayNode, context, value);
                break;

            default:
                throw new Exception("Invalid assignment target");
        }

        return null;
    }

    private void SetMember(MemberAccessNode memberNode, Context context, object? value)
    {
        var leftVal = memberNode.Left.Execute(context);
        if (leftVal == null)
            throw new Exception("Cannot assign to null target");

        // Dictionaries
        if (leftVal is Dictionary<string, object?> dict)
        {
            dict[memberNode.Member] = value;
            return;
        }

        // Context
        if (leftVal is Context ctx)
        {
            ctx.Set(memberNode.Member, value);
            return;
        }

        // Reflection (C# object)
        var type = leftVal.GetType();
        var prop = type.GetProperty(memberNode.Member, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (prop != null)
        {
            prop.SetValue(leftVal, Convert.ChangeType(value, prop.PropertyType));
            return;
        }

        var field = type.GetField(memberNode.Member, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (field != null)
        {
            field.SetValue(leftVal, Convert.ChangeType(value, field.FieldType));
            return;
        }

        throw new Exception($"Cannot assign to member '{memberNode.Member}' of type '{type.Name}'");
    }

    private void SetArrayValue(ArrayAccessNode arrayNode, Context context, object? value)
    {
        var current = arrayNode.Target.Execute(context);
        var indexValues = arrayNode.Index.Execute(context) as List<object?> 
                          ?? throw new Exception("Invalid array index");

        if (indexValues.Count == 0)
            throw new Exception("Empty index");

        // Traverse all but the last index
        for (int i = 0; i < indexValues.Count - 1; i++)
        {
            int idx = (int)Convert.ToInt64(indexValues[i]);
            if (current is List<object?> list)
            {
                if (idx < 0 || idx >= list.Count)
                    throw new Exception($"Index {idx} out of bounds");
                current = list[idx];
            }
            else
            {
                throw new Exception($"Cannot index type '{current?.GetType().Name ?? "null"}'");
            }
        }

        // Set the final element
        int lastIdx = (int)Convert.ToInt64(indexValues.Last());
        if (current is List<object?> finalList)
        {
            if (lastIdx < 0 || lastIdx >= finalList.Count)
                throw new Exception($"Index {lastIdx} out of bounds");
            finalList[lastIdx] = value;
        }
        else
        {
            throw new Exception($"Cannot assign to non-list type '{current?.GetType().Name ?? "null"}'");
        }
    }

}
