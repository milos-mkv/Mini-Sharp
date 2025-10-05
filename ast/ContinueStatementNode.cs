namespace MiniSharp.ast;

public class ContinueException : Exception
{
    
}

public class ContinueStatementNode : ASTNode
{
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "ContinueStatementNode:");
    }

    public override object? Execute(Context context)
    {
        throw new ContinueException();
    }
}