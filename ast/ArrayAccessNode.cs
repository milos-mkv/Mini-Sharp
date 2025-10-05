namespace MiniSharp.ast;

public class ArrayAccessNode : TermNode
{
    public ASTNode Target { get; set; }
    public ExpressionListNode Index {get;set;}
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "ArrayAccessNode:");
        Target.Print(indent + "  ");
        Index.Print(indent + "  ");
    }

    public override object? Execute(Context context)
    {
        Debug(context);

        // Evaluate the target (the array or object being indexed)
        var current = Target?.Execute(context);

        // Evaluate all indexes
        var indexValues = Index.Execute(context) as List<object?> 
                          ?? throw new Exception("Invalid index expression");

        foreach (var indexVal in indexValues)
        {
            if (current is List<object?> list)
            {
                var idx = (int)Convert.ToInt64(indexVal);
                if (idx < 0 || idx >= list.Count)
                    throw new Exception($"Index {idx} out of bounds");
                current = list[idx];
            }
            else
            {
                // You can’t index something that’s not a list
                throw new Exception($"Cannot index type '{current?.GetType().Name ?? "null"}'");
            }
        }

        return current;
    }


}