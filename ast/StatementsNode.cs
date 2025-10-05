namespace MiniSharp.ast;

public class StatementsNode : ASTDebug
{
    public List<ASTNode> Statements { get; set; } = [];

    public StatementsNode()
    {
    }
    
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "StatementsNode:"+Line);
        foreach (var statement in Statements)
        {
            statement.Print(indent + "  ");
        }
    }

    public override object? Execute(Context context)
    {
        Debug(context);
       
        foreach (var statement in Statements)
        {
            statement.Execute(context);
        }
        return null;
    }
}