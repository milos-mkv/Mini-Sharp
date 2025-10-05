namespace MiniSharp.ast;

public class NumberConstantNode(string value) : TermNode
{
    public string Value { get; set; } = value;

    public override void Print(string indent) => Console.WriteLine(indent + "NumberConstantNode: " + Value + " LINE " + Line);

    public override object? Execute(Context context)
    {
        Debug(context);
        return Convert.ToDouble(Value);
    }
}