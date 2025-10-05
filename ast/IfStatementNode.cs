namespace MiniSharp.ast;

public class IfStatementNode : ASTDebug
{
    public ExpressionNode Condition { get; set; }
    public StatementsNode Statements { get; set; }
    public ASTNode? Else { get; set; }
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "IfStatementNode:"+Line);
        Condition.Print(indent + "  ");
        Statements.Print(indent + "  ");
        Else?.Print(indent + "  ");
    }

    public override List<object> Execute(Context context)
    {
        Debug(context);
        var currentContext = new Context(context);
        if (Convert.ToBoolean(Condition.Execute(context)))
        {
            Statements.Execute(currentContext);
        }
        else
        { 
            Else?.Execute(currentContext);
        }
        return null;
    }
}