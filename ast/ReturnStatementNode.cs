namespace MiniSharp.ast;

public class ReturnSignal : Exception
{
    public object? Value { get; }
    public ReturnSignal(object? value) { Value = value; }
}
public class ReturnStatementNode : ASTDebug
{
    public ExpressionNode Expression { get; set; }
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "ReturnStatementNode:"+Line);
        Expression.Print(indent + "  ");
    }

    public override List<object> Execute(Context context)
    {
        Debug(context);
        object? value = Expression?.Execute(context);
        throw new ReturnSignal(value);
    }
}