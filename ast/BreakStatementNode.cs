namespace MiniSharp.ast;

public class BreakException : Exception
{
    
}

public class BreakStatementNode : ASTNode
{
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "BreakStatementNode:");
    }

    public override object? Execute(Context context)
    {
        throw new BreakException();
    }
}