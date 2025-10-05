namespace MiniSharp.ast;

public class DoStatementNode : ASTDebug
{
    public SubroutineCallNode Call { get; set; }
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "DoStatementNode: " + Line);
        Call.Print(indent + "  ");
    }

    public override List<object> Execute(Context context)
    {
        Debug(context);
        Call.Execute(context);
        return null;
    }
}