namespace MiniSharp.ast;

public class ArrayLiteralNode : TermNode
{
    public List<ExpressionNode> Elements { get; set; } = new();

    public override void Print(string indent)
    {
        Console.WriteLine(indent + "ArrayLiteralNode");
    }

    public override object? Execute(Context context)
    {
        var values = new List<object?>();
        foreach (var expr in Elements)
            values.Add(expr.Execute(context));
        return values;
    }
}