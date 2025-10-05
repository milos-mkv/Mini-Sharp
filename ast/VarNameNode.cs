namespace MiniSharp.ast;

public class VarNameNode(string value) : TermNode
{
    public string Value { get; set; } = value;

    public override void Print(string indent) => Console.WriteLine(indent + "VarNameNode: " + Value + " LINE " + Line);

    public override object? Execute(Context context)
    {
        Debug(context);
        return context.Get(Value);
    }

}