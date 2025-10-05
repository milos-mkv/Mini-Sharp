namespace MiniSharp.ast;

public class ExpressionListNode : TermNode
{
    public List<ExpressionNode> Expressions { get; set; } = new();
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "ExpressionListNode: " + Line);
        foreach (var expression in Expressions)
        {
            expression.Print(indent + "  ");
        }
    }

    public override object? Execute(Context context)
    {
        Debug(context);
        List<object?> values = new List<object?>();
        foreach (var expression in Expressions)
        {
            values.Add(expression.Execute(context));
        }
        return values;
    }
}