namespace MiniSharp.ast;

public abstract class ASTNode
{
    public int Line { get; set; }
    public abstract void Print(string indent);
    public abstract object? Execute(Context context);
}