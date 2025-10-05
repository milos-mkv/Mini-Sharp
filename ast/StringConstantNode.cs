namespace MiniSharp.ast;

public class StringConstantNode(string value) : TermNode
{
    public string Value { get; set; } = value;

    public override void Print(string indent) => Console.WriteLine(indent + "StringConstantNode: " + Value + " LINE " + Line);

    public override object? Execute(Context context)
    {
        Debug(context);
        return Value;   
    }
}