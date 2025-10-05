namespace MiniSharp.ast;

public class ProgramNode : ASTNode
{
    public StatementsNode Statements { get; set; }
    
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "ProgramNode: " + Line);
        Statements.Print(indent + "  ");
    }

    public override object? Execute(Context context) => Statements.Execute(context);
}