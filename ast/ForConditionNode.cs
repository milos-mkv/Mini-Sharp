namespace MiniSharp.ast;

public class ForConditionNode : ASTDebug
{
    public string VarName { get; set; }
    public ASTNode Start { get; set; }
    public ASTNode End { get; set; }
    public ASTNode Increment { get; set; }
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "ForConditionNode:"+Line);
        Start.Print(indent + "  ");
        End.Print(indent + "  ");
        Increment.Print(indent + "  ");
    }

    public override List<object> Execute(Context context)
    {
        Debug(context);
        return null;
    }
}